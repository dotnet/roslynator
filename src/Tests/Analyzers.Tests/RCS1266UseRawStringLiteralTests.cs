#if ROSLYN_4_2
// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1266UseRawStringLiteralTests : AbstractCSharpDiagnosticVerifier<UseRawStringLiteralAnalyzer, RawStringLiteralCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseRawStringLiteral;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseRawStringLiteral)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        string s = [|@"
 ""foo""
"|];
    }
}
""", """"
class C
{
    void M()
    {
        string s = """
 "foo"
""";
    }
}
"""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseRawStringLiteral)]
    public async Task Test2()
    {
        await VerifyDiagnosticAndFixAsync("""""""
class C
{
    void M()
    {
        string s = [|@"
 """"""foo""
"|];
    }
}
""""""", """"""
class C
{
    void M()
    {
        string s = """"
 """foo"
"""";
    }
}
"""""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseRawStringLiteral)]
    public async Task TestNoDiagnostic_NoQuotes()
    {
        await VerifyNoDiagnosticAsync("""""""
class C
{
    void M()
    {
        string s = @"
 foo
";
    }
}
""""""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseRawStringLiteral)]
    public async Task TestNoDiagnostic_LeadingTrivia()
    {
        await VerifyNoDiagnosticAsync("""""""
class C
{
    void M()
    {
        string s = @"    
 """"""foo""
";
    }
}
""""""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseRawStringLiteral)]
    public async Task TestNoDiagnostic_TrailingTrivia()
    {
        await VerifyNoDiagnosticAsync("""""""
class C
{
    void M()
    {
        string s = @"
 """"""foo""
    ";
    }
}
""""""");
    }
}
#endif
