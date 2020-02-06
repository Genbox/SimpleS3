using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Errors;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Network
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

        public async Task<TResp> SendRequestAsync<TReq, TResp>(TReq request, CancellationToken cancellationToken = default) where TResp : IResponse, new() where TReq : IRequest
        {
            request.Timestamp = DateTimeOffset.UtcNow;
            request.RequestId = Guid.NewGuid();

            _logger.LogTrace("Sending {RequestType} with request id {RequestId}", typeof(TReq).Name, request.RequestId);

            Stream requestStream = _marshaller.MarshalRequest(request, _options.Value);

            _validator.ValidateAndThrow(request);

            string bucketName = null;

            if (request is IHasBucketName bn)
                bucketName = bn.BucketName;

            string objectKey = null;

            if (request is IHasObjectKey ok)
                objectKey = ok.ObjectKey;

            //Ensure that the object key is encoded
            string encodedResource = objectKey != null ? UrlHelper.UrlPathEncode(objectKey) : null;

            if (_options.Value.Endpoint == null || _options.Value.NamingMode == NamingMode.PathStyle)
            {
                if (bucketName != null)
                    objectKey = bucketName + '/' + encodedResource;
                else
                    objectKey = encodedResource;
            }
            else
                objectKey = encodedResource;

            StringBuilder sb = StringBuilderPool.Shared.Rent(100);

            if (_options.Value.Endpoint != null)
                sb.Append(_options.Value.Endpoint.Host);
            else
            {
                if (_options.Value.NamingMode == NamingMode.VirtualHost)
                {
                    if (bucketName != null)
                        sb.Append(bucketName).Append(".s3.").Append(ValueHelper.EnumToString(_options.Value.Region)).Append(".amazonaws.com");
                    else
                        sb.Append("s3.").Append(ValueHelper.EnumToString(_options.Value.Region)).Append(".amazonaws.com");
                }
                else
                    sb.Append("s3.").Append(ValueHelper.EnumToString(_options.Value.Region)).Append(".amazonaws.com");
            }

            request.SetHeader(HttpHeaders.Host, sb.ToString());
            request.SetHeader(AmzHeaders.XAmzDate, request.Timestamp, DateTimeFormat.Iso8601DateTime);

            if (requestStream != null)
            {
                foreach (IRequestStreamWrapper wrapper in _requestStreamWrappers)
                {
                    if (wrapper.IsSupported(request))
                        requestStream = wrapper.Wrap(requestStream, request);
                }
            }

            if (!request.Headers.TryGetValue(AmzHeaders.XAmzContentSha256, out string contentHash))
            {
                if (_options.Value.PayloadSignatureMode == SignatureMode.Unsigned)
                    contentHash = "UNSIGNED-PAYLOAD";
                else
                    contentHash = requestStream == null ? "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855" : CryptoHelper.Sha256Hash(requestStream, true).HexEncode();

                request.SetHeader(AmzHeaders.XAmzContentSha256, contentHash);
            }

            _logger.LogDebug("ContentSha256 is {ContentSha256}", contentHash);

            //We add the authorization header here because we need ALL other headers to be present when we do
            request.SetHeader(HttpHeaders.Authorization, _authBuilder.BuildAuthorization(request));

            sb.Append('/').Append(objectKey);

            //Map all the parameters on to the url
            if (request.QueryParameters.Count > 0)
                sb.Append('?').Append(UrlHelper.CreateQueryString(request.QueryParameters));

            string fullUrl = "https://" + sb;

            StringBuilderPool.Shared.Return(sb);

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

            if (!response.IsSuccess)
            {
                //Don't know why, but ContentLength seems to be 0 sometimes, even though it is in the headers
                if (response.ContentLength > 0 || responseStream.CanSeek && responseStream.Length > 0)
                {
                    using (responseStream)
                        response.Error = ErrorHandler.Create(responseStream);

                    _logger.LogError("Received error: '{Message}'. Details: '{Details}'", response.Error.Message, response.Error.GetExtraData());
                }
            }

            //Only marshal successful responses
            if (response.IsSuccess)
                _marshaller.MarshalResponse(_options.Value, request, response, headers, responseStream);

            return response;
        }
    }
}