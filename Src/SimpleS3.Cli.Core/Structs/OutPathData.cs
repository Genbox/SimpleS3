namespace Genbox.SimpleS3.Cli.Core.Structs;

public struct OutPathData
{
    public OutPathData(string fullPath, string relativeIdentifier)
    {
        FullPath = fullPath;
        RelativeIdentifier = relativeIdentifier;
    }

    public string FullPath { get; }
    public string RelativeIdentifier { get; }
}