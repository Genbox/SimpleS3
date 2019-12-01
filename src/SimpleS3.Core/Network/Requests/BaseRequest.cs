﻿using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests
{
    public abstract class BaseRequest : IRequest
    {
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _queryParameters = new Dictionary<string, string>();

        protected BaseRequest(HttpMethod method)
        {
            Method = method;
            RequestId = Guid.NewGuid();
            Date = DateTimeOffset.UtcNow;
        }

        public Func<bool> ForceContentMd5 { get; internal set; }
        public Guid RequestId { get; }
        public DateTimeOffset Date { get; internal set; }
        public HttpMethod Method { get; internal set; }
        public IReadOnlyDictionary<string, string> Headers => _headers;
        public IReadOnlyDictionary<string, string> QueryParameters => _queryParameters;

        public void AddQueryParameter(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            if (value == null)
                return;

            if (!_queryParameters.ContainsKey(key))
                _queryParameters.Add(key, value);
        }


        public void AddHeader(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            if (string.IsNullOrWhiteSpace(value))
                return;

            if (!_headers.ContainsKey(key))
                _headers.Add(key.ToLowerInvariant(), value);
        }
    }
}