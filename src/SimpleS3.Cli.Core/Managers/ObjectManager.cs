using System.Threading.Tasks;

namespace Genbox.SimpleS3.Cli.Core.Managers
{
    public static class ObjectManager
    {
        public static async Task MoveAsync(string source, string destination)
        {
            //using (Stream s = File.OpenRead(FileName))
            //{
            //    long length = s.Length;

            //    PutObjectResponse resp = await ExecuteRequestAsync(client => client.PutObjectAsync(BucketName, ObjectName, s)).ConfigureAwait(false);
            //}
        }

        public static async Task CopyAsync()
        {
            //GetObjectResponse resp = await ExecuteRequestAsync(client => client.GetObjectAsync(BucketName, ObjectName)).ConfigureAwait(false);

            //using (Stream s = resp.Content.AsStream())
            //{
            //    long length = s.Length;

            //    using (FileStream fs = File.OpenWrite(FileName))
            //        await s.CopyToAsync(fs).ConfigureAwait(false);
            //}
        }

        public static void Delete()
        {
            //    if (ResourceHelper.TryParseResource(Resource, out (string bucket, string resource, ResourceType resourceType) data))
            //    {
            //        if (data.resourceType == ResourceType.File)

            //    }
        }
    }
}