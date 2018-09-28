// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1128UseCoalesceExpressionTests2 : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseCoalesceExpression;

        public override DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UseCoalesceExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseCoalesceExpression)]
        public async Task Test_GetValueOrDefault()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        int? x = 0;
        int y = 1;
        int z = x.[|GetValueOrDefault|](y);
    }
}
", @"
class C
{
    void M()
    {
        int? x = 0;
        int y = 1;
        int z = x ?? y;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseCoalesceExpression)]
        public async Task Test_GetValueOrDefault_ConditionalAccess()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var x = new C();

        int? i = x.P1?.P2.[|GetValueOrDefault|](0);
    }

    public C P1 { get; }

    public int? P2 { get; }
}
", @"
class C
{
    void M()
    {
        var x = new C();

        int? i = x.P1?.P2 ?? 0;
    }

    public C P1 { get; }

    public int? P2 { get; }
}
");
        }
    }
}
