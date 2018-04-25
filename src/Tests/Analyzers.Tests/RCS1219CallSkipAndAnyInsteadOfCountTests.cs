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
    public static class RCS1219CallSkipAndAnyInsteadOfCountTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.CallSkipAndAnyInsteadOfCount;

        private static DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new BinaryExpressionCodeFixProvider();

        [Theory]
        [InlineData("items.Count() > i", "items.Skip(i).Any()")]
        [InlineData("i < items.Count()", "items.Skip(i).Any()")]
        [InlineData("items.Count() >= i", "items.Skip(i - 1).Any()")]
        [InlineData("i <= items.Count()", "items.Skip(i - 1).Any()")]
        [InlineData("items.Count() <= i", "!items.Skip(i).Any()")]
        [InlineData("i >= items.Count()", "!items.Skip(i).Any()")]
        [InlineData("items.Count() < i", "!items.Skip(i - 1).Any()")]
        [InlineData("i > items.Count()", "!items.Skip(i - 1).Any()")]
        public static void TestDiagnosticWithCodeFix(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        int i = 0;
        IEnumerable<object> items = null;

        if (<<<>>>)
        {
        }
    }
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestNoDiagnostic()
        {
            VerifyNoDiagnostic(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        int i = 0;
        IEnumerable<object> items = null;

        if (items.Count() == 0) { }
        if (items.Count() == 1) { }
        if (items.Count() == i) { }
        if (items.Count() != 0) { }
        if (items.Count() != 1) { }
        if (items.Count() != i) { }
        if (items.Count() > 0) { }
        if (items.Count() >= 1) { }
        if (items.Count() < 1) { }
        if (items.Count() <= 0) { }
        if (0 == items.Count()) { }
        if (1 == items.Count()) { }
        if (i == items.Count()) { }
        if (0 != items.Count()) { }
        if (1 != items.Count()) { }
        if (i != items.Count()) { }
        if (0 < items.Count()) { }
        if (1 <= items.Count()) { }
        if (1 > items.Count()) { }
        if (0 >= items.Count()) { }
    }
}
", Descriptor, Analyzer);
        }
    }
}
