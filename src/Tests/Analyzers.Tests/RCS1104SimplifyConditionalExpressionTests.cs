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
    public static class RCS1104SimplifyConditionalExpressionTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.SimplifyConditionalExpression;

        private static DiagnosticAnalyzer Analyzer { get; } = new SimplifyConditionalExpressionAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new ConditionalExpressionCodeFixProvider();

        [Theory]
        [InlineData("f ? true : false", "f")]
        [InlineData("!f ? false : true", "f")]
        [InlineData("((f)) ? ((true)) : ((false))", "f")]
        [InlineData("f ? false : true", "!f")]
        [InlineData("f == g ? false : true", "f != g")]
        [InlineData("f != g ? false : true", "f == g")]

        [InlineData(@"f
            ? true
            : false", "f")]

        [InlineData(@"<<<f //a
              /*b*/ ? /*c*/ true //d
                                 /*e*/ : /*f*/ false>>> /*g*/", @"f //a
              /*b*/  /*c*/  //d
                                 /*e*/  /*f*/  /*g*/")]
        public static void TestDiagnosticWithCodeFix(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
class C
{
    void M(bool f, bool g)
    {
        if (<<<>>>) { }
}
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Theory]
        [InlineData("f ? g : false", "f && g")]
        [InlineData(@"<<<f
            ? g
            : false>>> /**/", @"f
            && g /**/")]
        public static void TestDiagnosticWithCodeFix_LogicalAnd(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
class C
{
    void M(bool f, bool g)
    {
        if (<<<>>>) { }
    }
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Theory]
        [InlineData("f ? true : g", "f || g")]
        [InlineData(@"<<<f
            ? true
            : g>>> /**/", @"f
            || g /**/")]
        public static void TestDiagnosticWithCodeFix_LogicalOr(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
class C
{
    void M(bool f, bool g)
    {
        if (<<<>>>) { }
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
    void M(bool f, bool g, bool h)
    {
        if ((f) ? g : h) { }
        if ((f) ? false : g) { }
        if ((f) ? g : true) { }

        if ((f)
#if DEBUG
                ? true
            : false;
#else
                ? false
                : true) { }
#endif
    }
}
", Descriptor, Analyzer);
        }
    }
}
