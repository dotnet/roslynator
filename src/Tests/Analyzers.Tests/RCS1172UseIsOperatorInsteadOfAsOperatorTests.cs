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
}
