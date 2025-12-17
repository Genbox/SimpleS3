using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.GenericS3.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ValidationTests
{
    [Fact]
    public void EnsureValidationIsRun()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<INetworkDriver, NullNetworkDriver>();
        services.AddSingleton<IInputValidator, LocalValidator>();

        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(services);

        builder.UseGenericS3(config =>
        {
            config.Endpoint = "https://localhost";
            config.RegionCode = "us-west-2";
            config.NamingMode = NamingMode.PathStyle;
            config.Credentials = new StringAccessKey("test", "secret");
        });

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        OptionsValidationException ex = Assert.Throws<OptionsValidationException>(() => serviceProvider.GetRequiredService<ISimpleClient>());
        Assert.Equal("Invalid key id: The input was not the correct length. Length should be '20'", ex.Message);
    }

    private sealed class LocalValidator : InputValidatorBase
    {
        protected override bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status, out string? message)
        {
            if (keyId.Length != 20)
            {
                status = ValidationStatus.WrongLength;
                message = "20";
                return false;
            }

            status = ValidationStatus.Ok;
            message = null;
            return true;
        }

        protected override bool TryValidateAccessKeyInternal(byte[] accessKey, out ValidationStatus status, out string? message)
        {
            status = ValidationStatus.Ok;
            message = null;
            return true;
        }

        protected override bool TryValidateBucketNameInternal(string bucketName, BucketNameValidationMode mode, out ValidationStatus status, out string? message)
        {
            status = ValidationStatus.Ok;
            message = null;
            return true;
        }

        protected override bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
        {
            status = ValidationStatus.Ok;
            message = null;
            return true;
        }
    }
}