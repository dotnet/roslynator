// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1097RemoveRedundantToStringCallTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveRedundantToStringCall;

        public override DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new InvocationExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantToStringCall)]
        public async Task Test_StringVariable()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s1 = null;
        string s2 = s1[|.ToString()|];
    }
}
", @"
class C
{
    void M()
    {
        string s1 = null;
        string s2 = s1;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantToStringCall)]
        public async Task Test_StringLiteral()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = """"[|.ToString()|];
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantToStringCall)]
        public async Task Test_InterpolatedString()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = $""""[|.ToString()|];
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantToStringCall)]
        public async Task Test_Interpolation()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;
        s = $""{s[|.ToString()|]}"";
    }
}
", @"
class C
{
    void M()
    {
        string s = null;
        s = $""{s}"";
    }
}
");
        }
    }
}
