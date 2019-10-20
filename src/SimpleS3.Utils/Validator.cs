using System;
using System.Collections;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Utils
{
    public static class Validator
    {
        [AssertionMethod]
        public static void RequireValidEnum<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]in string value, string parameterName, out T enumValue, string message = null) where T : struct, Enum
        {
            RequireNotNull(value, parameterName, message);
            RequireThat(Enum.TryParse(value, true, out enumValue), () => new ArgumentException(parameterName, message));
        }

        [AssertionMethod]
        public static void RequireValidEnum<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]in T value, string parameterName, string message = null) where T : struct, Enum
        {
            RequireThat(Enum.IsDefined(typeof(T), value), () => new ArgumentException(parameterName, message));
        }

        [AssertionMethod]
        public static void RequireNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]in T value, string parameterName, string message = null) where T : class
        {
            RequireThat(value != null, () => new ArgumentNullException(parameterName, message));
        }

        [AssertionMethod]
        public static void RequireNotNullOrEmpty(in ICollection value, string parameterName, string message = null)
        {
            RequireNotNull(value, message);
            RequireThat(value.Count > 0, () => new ArgumentNullException(parameterName, message));
        }

        [AssertionMethod]
        public static void RequireNotNullOrEmpty([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]in string value, string parameterName, string message = null)
        {
            RequireThat(!string.IsNullOrEmpty(value), () => new ArgumentNullException(parameterName, message));
        }

        [AssertionMethod]
        public static void RequireNotNullOrWhiteSpace([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]in string value, string parameterName, string message = null)
        {
            RequireThat(!string.IsNullOrWhiteSpace(value), () => new ArgumentNullException(parameterName, message));
        }

        [AssertionMethod]
        public static void RequireThat<T>([AssertionCondition(AssertionConditionType.IS_TRUE)]in bool condition, Func<T> func = null) where T : Exception, new()
        {
            if (condition)
                return;

            if (func != null)
                throw func();

            throw new T();
        }
    }
}