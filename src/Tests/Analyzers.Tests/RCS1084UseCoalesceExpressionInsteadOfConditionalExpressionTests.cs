// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.CodeFixes;
using Xunit;
using static Roslynator.Tests.CSharp.CSharpDiagnosticVerifier;

namespace Roslynator.Analyzers.Tests
{
    public static class RCS1084UseCoalesceExpressionInsteadOfConditionalExpressionTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseCoalesceExpressionInsteadOfConditionalExpression;

        private static DiagnosticAnalyzer Analyzer { get; } = new SimplifyNullCheckAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new ConditionalExpressionCodeFixProvider();

        [Theory]
        [InlineData("s != null ? s : \"\"", "s ?? \"\"")]
        [InlineData("s == null ? \"\" : s", "s ?? \"\"")]

        [InlineData("(s != null) ? (s) : (\"\")", "s ?? \"\"")]
        [InlineData("(s == null) ? (\"\") : (s)", "s ?? \"\"")]
        public static void TestDiagnosticWithCodeFix_ReferenceType(string fixableCode, string fixedCode)
        {
        VerifyDiagnosticAndFix(@"
class C
{
    void M()
    {
        string s = null;

        s = <<<>>>;
    }
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Theory]
        [InlineData("(ni != null) ? ni.Value : 1", "ni ?? 1")]
        [InlineData("(ni == null) ? 1 : ni.Value", "ni ?? 1")]
        [InlineData("(ni.HasValue) ? ni.Value : 1", "ni ?? 1")]
        [InlineData("(!ni.HasValue) ? 1 : ni.Value", "ni ?? 1")]
        public static void TestDiagnosticWithCodeFix_ValuType(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
class C
{
    void M()
    {
        int i = 0;
        int? ni = null;

        i = <<<>>>;
    }
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestNoDiagnostic()
        {
            VerifyNoDiagnostic(@"
class C
{
    public unsafe void M()
    {
        string s = """";

        s = (s != null) ? """" : s;
        s = (s == null) ? s : """";
    }
}
", Descriptor, Analyzer);
        }

        [Fact]
        public static void TestNoDiagnostic_Pointer()
        {
            VerifyNoDiagnostic(@"
class C
{
    public unsafe void M()
    {
        int* i = null;

        i = (i == null) ? default(int*) : i;
        i = (i != null) ? i : default(int*);
    }
}
", Descriptor, Analyzer);
        }
    }
}
