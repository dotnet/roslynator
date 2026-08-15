// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests;

public class RCS9012UseIsKindInsteadOfKindComparisonTests : AbstractCSharpDiagnosticVerifier<UseIsKindInsteadOfKindComparisonAnalyzer, UseIsKindInsteadOfKindComparisonCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = CodeAnalysisDiagnosticRules.UseIsKindInsteadOfKindComparison;

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseIsKindInsteadOfKindComparison)]
    public async Task Test_Equals()
    {
        await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node = null;

        if ([|node.Kind() == SyntaxKind.IdentifierName|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node = null;

        if (node.IsKind(SyntaxKind.IdentifierName)) { }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseIsKindInsteadOfKindComparison)]
    public async Task Test_NotEquals()
    {
        await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node = null;

        if ([|node.Kind() != SyntaxKind.IdentifierName|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node = null;

        if (!node.IsKind(SyntaxKind.IdentifierName)) { }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseIsKindInsteadOfKindComparison)]
    public async Task Test_Equals_KindOnRight()
    {
        await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node = null;

        if ([|SyntaxKind.IdentifierName == node.Kind()|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node = null;

        if (node.IsKind(SyntaxKind.IdentifierName)) { }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseIsKindInsteadOfKindComparison)]
    public async Task Test_ConditionalAccess()
    {
        await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node = null;

        if ([|node?.Kind() == SyntaxKind.IdentifierName|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node = null;

        if (node.IsKind(SyntaxKind.IdentifierName)) { }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseIsKindInsteadOfKindComparison)]
    public async Task Test_LogicalOr()
    {
        await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node = null;

        if ([|node.Kind() == SyntaxKind.IfStatement || node.Kind() == SyntaxKind.WhileStatement|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node = null;

        if (node.IsKind(SyntaxKind.IfStatement) || node.IsKind(SyntaxKind.WhileStatement)) { }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseIsKindInsteadOfKindComparison)]
    public async Task TestNoDiagnostic_IsKind()
    {
        await VerifyNoDiagnosticAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node = null;

        if (node.IsKind(SyntaxKind.IdentifierName)) { }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseIsKindInsteadOfKindComparison)]
    public async Task TestNoDiagnostic_KindComparedToKind()
    {
        await VerifyNoDiagnosticAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node1 = null;
        SyntaxNode node2 = null;

        if (node1.Kind() == node2.Kind()) { }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseIsKindInsteadOfKindComparison)]
    public async Task Test_LogicalOr_DifferentReceivers()
    {
        await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node1 = null;
        SyntaxNode node2 = null;

        if ([|node1.Kind() == SyntaxKind.IfStatement|] || [|node2.Kind() == SyntaxKind.WhileStatement|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class C
{
    void M()
    {
        SyntaxNode node1 = null;
        SyntaxNode node2 = null;

        if (node1.IsKind(SyntaxKind.IfStatement) || node2.IsKind(SyntaxKind.WhileStatement)) { }
    }
}
");
    }
}
