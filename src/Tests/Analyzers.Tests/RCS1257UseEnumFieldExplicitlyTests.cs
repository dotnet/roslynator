// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1257UseEnumFieldExplicitlyTests : AbstractCSharpDiagnosticVerifier<UseEnumFieldExplicitlyAnalyzer, CastExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseEnumFieldExplicitly;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEnumFieldExplicitly)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Text.RegularExpressions;

class C
{
    void M()
    {
        var x = [|(RegexOptions)1|];
    }
}
", @"
using System.Text.RegularExpressions;

class C
{
    void M()
    {
        var x = RegexOptions.IgnoreCase;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEnumFieldExplicitly)]
    public async Task Test_Flags()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Text.RegularExpressions;

class C
{
    void M()
    {
        var x = [|(RegexOptions)7|];
    }
}
", @"
using System.Text.RegularExpressions;

class C
{
    void M()
    {
        var x = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEnumFieldExplicitly)]
    public async Task TestNoDiagnostic_UndefinedValue()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Text.RegularExpressions;

class C
{
    void M()
    {
        var x = (Foo)17;
    }
}

[System.Flags]
enum Foo
{
    None = 0,
    A = 1,
    B = 2,
    C = 4,
    D = 8,
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEnumFieldExplicitly)]
    public async Task TestNoDiagnostic_FileAttributes()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var x = (System.IO.FileAttributes)0;
    }
}
");
    }
}
