using System.Diagnostics;

namespace Roslynator.Testing;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class TestFile
{
    public TestFile(string source, string? expectedSource = null, string? path = null)
    {
        Source = source;
        ExpectedSource = expectedSource;
        Path = path;
    }

    public string Source { get; }

    public string? ExpectedSource { get; }

    public string? Path { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => Source;
}
