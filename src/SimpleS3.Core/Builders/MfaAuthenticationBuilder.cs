using Genbox.HttpBuilders.Abstracts;

namespace Genbox.SimpleS3.Core.Builders
{
    public class MfaAuthenticationBuilder : IHttpHeaderBuilder
    {
        private string? _serialNumber;
        private string? _value;

        public string? Build()
        {
            if (!HasData())
                return null;

            return $"{_serialNumber} {_value}";
        }

        public void Reset()
        {
            _serialNumber = null;
            _value = null;
        }

        public bool HasData()
        {
            return _serialNumber != null;
        }

        public string? HeaderName => null;

        public void SetAuth(string serialNumber, string value)
        {
            _serialNumber = serialNumber;
            _value = value;
        }
    }
}