// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1058UseCompoundAssignmentTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseCompoundAssignment;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseCompoundAssignmentAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AssignmentExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseCompoundAssignment)]
        public async Task Test_Property()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|P = P + 1|];
    }

    int P { get; set; }
}
", @"
class C
{
    void M()
    {
        P += 1;
    }

    int P { get; set; }
}
");
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseCompoundAssignment)]
        [InlineData("i = i + 1", "i += 1")]
        [InlineData("i = (i + 1)", "i += 1")]
        [InlineData("i = i - 1", "i -= 1")]
        [InlineData("i = i * 1", "i *= 1")]
        [InlineData("i = i / 1", "i /= 1")]
        [InlineData("i = i % 1", "i %= 1")]
        [InlineData("i = i << 1", "i <<= 1")]
        [InlineData("i = i >> 1", "i >>= 1")]
        [InlineData("i = i | 1", "i |= 1")]
        [InlineData("i = i & 1", "i &= 1")]
        [InlineData("i = i ^ 1", "i ^= 1")]
        public async Task Test(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(int i)
    {
        [||];
    }
}
", fromData, toData);
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseCompoundAssignment)]
        public async Task TestNoDiagnostic_ObjectInitializer()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var x = new C() { P = P + 1 };
    }

    int P { get; set; }
}
");
        }
    }
}
