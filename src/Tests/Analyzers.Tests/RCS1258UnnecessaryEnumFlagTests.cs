// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1258UnnecessaryEnumFlagTests : AbstractCSharpDiagnosticVerifier<UnnecessaryEnumFlagAnalyzer, UnnecessaryEnumFlagCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UnnecessaryEnumFlag;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryEnumFlag)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Globalization;

class C
{
    void M()
    {
        var styles = NumberStyles.Integer | [|NumberStyles.AllowLeadingWhite|];
    }
}
", @"
using System.Globalization;

class C
{
    void M()
    {
        var styles = NumberStyles.Integer;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryEnumFlag)]
    public async Task Test2()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Globalization;

class C
{
    void M()
    {
        var styles = [|NumberStyles.AllowLeadingWhite|] | [|NumberStyles.AllowTrailingWhite|] | NumberStyles.Integer;
    }
}
", @"
using System.Globalization;

class C
{
    void M()
    {
        var styles = NumberStyles.Integer;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryEnumFlag)]
    public async Task TestNoDiagnostic()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Globalization;

class C
{
    void M()
    {
        var styles = NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowHexSpecifier | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowParentheses | NumberStyles.AllowThousands | NumberStyles.AllowTrailingSign | NumberStyles.AllowTrailingWhite;
    }
}
");
    }
}
