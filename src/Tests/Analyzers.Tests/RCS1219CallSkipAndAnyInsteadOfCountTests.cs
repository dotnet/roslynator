// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1219CallSkipAndAnyInsteadOfCountTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.CallSkipAndAnyInsteadOfCount;

        public override DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new BinaryExpressionCodeFixProvider();

        [Theory]
        [InlineData("items.Count() > i", "items.Skip(i).Any()")]
        [InlineData("i < items.Count()", "items.Skip(i).Any()")]
        [InlineData("items.Count() >= i", "items.Skip(i - 1).Any()")]
        [InlineData("i <= items.Count()", "items.Skip(i - 1).Any()")]
        [InlineData("items.Count() <= i", "!items.Skip(i).Any()")]
        [InlineData("i >= items.Count()", "!items.Skip(i).Any()")]
        [InlineData("items.Count() < i", "!items.Skip(i - 1).Any()")]
        [InlineData("i > items.Count()", "!items.Skip(i - 1).Any()")]
        public async Task TestDiagnostic(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        int i = 0;
        IEnumerable<object> items = null;

        if ([||])
        {
        }
    }
}
", fromData, toData);
        }

        [Fact]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
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
");
        }
    }
}
