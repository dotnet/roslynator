// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1049SimplifyBooleanComparisonTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.SimplifyBooleanComparison;

        public override DiagnosticAnalyzer Analyzer { get; } = new BooleanLiteralAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new BinaryExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyBooleanComparison)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool x)
    {
        if (
#if DEBUG
            x &&
#endif
            [|x == false|])
        {
        }
    }
}
", @"
class C
{
    void M(bool x)
    {
        if (
#if DEBUG
            x &&
#endif
            !x)
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyBooleanComparison)]
        public async Task Test2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool x)
    {
        if (
#if RELEASE
            x &&
#endif
            [|x == false|])
        {
        }
    }
}
", @"
class C
{
    void M(bool x)
    {
        if (
#if RELEASE
            x &&
#endif
            !x)
        {
        }
    }
}
");
        }
    }
}
