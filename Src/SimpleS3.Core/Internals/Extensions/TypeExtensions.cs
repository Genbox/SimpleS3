namespace Genbox.SimpleS3.Core.Internals.Extensions;

internal static class TypeExtensions
{
    internal static Type[] GetTypeArguments(this Type type)
    {
        Type[]? args = null;

        foreach (Type intType in type.GetInterfaces())
        {
            args = intType.GetGenericArguments();

            if (args.Length != 0)
                break;
        }

        if (args == null)
            throw new InvalidOperationException("Unable to find generic arguments");

        return args;
    }
}