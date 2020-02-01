// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1143SimplifyCoalesceExpressionTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.SimplifyCoalesceExpression;

        public override DiagnosticAnalyzer Analyzer { get; } = new SimplifyCoalesceExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new BinaryExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCoalesceExpression)]
        public async Task Test_DefaultOfNullableType()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        C x = null;

        int? y = x?.M2() [|?? default(int?)|];
    }

    int M2() => default;
}
", @"
class C
{
    void M()
    {
        C x = null;

        int? y = x?.M2();
    }

    int M2() => default;
}
");
        }
    }
}
