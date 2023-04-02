using System.IO.Enumeration;

namespace Genbox.SimpleS3.Cli.Core.Helpers;

public static class FileHelper
{
    public static bool TryDeleteFile(string file)
    {
        try
        {
            File.Delete(file);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async IAsyncEnumerable<string> GetFilesAsync(string path)
    {
        FileSystemEnumerable<string> enu = new FileSystemEnumerable<string>(path, (ref FileSystemEntry entry) => entry.ToSpecifiedFullPath(),
            new EnumerationOptions
            {
                AttributesToSkip = FileAttributes.Offline | FileAttributes.ReparsePoint,
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            })
        {
            ShouldIncludePredicate = (ref FileSystemEntry entry) => !entry.IsDirectory
        };

        foreach (string file in enu)
            yield return file;
    }
}