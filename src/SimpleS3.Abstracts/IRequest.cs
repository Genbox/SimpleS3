using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Abstracts
{
    public interface IRequest
    {
        /// <summary>Date of the request</summary>
        DateTimeOffset Date { get; }

        /// <summary>Resource (if any) of the request</summary>
        string Resource { get; set; }

        /// <summary>The method to use when performing the request</summary>
        HttpMethod Method { get; }

        /// <summary>The bucket (if any) used in the request</summary>
        string BucketName { get; }

        /// <summary>Headers to apply to the request</summary>
        IReadOnlyDictionary<string, string> Headers { get; }

        /// <summary>Query parameters to apply to the request</summary>
        IReadOnlyDictionary<string, string> QueryParameters { get; }

        /// <summary>Adds a query parameter</summary>
        /// <param name="key">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        void AddQueryParameter(string key, string value);

        /// <summary>Adds a header</summary>
        /// <param name="key">Name of the header</param>
        /// <param name="value">Value of the header</param>
        void AddHeader(string key, string value);
    }
}