// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests;

public class RCS9002UsePropertySyntaxNodeSpanStartTests : AbstractCSharpDiagnosticVerifier<SimpleMemberAccessExpressionAnalyzer, SimpleMemberAccessExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = CodeAnalysisDiagnosticRules.UsePropertySyntaxNodeSpanStart;

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UsePropertySyntaxNodeSpanStart)]
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
