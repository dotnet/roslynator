// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests
{
    public class RCS9002UsePropertySyntaxNodeSpanStartTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UsePropertySyntaxNodeSpanStart;

        protected override DiagnosticAnalyzer Analyzer { get; } = new SimpleMemberAccessExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SimpleMemberAccessExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePropertySyntaxNodeSpanStart)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxNode n = null;

        int start = [|n.Span.Start|];
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxNode n = null;

        int start = n.SpanStart;
    }
}
");
        }
    }
}
