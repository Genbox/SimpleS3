using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Internals.Pools;

namespace Genbox.SimpleS3.Core.Network.Requests
{
    public abstract class BaseRequest : IRequest, IPooledObject
    {
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _queryParameters = new Dictionary<string, string>();

        protected BaseRequest(HttpMethod method)
        {
            Method = method;
        }

        public Guid RequestId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public HttpMethod Method { get; internal set; }
        public IReadOnlyDictionary<string, string> Headers => _headers;
        public IReadOnlyDictionary<string, string> QueryParameters => _queryParameters;

        public void SetQueryParameter(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            if (value == null)
                return;

            _queryParameters[key] = value;
        }

        public void SetHeader(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            if (string.IsNullOrWhiteSpace(value))
                return;

            _headers[key.ToLowerInvariant()] = value;
        }

        public virtual void Reset()
        {
            _headers.Clear();
            _queryParameters.Clear();
        }
    }
}