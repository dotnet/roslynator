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
    public static class RCS12222MergePreprocessorDirectivesTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.MergePreprocessorDirectives;

        private static DiagnosticAnalyzer Analyzer { get; } = new MergePreprocessorDirectivesAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new DirectiveTriviaCodeFixProvider();

        [Fact]
        public static void TestDiagnosticWithFix_Disable()
        {
            VerifyDiagnosticAndFix(@"
class C
{
    void M()
    {
<<<#pragma warning disable RCS0>>>
#pragma warning disable RCS1, RCS2,
    
#pragma warning disable RCS3, RCS4, RCS5
#pragma warning restore RCS0
    }
}
", @"
class C
{
    void M()
    {
#pragma warning disable RCS0, RCS1, RCS2, RCS3, RCS4, RCS5
#pragma warning restore RCS0
    }
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithFix_Restore()
        {
            VerifyDiagnosticAndFix(@"
class C
{
    void M()
    {
<<<#pragma warning restore RCS0>>>
#pragma warning restore RCS1, RCS2,
    
#pragma warning restore RCS3, RCS4, RCS5
#pragma warning disable RCS0
    }
}
", @"
class C
{
    void M()
    {
#pragma warning restore RCS0, RCS1, RCS2, RCS3, RCS4, RCS5
#pragma warning disable RCS0
    }
}
", Descriptor, Analyzer, CodeFixProvider);
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
#pragma warning disable RCS0
    }
#pragma warning disable RCS0
#pragma warning restore RCS0
}
", Descriptor, Analyzer);
        }
    }
}
