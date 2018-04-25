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
    public static class RCS1206UseConditionalAccessInsteadOfConditionalExpressionTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseConditionalAccessInsteadOfConditionalExpression;

        private static DiagnosticAnalyzer Analyzer { get; } = new SimplifyNullCheckAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new ConditionalExpressionCodeFixProvider();

        [Theory]
        [InlineData("(x != null) ? x.ToString() : null", "x?.ToString()")]
        [InlineData("(x != null) ? x.ToString() : default", "x?.ToString()")]
        [InlineData("(x != null) ? x.ToString() : default(string)", "x?.ToString()")]

        [InlineData("(x == null) ? null : x.ToString()", "x?.ToString()")]
        [InlineData("(x == null) ? default : x.ToString()", "x?.ToString()")]
        [InlineData("(x == null) ? default(string) : x.ToString()", "x?.ToString()")]
        public static void TestDiagnosticWithCodeFix_ReferenceTypeToReferenceType(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
class Foo
{
    void M()
    {
        var x = new Foo();

        string s = <<<>>>;
    }
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Theory]
        [InlineData("(x != null) ? x.Value : 0", "x?.Value ?? 0")]
        [InlineData("(x != null) ? x.Value : default", "x?.Value ?? (default)")]
        [InlineData("(x != null) ? x.Value : default(int)", "x?.Value ?? default(int)")]

        [InlineData("(x == null) ? 0 : x.Value", "x?.Value ?? 0")]
        [InlineData("(x == null) ? default : x.Value", "x?.Value ?? (default)")]
        [InlineData("(x == null) ? default(int) : x.Value", "x?.Value ?? default(int)")]
        public static void TestDiagnosticWithCodeFix_ReferenceTypeToValueType(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
class Foo
{
    void M()
    {
        var x = new Foo();

        int i = <<<>>>;
    }

    public int Value { get; }
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Theory]
        [InlineData("(x != null) ? x.Value : null", "x?.Value")]
        [InlineData("(x != null) ? x.Value : default", "x?.Value")]
        [InlineData("(x != null) ? x.Value : default(int?)", "x?.Value")]

        [InlineData("(x == null) ? null : x.Value", "x?.Value")]
        [InlineData("(x == null) ? default : x.Value", "x?.Value")]
        [InlineData("(x == null) ? default(int?) : x.Value", "x?.Value")]
        public static void TestDiagnosticWithCodeFix_ReferenceTypeToNullableType(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
class Foo
{
    void M()
    {
        Foo x = null;

        int? ni = <<<>>>;
    }

    public int? Value { get; }
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Theory]
        [InlineData("(ni != null) ? ni.Value.ToString() : null", "ni?.ToString()")]
        [InlineData("(ni == null) ? null : ni.Value.ToString()", "ni?.ToString()")]
        [InlineData("(ni.HasValue) ? ni.Value.ToString() : null", "ni?.ToString()")]
        [InlineData("(!ni.HasValue) ? null : ni.Value.ToString()", "ni?.ToString()")]
        public static void TestDiagnosticWithCodeFix_NullableTypeToReferenceType(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
class C
{
    void M()
    {
        int? ni = null;

        string s = <<<>>>;
    }
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Theory]
        [InlineData("(ni != null) ? ni.Value.GetHashCode() : 0", "ni?.GetHashCode() ?? 0")]
        [InlineData("(ni == null) ? 0 : ni.Value.GetHashCode()", "ni?.GetHashCode() ?? 0")]
        [InlineData("(ni.HasValue) ? ni.Value.GetHashCode() : 0", "ni?.GetHashCode() ?? 0")]
        [InlineData("(!ni.HasValue) ? 0 : ni.Value.GetHashCode()", "ni?.GetHashCode() ?? 0")]
        public static void TestDiagnosticWithCodeFix_NullableTypeToValueType(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
class C
{
    void M()
    {
        int? ni = null;

        int i = <<<>>>;
    }
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestNoDiagnostic()
        {
            VerifyNoDiagnostic(@"
class Foo
{
    void M()
    {
        var x = new Foo();
        var x2 = new Foo();

        string s = null;
        int i = 0;
        int? ni = null;
        int? ni2 = null;

        i = (x != null) ? x2.Value : default(int);
        i = (x == null) ? default(int) : x2.Value;

        i = (x != null) ? x.Value : 1;
        i = (x == null) ? 1 : x.Value;

        i = (ni != null) ? ni2.Value : default(int);
        i = (ni == null) ? default(int) : ni2.Value;

        i = (ni.HasValue) ? ni2.Value : default(int);
        i = (!ni.HasValue) ? default(int) : ni2.Value;

        i = (ni != null) ? ni.Value : 1;
        i = (ni == null) ? 1 : ni.Value;

        i = (ni.HasValue) ? ni.Value : 1;
        i = (!ni.HasValue) ? 1 : ni.Value;

        s = (i != null) ? i.ToString() : null;
        s = (i == null) ? null : i.ToString();
    }

    public int Value { get; }
}
", Descriptor, Analyzer);
        }
    }
}
