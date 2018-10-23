// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1214UnnecessaryInterpolatedStringTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UnnecessaryInterpolatedString;

        public override DiagnosticAnalyzer Analyzer { get; } = new UnnecessaryInterpolatedStringAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new InterpolatedStringCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryInterpolatedString)]
        public async Task Test_StringLiteral()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = [|$""{""""}""|];
    }
}
", @"
class C
{
    void M()
    {
        string s = """";
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryInterpolatedString)]
        public async Task Test_InterpolatedString()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = [|$""{$""""}""|];
    }
}
", @"
class C
{
    void M()
    {
        string s = $"""";
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryInterpolatedString)]
        public async Task Test_NonNulStringConstant()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        const string x = """";
        string s = [|$""{x}""|];
    }
}
", @"
class C
{
    void M()
    {
        const string x = """";
        string s = x;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryInterpolatedString)]
        public async Task TestNoDiagnosti()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        const string x = null;
        string s = $""{x}"";
    }
}
");
        }
    }
}
