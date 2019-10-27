using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Authentication;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Abstracts.Factories;
using Genbox.SimpleS3.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Errors;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Requests
{
    /// <summary>Handles common request and response logic before sending to transport drivers.</summary>
    [PublicAPI]
    public class DefaultRequestHandler : IRequestHandler
    {
        private readonly IAuthorizationBuilder _authBuilder;
        private readonly ILogger<DefaultRequestHandler> _logger;
        private readonly IMarshalFactory _marshaller;
        private readonly INetworkDriver _networkDriver;
        private readonly IOptions<S3Config> _options;
        private readonly IList<IRequestStreamWrapper> _requestStreamWrappers;
        private readonly IValidatorFactory _validator;

        public DefaultRequestHandler(IOptions<S3Config> options, IValidatorFactory validator, IMarshalFactory marshaller, INetworkDriver networkDriver, IAuthorizationBuilder authBuilder, IEnumerable<IRequestStreamWrapper> requestStreamWrappers, ILogger<DefaultRequestHandler> logger)
        {
            Validator.RequireNotNull(options, nameof(options));
            Validator.RequireNotNull(validator, nameof(validator));
            Validator.RequireNotNull(marshaller, nameof(marshaller));
            Validator.RequireNotNull(networkDriver, nameof(networkDriver));
            Validator.RequireNotNull(authBuilder, nameof(authBuilder));
            Validator.RequireNotNull(requestStreamWrappers, nameof(requestStreamWrappers));
            Validator.RequireNotNull(logger, nameof(logger));

            validator.ValidateAndThrow(options.Value);

            _validator = validator;
            _options = options;
            _networkDriver = networkDriver;
            _authBuilder = authBuilder;
            _marshaller = marshaller;
            _requestStreamWrappers = requestStreamWrappers.ToList();
            _logger = logger;
        }

        public async Task<TResp> SendRequestAsync<TReq, TResp>(TReq request, CancellationToken cancellationToken) where TResp : IResponse, new() where TReq : IRequest
        {
            _logger.LogTrace($"Sending {typeof(TReq)} to bucket '{request.BucketName}' with key '{request.ObjectKey}'");

            Stream requestStream = _marshaller.MarshalRequest(request, _options.Value);

            _validator.ValidateAndThrow(request);

            //Ensure that the object key is encoded
            string encodedResource = UrlHelper.UrlPathEncode(request.ObjectKey);

            if (_options.Value.Endpoint == null || _options.Value.NamingType == NamingType.PathStyle)
            {
                if (!string.IsNullOrEmpty(request.BucketName))
                    request.ObjectKey = request.BucketName + '/' + encodedResource;
                else
                    request.ObjectKey = encodedResource;
            }
            else
                request.ObjectKey = encodedResource;

            StringBuilder sb = new StringBuilder(512);

            if (_options.Value.Endpoint != null)
                sb.Append(_options.Value.Endpoint.Host);
            else
            {
                if (_options.Value.NamingType == NamingType.VirtualHost)
                {
                    if (!string.IsNullOrEmpty(request.BucketName))
                        sb.Append(request.BucketName).Append(".s3.").Append(ValueHelper.EnumToString(_options.Value.Region)).Append(".amazonaws.com");
                    else
                        sb.Append("s3.").Append(ValueHelper.EnumToString(_options.Value.Region)).Append(".amazonaws.com");
                }
                else
                    sb.Append("s3.").Append(ValueHelper.EnumToString(_options.Value.Region)).Append(".amazonaws.com");
            }

            request.AddHeader(HttpHeaders.Host, sb.ToString());
            request.AddHeader(AmzHeaders.XAmzDate, request.Date, DateTimeFormat.Iso8601DateTime);

            if (requestStream != null && _requestStreamWrappers != null)
            {
                foreach (IRequestStreamWrapper wrapper in _requestStreamWrappers)
                {
                    if (wrapper.IsSupported(request))
                        requestStream = wrapper.Wrap(requestStream, request);
                }
            }

            //We check if it was already added here because it might have been due to streaming support
            if (!request.Headers.ContainsKey(AmzHeaders.XAmzContentSha256))
            {
                string contentHash;

                if (!_options.Value.EnablePayloadSigning)
                    contentHash = "UNSIGNED-PAYLOAD";
                else
                    contentHash = requestStream == null ? "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855" : CryptoHelper.Sha256Hash(requestStream, true).HexEncode();

                _logger.LogDebug("ContentSha256 is {ContentSha256}", contentHash);

                request.AddHeader(AmzHeaders.XAmzContentSha256, contentHash);
            }

            //We add the authorization header here because we need ALL other headers to be present when we do
            request.AddHeader(HttpHeaders.Authorization, _authBuilder.BuildAuthorization(request));

            sb.Append('/').Append(request.ObjectKey);

            //Map all the parameters on to the url
            if (request.QueryParameters.Count > 0)
                sb.Append('?').Append(UrlHelper.CreateQueryString(request.QueryParameters));

            string fullUrl = "https://" + sb;

            _logger.LogDebug("Building request for {Url}", fullUrl);

            (int statusCode, IDictionary<string, string> headers, Stream responseStream) = await _networkDriver.SendRequestAsync(request.Method, fullUrl, request.Headers, requestStream, cancellationToken).ConfigureAwait(false);

            //Clear sensitive material from the request
            if (request is IContainSensitiveMaterial sensitive)
                sensitive.ClearSensitiveMaterial();

            TResp response = new TResp();
            response.StatusCode = statusCode;
            response.ContentLength = headers.GetHeaderLong(HttpHeaders.ContentLength);
            response.ConnectionClosed = "closed".Equals(headers.GetHeader(HttpHeaders.Connection), StringComparison.OrdinalIgnoreCase);
            response.Date = headers.GetHeaderDate(HttpHeaders.Date, DateTimeFormat.Rfc1123);
            response.Server = headers.GetHeader(HttpHeaders.Server);
            response.ResponseId = headers.GetHeader(AmzHeaders.XAmzId2);
            response.RequestId = headers.GetHeader(AmzHeaders.XAmzRequestId);

            // https://docs.aws.amazon.com/AmazonS3/latest/API/ErrorResponses.html
            response.IsSuccess = !(statusCode == 403 //Forbidden
                                   || statusCode == 400 //BadRequest
                                   || statusCode == 500 //InternalServerError
                                   || statusCode == 416 //RequestedRangeNotSatisfiable
                                   || statusCode == 405 //MethodNotAllowed
                                   || statusCode == 411 //LengthRequired
                                   || statusCode == 404 //NotFound
                                   || statusCode == 501 //NotImplemented
                                   || statusCode == 504 //GatewayTimeout
                                   || statusCode == 301 //MovedPermanently
                                   || statusCode == 412 //PreconditionFailed
                                   || statusCode == 307 //TemporaryRedirect
                                   || statusCode == 409 //Conflict
                                   || statusCode == 503); //ServiceUnavailable

            //Don't know why, but ContentLength seems to be 0 sometimes, even though it is in the headers
            if (!response.IsSuccess && (response.ContentLength > 0 || responseStream.Length > 0))
            {
                using (responseStream)
                    response.Error = ErrorHandler.Create(responseStream);

                _logger.LogError("Received error: '{Message}'. Details: '{Details}'", response.Error.Message, response.Error.GetExtraData());
            }

            //Only marshal successful responses
            if (response.IsSuccess)
                _marshaller.MarshalResponse(request, response, headers, responseStream);

            return response;
        }
    }
}