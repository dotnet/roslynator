// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests;

public class RCS9003UnnecessaryConditionalAccessTests : AbstractCSharpDiagnosticVerifier<UnnecessaryConditionalAccessAnalyzer, ConditionalAccessExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = CodeAnalysisDiagnosticRules.UnnecessaryConditionalAccess;

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIds.UnnecessaryConditionalAccess)]
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
