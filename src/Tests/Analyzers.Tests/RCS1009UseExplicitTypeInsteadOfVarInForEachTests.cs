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
    public static class RCS1009UseExplicitTypeInsteadOfVarInForEachTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseExplicitTypeInsteadOfVarInForEach;

        private static DiagnosticAnalyzer Analyzer { get; } = new UseExplicitTypeInForEachAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new UseExplicitTypeInsteadOfVarInForEachCodeFixProvider();

        [Fact]
        public static void TestDiagnosticWithCodeFix()
        {
            VerifyDiagnosticAndFix(@"
using System;
using System.Collections.Generic;

class C
{
    public void M()
    {
        var items = new List<DateTime>();

        foreach (<<<var>>> item in items)
        {
        }
    }
}
", @"
using System;
using System.Collections.Generic;

class C
{
    public void M()
    {
        var items = new List<DateTime>();

        foreach (DateTime item in items)
        {
        }
    }
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        internal static void TestNoDiagnostic()
        {
            VerifyNoDiagnostic(@"
using System;
using System.Collections.Generic;

class C
{
    public void M()
    {
        var items = new List<DateTime>();

        foreach (DateTime item in items)
        {
        }
    }
}", Descriptor, Analyzer);
        }
    }
}
