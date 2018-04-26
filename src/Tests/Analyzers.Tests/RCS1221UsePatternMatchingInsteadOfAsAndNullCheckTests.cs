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
    public static class RCS1221UsePatternMatchingInsteadOfAsAndNullCheckTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UsePatternMatchingInsteadOfAsAndNullCheck;

        private static DiagnosticAnalyzer Analyzer { get; } = new UsePatternMatchingInsteadOfAsAndNullCheckAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new UsePatternMatchingInsteadOfAsAndNullCheckCodeFixProvider();

        [Fact]
        public static void TestDiagnosticWithFix_EqualsToNull()
        {
            VerifyDiagnosticAndFix(
@"
class C
{
    void M()
    {
        object x = null;

        <<<var s = x as string;>>>
        if (s == null)
        {
            return;
        }
    }
}
",
@"
class C
{
    void M()
    {
        object x = null;

        if (!(x is string s))
        {
            return;
        }
    }
}
",
                descriptor: Descriptor,
                analyzer: Analyzer,
                fixProvider: CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithFix_IsNull()
        {
            VerifyDiagnosticAndFix(
@"
class C
{
    void M()
    {
        object x = null;

        <<<var s = x as string;>>>
        if (s is null)
        {
            return;
        }
    }
}
",
@"
class C
{
    void M()
    {
        object x = null;

        if (!(x is string s))
        {
            return;
        }
    }
}
",
                descriptor: Descriptor,
                analyzer: Analyzer,
                fixProvider: CodeFixProvider);
        }

        [Fact]
        public static void TestNoDiagnostic()
        {
            VerifyNoDiagnostic(
@"
class C
{
    void M()
    {
        object x = null;

        string s = x as string, y = x as string;
        if (s == null)
        {
            return;
        }

        var s2 = x as string;
        if (s2 == null)
        {
            return;
        }
        else
        {
        }

        var s3 = x as string;
        if (s3 == null)
        {
            M();
        }

        var s4 = x as string;
        if (s4 != null)
        {
            return;
        }

        var s5 = x as string;
        if (s4 == null)
        {
            return;
        }
    }
}
",
                descriptor: Descriptor,
                analyzer: Analyzer);
        }

        [Fact]
        public static void TestNoDiagnostic_Directive()
        {
            VerifyNoDiagnostic(
@"
class C
{
    void M()
    {
        object x = null;

#region
        var s = x as string;
#endregion
        if (s == null)
        {
            return;
        }
    }
}
",
                descriptor: Descriptor,
                analyzer: Analyzer);
        }
    }
}
