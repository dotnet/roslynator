using System;
using System.IO;
using System.Linq;

namespace Roslynator.Testing;

internal static class FileSystemVerifier
{
    private static readonly char[] _invalidPathChars = Path.GetInvalidPathChars();
    private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

    public static void VerifyDirectoryPath(string? path)
    {
        if (path is null)
            return;

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Directory path cannot be empty or contain only whitespace", nameof(path));

        if (path.IndexOfAny(_invalidPathChars) >= 0)
            throw new ArgumentException("Directory path contains invalid character(s)", nameof(path));

        if (path[path.Length - 1] == Path.DirectorySeparatorChar
            || path[path.Length - 1] == Path.AltDirectorySeparatorChar)
        {
            throw new ArgumentException("Directory path cannot end with a directory separator", nameof(path));
        }
    }

    public static void VerifyFileName(string? path)
    {
        if (path is null)
            return;

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("File name cannot be empty or contain only whitespace", nameof(path));

        if (path.IndexOfAny(_invalidFileNameChars) >= 0)
            throw new ArgumentException("File name contains invalid character(s)", nameof(path));
    }
}
