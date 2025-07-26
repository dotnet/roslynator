using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1105UnnecessaryInterpolationTests : AbstractCSharpDiagnosticVerifier<UnnecessaryInterpolationAnalyzer, InterpolationCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UnnecessaryInterpolation;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task TestNoDiagnostic_RawStringLiteral()
    {
        await VerifyNoDiagnosticAsync(""""
class C
{
    void M()
    {
        var s = $"""a {"\n"} b""";
    }
}
"""");
    }
}
