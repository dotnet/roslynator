using System;
using System.IO;
using System.Linq;

namespace Roslynator.Testing;

internal static class FilePathVerifier
{
    private static readonly char[] _invalidPathChars = Path.GetInvalidPathChars();
    private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

    public static void VerifyFilePath(string? path)
    {
        if (path is null)
            return;

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("File path cannot be empty or contain only whitespace", nameof(path));

        if (path.IndexOfAny(_invalidPathChars) >= 0)
            throw new ArgumentException("File path contains invalid character(s)", nameof(path));

        if (path[path.Length - 1] == Path.DirectorySeparatorChar
            || path[path.Length - 1] == Path.AltDirectorySeparatorChar)
        {
            throw new ArgumentException("File path cannot end with directory separator", nameof(path));
        }

        var dirSeparatorIndex = FileSystemHelpers.LastIndexOfDirectorySeparator(path);
        if (dirSeparatorIndex >= 0)
        {
            for (int i = dirSeparatorIndex + 1; i < path.Length; i++)
            {
                if (_invalidFileNameChars.Contains(path[i]))
                    throw new ArgumentException("File name contains invalid character(s)", nameof(path));
            }
        }
    }
}
