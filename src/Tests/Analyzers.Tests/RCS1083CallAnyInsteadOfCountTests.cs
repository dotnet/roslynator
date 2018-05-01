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
    public class RCS1083CallAnyInsteadOfCountTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.CallAnyInsteadOfCount;

        public override DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new BinaryExpressionCodeFixProvider();

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
");
        }
    }
}
