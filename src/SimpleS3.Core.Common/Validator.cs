using System;
using System.Collections;

namespace Genbox.SimpleS3.Core.Common
{
    public static class Validator
    {
        public static void RequireValidEnum<T>(string value, string parameterName, out T enumValue, string? message = null) where T : struct, Enum
        {
            RequireNotNull(value, parameterName, message);
            RequireThat(Enum.TryParse(value, true, out enumValue), () => new ArgumentException(parameterName, message));
        }

        public static void RequireValidEnum<T>(T value, string parameterName, string? message = null) where T : struct, Enum
        {
            RequireThat(Enum.IsDefined(typeof(T), value), () => new ArgumentException(parameterName, message));
        }

        public static void RequireNotNull<T>(T? value, string parameterName, string? message = null) where T : class
        {
            RequireThat(value != null, () => new ArgumentNullException(parameterName, message));
        }

        public static void RequireNotNullOrEmpty(ICollection value, string parameterName, string? message = null)
        {
            RequireNotNull(value, parameterName, message);
            RequireThat(value.Count > 0, () => new ArgumentNullException(parameterName, message));
        }

        public static void RequireNotNullOrEmpty(string? value, string parameterName, string? message = null)
        {
            RequireThat(!string.IsNullOrEmpty(value), () => new ArgumentNullException(parameterName, message));
        }

        public static void RequireNotNullOrWhiteSpace(string value, string parameterName, string? message = null)
        {
            RequireThat(!string.IsNullOrWhiteSpace(value), () => new ArgumentNullException(parameterName, message));
        }

        public static void RequireThat(bool condition, string parameterName, string? message = null)
        {
            if (!condition)
                throw new ArgumentException(message, parameterName);
        }
        
        public static void RequireThat<T>(bool condition, Func<T>? func = null) where T : Exception, new()
        {
            if (condition)
                return;

            if (func != null)
                throw func();

            throw new T();
        }
    }
}