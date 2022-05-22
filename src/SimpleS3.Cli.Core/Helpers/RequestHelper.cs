using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Network.Responses;

namespace Genbox.SimpleS3.Cli.Core.Helpers;

public static class RequestHelper
{
    public static async Task ExecuteRequestAsync(ISimpleClient client, Func<ISimpleClient, Task> func)
    {
        Validator.RequireNotNull(func, nameof(func));

        await func(client).ConfigureAwait(false);
    }

    public static async Task<T> ExecuteRequestAsync<T>(ISimpleClient client, Func<ISimpleClient, Task<T>> func) where T : BaseResponse
    {
        Validator.RequireNotNull(func, nameof(func));

        T resp = await func(client).ConfigureAwait(false);

        if (resp.IsSuccess)
            return resp;

        StringBuilder sb = StringBuilderPool.Shared.Rent();
        sb.Append("Request failed with error ").Append(resp.StatusCode).AppendLine();

        if (resp.Error != null)
        {
            sb.Append("Message: ").Append(resp.Error.Message).AppendLine();

            string extraData = resp.Error.GetErrorDetails();

            if (!string.IsNullOrWhiteSpace(extraData))
                sb.Append("Details: ").Append(extraData).AppendLine();
        }

        throw new S3Exception(StringBuilderPool.Shared.ReturnString(sb));
    }

    public static IAsyncEnumerable<T> ExecuteAsyncEnumerable<T>(ISimpleClient client, Func<ISimpleClient, IAsyncEnumerable<T>> func)
    {
        Validator.RequireNotNull(func, nameof(func));

        return func(client);
    }
}