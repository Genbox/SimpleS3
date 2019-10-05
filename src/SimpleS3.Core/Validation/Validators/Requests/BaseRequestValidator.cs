using FluentValidation;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Requests;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests
{
    public abstract class BaseRequestValidator<T> : ValidatorBase<T> where T : BaseRequest
    {
        protected BaseRequestValidator(IOptions<S3Config> config)
        {
            RuleFor(x => x.Method).IsInEnum().Must(x => x != HttpMethod.Unknown);

            //See https://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html
            //- Can contain multiple DNS labels, which must start with lowercase letter or digit
            //- Other than the start character, it must also contain hyphens
            //- Must be between 3 and 63 long
            RuleFor(x => x.BucketName)
                .Length(3, 64)
                .Must(BeValidDns)
                .When(x => config.Value.EnableBucketNameValidation)
                .WithMessage("Amazon recommends naming buckets to be valid DNS names, as you can't change the name later on. To turn off DNS name validation, set S3Config.EnableBucketNameValidation to false");
        }

        internal static bool BeValidDns(string name)
        {
            int curPos = 0;
            int end = name.Length;

            do
            {
                //find the dot or hit the end
                int newPos = curPos;
                while (newPos < end)
                {
                    if (name[newPos] == '.')
                        break;

                    ++newPos;
                }

                if (curPos == newPos || newPos - curPos > 63)
                    return false;

                char startChar = name[curPos];

                if (!IsInRange(startChar, 'a', 'z') && !IsInRange(startChar, '0', '9'))
                    return false;

                curPos++;

                //check the label content
                while (curPos < newPos)
                {
                    char character = name[curPos++];

                    if (IsInRange(character, 'a', 'z') || IsInRange(character, '0', '9') || character == '-')
                        continue;

                    return false;
                }

                ++curPos;
            } while (curPos < end);

            return true;
        }

        private static bool IsInRange(char c, char min, char max)
        {
            return unchecked((uint)(c - min) <= (uint)(max - min));
        }
    }
}