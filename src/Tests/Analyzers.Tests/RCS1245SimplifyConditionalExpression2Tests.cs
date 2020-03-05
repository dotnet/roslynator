// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1245SimplifyConditionalExpression2Tests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.SimplifyConditionalExpression2;

        public override DiagnosticAnalyzer Analyzer { get; } = new SimplifyConditionalExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ConditionalExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyConditionalExpression2)]
        public async Task Test3()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false;

        bool z = [|x ? false : y|];
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false;

        bool z = !x && y;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyConditionalExpression2)]
        public async Task Test4()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false;

        bool z = [|x ? y : true|];
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false;

        bool z = !x || y;
    }
}
");
        }
    }
}
