// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1098AvoidNullLiteralExpressionOnLeftSideOfBinaryExpressionTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression;

        public override DiagnosticAnalyzer Analyzer { get; } = new AvoidNullLiteralOnLeftOfBinaryExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new BinaryExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression)]
        public async Task TestDiagnosticWithCodeFix()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(string s)
    {
        if ([|null|] == s)
        {
        }
    }
}
", @"
class C
{
    void M(string s)
    {
        if (s == null)
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M(string s)
    {
        if (null
        #region
            == s)
        {
        }
        #endregion
    }
}
");
        }
    }
}
