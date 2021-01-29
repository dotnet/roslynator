// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests
{
    public class RCS9003UnnecessaryConditionalAccessTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UnnecessaryConditionalAccess;

        protected override DiagnosticAnalyzer Analyzer { get; } = new UnnecessaryConditionalAccessAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ConditionalAccessExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryConditionalAccess)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class C
{
    void M()
    {
        SyntaxNode n = null;

        if(n[|?|].IsKind(SyntaxKind.None) == true) { }
    }
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class C
{
    void M()
    {
        SyntaxNode n = null;

        if(n.IsKind(SyntaxKind.None)) { }
    }
}
");
        }
    }
}
