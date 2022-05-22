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
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(func);

        await func(client).ConfigureAwait(false);
    }

    public static async Task<T> ExecuteRequestAsync<T>(ISimpleClient client, Func<ISimpleClient, Task<T>> func) where T : BaseResponse
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(func);

        T resp = await func(client).ConfigureAwait(false);

        if (resp.IsSuccess)
            return resp;

        StringBuilder sb = StringBuilderPool.Shared.Rent();
        sb.Append("Request failed with error ").AppendLine(resp.StatusCode.ToString());

        if (resp.Error != null)
        {
            sb.Append("Message: ").AppendLine(resp.Error.Message);

            string extraData = resp.Error.GetErrorDetails();

            if (!string.IsNullOrWhiteSpace(extraData))
                sb.Append("Details: ").AppendLine(extraData);
        }

        throw new S3Exception(StringBuilderPool.Shared.ReturnString(sb));
    }

    public static IAsyncEnumerable<T> ExecuteAsyncEnumerable<T>(ISimpleClient client, Func<ISimpleClient, IAsyncEnumerable<T>> func)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(func);

        return func(client);
    }
}