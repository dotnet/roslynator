// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1107RemoveRedundantStringToCharArrayCallTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveRedundantStringToCharArrayCall;

        public override DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new InvocationExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantStringToCharArrayCall)]
        public async Task Test_ElementAccess()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;

        char ch = s[|.ToCharArray()|][0];
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        char ch = s[0];
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantStringToCharArrayCall)]
        public async Task Test_ForEach()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;

        foreach (char ch in s[|.ToCharArray()|])
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        foreach (char ch in s)
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantStringToCharArrayCall)]
        public async Task Test_ForEach_ParenthesizedExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;

        foreach (char ch in (s[|.ToCharArray()|]))
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        foreach (char ch in (s))
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantStringToCharArrayCall)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = null;

        var chars = s.ToCharArray();
    }
}
");
        }
    }
}
