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
    public static class RCS1083CallAnyInsteadOfCountTests
    {
        public static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.CallAnyInsteadOfCount;

        public static DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public static CodeFixProvider CodeFixProvider { get; } = new BinaryExpressionCodeFixProvider();

        [Theory]
        [InlineData("items.Count() != 0", "items.Any()")]
        [InlineData("items.Count() > 0", "items.Any()")]
        [InlineData("items.Count() >= 1", "items.Any()")]
        [InlineData("0 != items.Count()", "items.Any()")]
        [InlineData("0 < items.Count()", "items.Any()")]
        [InlineData("1 <= items.Count()", "items.Any()")]
        [InlineData("items.Count() == 0", "!items.Any()")]
        [InlineData("items.Count() < 1", "!items.Any()")]
        [InlineData("items.Count() <= 0", "!items.Any()")]
        [InlineData("0 == items.Count()", "!items.Any()")]
        [InlineData("1 > items.Count()", "!items.Any()")]
        [InlineData("0 >= items.Count()", "!items.Any()")]
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

        if (items.Count() == 1) { }
        if (items.Count() == i) { }
        if (items.Count() != 1) { }
        if (items.Count() != i) { }
        if (items.Count() > i) { }
        if (items.Count() >= i) { }
        if (items.Count() <= i) { }
        if (items.Count() < i) { }
        if (1 == items.Count()) { }
        if (i == items.Count()) { }
        if (1 != items.Count()) { }
        if (i != items.Count()) { }
        if (i < items.Count()) { }
        if (i <= items.Count()) { }
        if (i >= items.Count()) { }
        if (i > items.Count()) { }
    }
}
", Descriptor, Analyzer);
        }
    }
}
