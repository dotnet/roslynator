#if ROSLYN_4_2
// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1262UnnecessaryRawStringLiteralTests : AbstractCSharpDiagnosticVerifier<UnnecessaryRawStringLiteralAnalyzer, UnnecessaryRawStringLiteralCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UnnecessaryRawStringLiteral;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryRawStringLiteral)]
    public async Task Test_SingleLineRawStringLiteral()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = ""[|""""|]foo"""""";
    }
}
", @"
class C
{
    void M()
    {
        string s = ""foo"";
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryRawStringLiteral)]
    public async Task Test_InterpolatedSingleLineRawString()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = """";
        string s1 = $""[|""""|] {s} foo """""";
    }
}
", @"
class C
{
    void M()
    {
        string s = """";
        string s1 = $"" {s} foo "";
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryRawStringLiteral)]
    public async Task Test_InterpolatedString_MultipleDollarSigns()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s1 = """";
        string s2 = """";
        string s3 = $$$""[|""""|] {{{s1}}} foo {{{s2}}} """""";
    }
}
", @"
class C
{
    void M()
    {
        string s1 = """";
        string s2 = """";
        string s3 = $"" {s1} foo {s2} "";
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryRawStringLiteral)]
    public async Task TestNoDiagnostic_ContainsQuote()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = """""" "" """""";
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryRawStringLiteral)]
    public async Task TestNoDiagnostic_ContainsEscape()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = """""" \t """""";
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryRawStringLiteral)]
    public async Task TestNoDiagnostic_InterpolatedString_ContainsQuote()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = $"""""" {""""} "" """""";
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryRawStringLiteral)]
    public async Task TestNoDiagnostic_InterpolatedString_ContainsEscape()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = $"""""" {""""} \t """""";
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryRawStringLiteral)]
    public async Task TestNoDiagnostic_MultipleDollarSigns()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = string.Empty;
        s = $$""""""{{s}}{s}"""""";
    }
}
");
    }
}
#endif
