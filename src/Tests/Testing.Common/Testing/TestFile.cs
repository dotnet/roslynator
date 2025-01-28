using System.Diagnostics;

namespace Roslynator.Testing;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class TestFile
{
    public TestFile(string source, string? expectedSource = null, string? directoryPath = null, string? name = null)
    {
        Source = source;
        ExpectedSource = expectedSource;

        FileSystemVerifier.VerifyDirectoryPath(directoryPath);
        DirectoryPath = directoryPath;

        FileSystemVerifier.VerifyFileName(name);
        Name = name;
    }

    public string Source { get; }

    public string? ExpectedSource { get; }

    /// <summary>
    /// Gets the relative directory path.
    /// </summary>
    public string? DirectoryPath { get; }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string? Name { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => Source;
}
