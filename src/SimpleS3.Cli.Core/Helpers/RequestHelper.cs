using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Network.Responses;

namespace Genbox.SimpleS3.Cli.Core.Helpers
{
    public static class RequestHelper
    {
        public static async Task<T> ExecuteRequestAsync<T>(IClient client, Func<IClient, Task<T>> func) where T : BaseResponse
        {
            Validator.RequireNotNull(func, nameof(func));

            T resp = await func(client).ConfigureAwait(false);

            if (resp.IsSuccess)
                return resp;

            StringBuilder sb = new StringBuilder();
            sb.Append("Request failed with error ").Append(resp.StatusCode).AppendLine();

            if (resp.Error != null)
            {
                sb.Append("Message: ").Append(resp.Error.Message).AppendLine();

                string extraData = resp.Error.GetErrorDetails();

                if (!string.IsNullOrWhiteSpace(extraData))
                    sb.Append("Details: ").Append(extraData).AppendLine();
            }

            throw new Exception(sb.ToString());
        }

        public static IAsyncEnumerable<T> ExecuteAsyncEnumerable<T>(IClient client, Func<IClient, IAsyncEnumerable<T>> func)
        {
            Validator.RequireNotNull(func, nameof(func));

            //IAsyncEnumerator<T> enumerator = func(client).GetAsyncEnumerator();

            return func(client);
            //bool next;

            //do
            //{
            //    next = await enumerator.MoveNextAsync().ConfigureAwait(false);
            //    yield return enumerator.Current;
            //} while (next);
        }
    }
}