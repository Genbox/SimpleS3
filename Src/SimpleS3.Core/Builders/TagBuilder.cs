using System.Collections;
using System.Globalization;
using Genbox.HttpBuilders.Abstracts;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Core.Builders;

/// <summary>Provides the <c>TagBuilder</c> API.</summary>
public class TagBuilder : IHttpHeaderBuilder, IEnumerable<KeyValuePair<string, string>>
{
    private const string AllowedSymbols = "+-=._:/@";
    private readonly int _maxTags;
    private Dictionary<string, string>? _tags;

    /// <summary>Creates a builder with a resource-specific tag limit.</summary>
    /// <param name="maxTags">Maximum number of tags accepted by the target S3 resource.</param>
    public TagBuilder(int maxTags)
    {
        if (maxTags is <= 0 or > 50)
            throw new ArgumentOutOfRangeException(nameof(maxTags));

        _maxTags = maxTags;
    }

    /// <summary>Provides the <c>GetEnumerator</c> API.</summary>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        if (_tags == null)
            return Enumerable.Empty<KeyValuePair<string, string>>().GetEnumerator();

        return _tags.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Provides the <c>Build</c> API.</summary>
    public string? Build()
    {
        if (!HasData())
            return null;

        return string.Join("&", _tags!.Select(x => UrlHelper.UrlEncode(x.Key) + '=' + UrlHelper.UrlEncode(x.Value)));
    }

    /// <summary>Clears the tags for reuse.</summary>
    public void Reset() => _tags?.Clear();

    /// <summary>Provides the <c>HasData</c> API.</summary>
    public bool HasData() => _tags != null && _tags.Count > 0;

    /// <summary>Provides the <c>HeaderName</c> API.</summary>
    public string? HeaderName => null;

    // S3 user-defined tag rules:
    // https://docs.aws.amazon.com/AmazonS3/latest/userguide/tagging.html#how-tags-work
    // - Keys contain 1-128 Unicode characters; values contain 0-256; both are case-sensitive.
    // - A resource supports up to 50 tags, except objects, which support up to 10.
    // - Keys must be unique within a tag set; tags can be assigned when a resource is created.
    // - Letters, numbers, whitespace, and + - = . _ : / @ are allowed.
    // S3 object tags are represented in UTF-16; string.Length counts those positions:
    // https://docs.aws.amazon.com/AmazonS3/latest/userguide/object-tagging.html
    // Cost-allocation rules: aws: is reserved for AWS-generated tags; user-defined keys
    // appear with the user: prefix in billing reports. Each key is used once per resource:
    // https://docs.aws.amazon.com/awsaccountbilling/latest/aboutv2/custom-tags.html#allocation-tag-restrictions
    // The active cost-allocation tag key quota is 500 per payer account (adjustable):
    // https://docs.aws.amazon.com/awsaccountbilling/latest/aboutv2/billing-limits.html#limits-cat
    // Applying a tag cannot be backdated. Billing activation can be backfilled, but tag
    // values are only available for periods when the resource was actually tagged:
    // https://docs.aws.amazon.com/awsaccountbilling/latest/aboutv2/cost-allocation-backfill.html

    /// <summary>Provides the <c>Add</c> API.</summary>
    public TagBuilder Add(string key, string value)
    {
        Validator.RequireNotNull(key);
        Validator.RequireNotNull(value);
        _tags ??= new Dictionary<string, string>(StringComparer.Ordinal);

        if (_tags.Count == _maxTags)
            throw new InvalidOperationException($"Only {_maxTags} tags are allowed.");

        if (key.Length is 0 or > 128)
            throw new ArgumentException("Keys must contain between 1 and 128 characters", nameof(key));

        if (key.StartsWith("aws:", StringComparison.Ordinal))
            throw new ArgumentException("The aws: prefix is reserved for AWS-generated tags.", nameof(key));

        if (!IsValidTagText(key))
            throw new ArgumentException("Keys must contain only Unicode letters, numbers, whitespace, or + - = . _ : / @", nameof(key));

        if (value.Length > 256)
            throw new ArgumentException("Values can only be up to 256 characters", nameof(value));

        if (!IsValidTagText(value))
            throw new ArgumentException("Values must contain only Unicode letters, numbers, whitespace, or + - = . _ : / @", nameof(value));

        _tags.Add(key, value);

        return this;
    }

    // .NET Regex tests Unicode categories on UTF-16 code units, so \p{L} rejects letters
    // represented by surrogate pairs. CharUnicodeInfo recognizes the complete pair.
    private static bool IsValidTagText(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            char current = text[i];
            if (char.IsHighSurrogate(current))
            {
                if (i + 1 >= text.Length || !char.IsLowSurrogate(text[i + 1]))
                    return false;
            }
            else if (char.IsLowSurrogate(current))
                return false;

            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(text, i);
            if (category is UnicodeCategory.UppercaseLetter or UnicodeCategory.LowercaseLetter or UnicodeCategory.TitlecaseLetter or UnicodeCategory.ModifierLetter or UnicodeCategory.OtherLetter or
                UnicodeCategory.DecimalDigitNumber or UnicodeCategory.LetterNumber or UnicodeCategory.OtherNumber || char.IsWhiteSpace(current) || AllowedSymbols.IndexOf(current) >= 0)
            {
                if (char.IsHighSurrogate(current))
                    i++;
                continue;
            }

            return false;
        }

        return true;
    }
}