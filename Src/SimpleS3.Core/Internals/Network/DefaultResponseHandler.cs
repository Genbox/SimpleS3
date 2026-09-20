using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Errors;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Network;

/// <summary>Provides the <c>DefaultResponseHandler</c> API.</summary>
public partial class DefaultResponseHandler : IResponseHandler
{
    private readonly SimpleS3Config _config;
    private readonly IMarshalFactory _marshaller;
    private readonly INetworkDriver _networkDriver;
    private readonly IPostMapperFactory _postMapper;

    /// <summary>Provides the <c>DefaultResponseHandler</c> API.</summary>
    public DefaultResponseHandler(IOptions<SimpleS3Config> options, IRequestValidatorFactory validator, IMarshalFactory marshaller, IPostMapperFactory postMapper, INetworkDriver networkDriver, ILogger<DefaultResponseHandler> logger)
    {
        Validator.RequireNotNull(options);
        Validator.RequireNotNull(validator);
        Validator.RequireNotNull(marshaller);
        Validator.RequireNotNull(networkDriver);
        Validator.RequireNotNull(logger);

        _networkDriver = networkDriver;
        _postMapper = postMapper;
        _logger = logger;
        _marshaller = marshaller;
        _config = options.Value;
    }

    /// <summary>Provides the <c>HandleResponseAsync&lt;TReq,TResp&gt;</c> API.</summary>
    public async Task<TResp> HandleResponseAsync<TReq, TResp>(TReq request, string url, Stream? requestStream, CancellationToken token) where TReq : IRequest where TResp : IResponse, new()
    {
        SendingRequest(url);

        HttpResponse httpResp;

        try
        {
            httpResp = await _networkDriver.SendRequestAsync<TResp>(request, url, requestStream, token);
        }
        finally
        {
            //Clear sensitive material from the request
            if (request is IContainSensitiveMaterial sensitive)
                sensitive.ClearSensitiveMaterial();
        }

        IDictionary<string, string> headers = httpResp.Headers;
        int statusCode = httpResp.StatusCode;

        ContentStream? responseStream = httpResp.Content;

        try
        {
            TResp response = new TResp();
            response.StatusCode = statusCode;
            response.ContentLength = headers.GetHeaderLong(HttpHeaders.ContentLength);
            response.ConnectionClosed = string.Equals("close", headers.GetOptionalValue(HttpHeaders.Connection), StringComparison.OrdinalIgnoreCase);
            response.Date = headers.GetHeaderDate(HttpHeaders.Date, DateTimeFormat.Rfc1123);
            response.Server = headers.GetOptionalValue(HttpHeaders.Server);
            response.ResponseId = headers.GetOptionalValue(AmzHeaders.XAmzId2);
            response.RequestId = headers.GetOptionalValue(AmzHeaders.XAmzRequestId);

            // S3-001: HTTP success is strictly the inclusive 200-299 range.
            response.IsSuccess = statusCode >= 200 && statusCode < 300;

            bool transfersContentOwnership = response.IsSuccess && response is IHasContent;

            //Only marshal successful responses
            if (response.IsSuccess)
                _marshaller.MarshalResponse(_config, response, headers, responseStream ?? ContentStream.Null);
            else if (responseStream != null)
            {
                try
                {
                    using MemoryStream ms = new MemoryStream();
                    await responseStream.CopyToAsync(ms, 81920, token);

                    if (ms.Length > 0)
                    {
                        ms.Seek(0, SeekOrigin.Begin);

                        using (XmlReader reader = XmlReader.Create(ms))
                            response.Error = ErrorHandler.Create(reader);

                        ReceivedError(response.Error.Message, response.Error.GetErrorDetails());
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    FailedToMapError(ex);
                }
            }

            //We always map even if the request is not successful
            _postMapper.PostMap(_config, request, response);

            if (_config.ThrowExceptionOnError && !response.IsSuccess)
                throw new S3RequestException(response, $"Received error: '{response.Error?.Message}'. Details: '{response.Error?.GetErrorDetails()}'");

            if (!transfersContentOwnership)
                responseStream?.Dispose();

            return response;
        }
        catch
        {
            try
            {
                responseStream?.Dispose();
            }
            catch
            {
                // Cleanup must not replace the exception that prevented delivery of the response.
            }

            throw;
        }
    }
}