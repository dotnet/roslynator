using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1172UseIsOperatorInsteadOfAsOperatorTests : AbstractCSharpDiagnosticVerifier<UseIsOperatorInsteadOfAsOperatorAnalyzer, UseIsOperatorInsteadOfAsOperatorCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseIsOperatorInsteadOfAsOperator;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseIsOperatorInsteadOfAsOperator)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
class MyClass
{
    void M(object item)
    {
        if ([|item as MyClass != null|])
        {
        }
    }
}
", @"
class MyClass
{
    void M(object item)
    {
        if (item is MyClass)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseIsOperatorInsteadOfAsOperator)]
    public async Task TestNoDiagnostic_OverloadedEqualityOperator()
    {
        await VerifyNoDiagnosticAsync(@"
internal class C
{
    public static implicit operator C(int i) => new C();
    public static bool operator ==(C left, C right) => default;
    public static bool operator !=(C left, C right) => default;
    public override bool Equals(object obj) => default;
    public override int GetHashCode() => default;

    void M(object x)
    {
        if (x as C != null)
        {
        }
    }
}
");
    }
}
