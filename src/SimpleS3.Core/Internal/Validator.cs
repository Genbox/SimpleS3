using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal
{
    internal static class Validator
    {
        [AssertionMethod]
        public static void RequireNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NoEnumeration]
            T value, [CanBeNull] string message = null) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(message);
        }

        [AssertionMethod]
        public static void RequireNotEmpty<T>(ICollection<T> value, [CanBeNull] string message = null)
        {
            RequireNotNull(value, message);
            RequireThat(value.Count > 0, message);
        }

        [AssertionMethod]
        public static void RequireNotEmpty([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
            string value, [CanBeNull] string message = null)
        {
            RequireThat(!string.IsNullOrEmpty(value), message);
        }

        [AssertionMethod]
        public static void RequireNotWhitespace([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
            string value, [CanBeNull] string message = null)
        {
            RequireThat(!string.IsNullOrWhiteSpace(value), message);
        }

        [AssertionMethod]
        public static void RequireThat([AssertionCondition(AssertionConditionType.IS_TRUE)]
            bool condition, [CanBeNull] string message = null)
        {
            if (!condition)
                throw new RequireException(message);
        }
    }
}