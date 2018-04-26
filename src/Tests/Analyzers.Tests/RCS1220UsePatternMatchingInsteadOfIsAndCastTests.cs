// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Analysis.UsePatternMatching;
using Roslynator.CSharp.CodeFixes;
using Xunit;
using static Roslynator.Tests.CSharp.CSharpDiagnosticVerifier;

namespace Roslynator.Analyzers.Tests
{
    public static class RCS1220UsePatternMatchingInsteadOfIsAndCastTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UsePatternMatchingInsteadOfIsAndCast;

        private static DiagnosticAnalyzer Analyzer { get; } = new UsePatternMatchingInsteadOfIsAndCastAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new UsePatternMatchingInsteadOfIsAndCastCodeFixProvider();

        [Fact]
        public static void TestDiagnosticWithFix_LogicalAndExpression()
        {
            VerifyDiagnosticAndFix(
@"
class C
{
    private readonly object _f;

    public void M()
    {
        string s = null;

        object x = null;

        if (<<<x is string && ((string)x) == s>>>) { }

        if (<<<x is string && ((string)x).Equals((string)x)>>>) { }

        if (<<<_f is string && (string)(_f) == s>>>) { }

        if (<<<this._f is string && (string)this._f == s>>>) { }

        if (<<<_f is string && (string)(this._f) == s>>>) { }

        if (<<<this._f is string && (string)_f == s>>>) { }

        if (<<<this._f is string && ((string)_f).Equals((string)this._f)>>>) { }
    }
}
",
@"
class C
{
    private readonly object _f;

    public void M()
    {
        string s = null;

        object x = null;

        if (x is string x2 && (x2) == s) { }

        if (x is string x3 && (x3).Equals(x3)) { }

        if (_f is string x4 && x4 == s) { }

        if (this._f is string x5 && x5 == s) { }

        if (_f is string x6 && x6 == s) { }

        if (this._f is string x7 && x7 == s) { }

        if (this._f is string x8 && (x8).Equals(x8)) { }
    }
}
",
                descriptor: Descriptor,
                analyzer: Analyzer,
                fixProvider: CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithFix_IfStatement()
        {
            VerifyDiagnosticAndFix(
@"
class C
{
    private readonly object _f;

    public void M()
    {
        string s = null;

        object x = null;

        if (<<<x is string>>>)
        {
            if (((string)x) == s) { }
        }

        if (<<<x is string>>>)
        {
            if (((string)x).Equals((string)x)) { }
        }

        if (<<<_f is string>>>)
        {
            if ((string)_f == s) { }
        }

        if (<<<this._f is string>>>)
        {
            if ((string)this._f == s) { }
        }

        if (<<<_f is string>>>)
        {
            if ((string)this._f == s) { }
        }

        if (<<<this._f is string>>>)
        {
            if ((string)_f == s) { }
        }

        if (<<<this._f is string>>>)
        {
            if (((string)_f).Equals((string)this._f)) { }
        }
    }
}
",
@"
class C
{
    private readonly object _f;

    public void M()
    {
        string s = null;

        object x = null;

        if (x is string x2)
        {
            if ((x2) == s) { }
        }

        if (x is string x3)
        {
            if ((x3).Equals(x3)) { }
        }

        if (_f is string x4)
        {
            if (x4 == s) { }
        }

        if (this._f is string x5)
        {
            if (x5 == s) { }
        }

        if (_f is string x6)
        {
            if (x6 == s) { }
        }

        if (this._f is string x7)
        {
            if (x7 == s) { }
        }

        if (this._f is string x8)
        {
            if ((x8).Equals(x8)) { }
        }
    }
}
",
                descriptor: Descriptor,
                analyzer: Analyzer,
                fixProvider: CodeFixProvider);
        }

        [Fact]
        public static void TestNoDiagnostic_LogicalAndExpression()
        {
            VerifyNoDiagnostic(
@"
class C
{
    private readonly object _f;

    public void M()
    {
        string s = null;
        object x = null;
        object x2 = null;

        if (x is string && ((string)x) == x) { }

        if (x is string && ((string)x2) == s) { }

        if (x is string && x == s) { }
    }
}
",
                descriptor: Descriptor,
                analyzer: Analyzer);
        }

        [Fact]
        public static void TestNoDiagnostic_IfStatement()
        {
            VerifyNoDiagnostic(
@"
class C
{
    private readonly object _f;

    public void M()
    {
        string s = null;
        object x = null;
        object x2 = null;

        if (x is string)
        {
            if (((string)x) == x) { }
        }

        if (x is string)
        {
            if (((string)x2) == s) { }
        }

        if (x is string)
        {
            if (x == s) { }
        }
    }
}
",
                descriptor: Descriptor,
                analyzer: Analyzer);
        }
    }
}
