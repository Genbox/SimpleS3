namespace Genbox.SimpleS3.Cli.Core.Helpers;

public static class LocalPathHelper
{
    public static string GetFileName(string path) => Path.GetFileName(path);

    public static string? GetDirectoryName(string path) => Path.GetFileName(Path.GetDirectoryName(Path.GetFullPath(path)));

    public static string Combine(params string[] components) => PathHelper.Combine('/', '\\', components);

    public static string GetRelativePath(string relativeTo, string path) => Path.GetRelativePath(relativeTo, path);
}