// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AddBlankLineAfterUsingDirectiveListAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.AddBlankLineAfterUsingDirectiveList);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeCompilationUnit(f), SyntaxKind.CompilationUnit);
        context.RegisterSyntaxNodeAction(f => AnalyzeNamespaceDeclaration(f), SyntaxKind.NamespaceDeclaration);
    }

    private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
    {
        var compilationUnit = (CompilationUnitSyntax)context.Node;

        UsingDirectiveSyntax usingDirective = compilationUnit.Usings.LastOrDefault();

        if (usingDirective is null)
            return;

        SyntaxToken nextToken = compilationUnit.AttributeLists.FirstOrDefault()?.OpenBracketToken
            ?? compilationUnit.Members.FirstOrDefault()?.GetFirstToken()
            ?? default;

        if (nextToken.IsKind(SyntaxKind.None))
            return;

        Analyze(context, usingDirective, nextToken);
    }

    private static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
    {
        var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

        UsingDirectiveSyntax usingDirective = namespaceDeclaration.Usings.LastOrDefault();

        if (usingDirective is null)
            return;

        SyntaxToken nextToken = namespaceDeclaration.Members.FirstOrDefault()?.GetFirstToken() ?? default;

        if (nextToken.IsKind(SyntaxKind.None))
            return;

        Analyze(context, usingDirective, nextToken);
    }

    private static void Analyze(
        SyntaxNodeAnalysisContext context,
        UsingDirectiveSyntax usingDirective,
        SyntaxToken nextToken)
    {
        TriviaBlock block = TriviaBlock.FromBetween(usingDirective, nextToken);

        if (!block.Success)
            return;

        if (block.Kind == TriviaBlockKind.BlankLine)
            return;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.AddBlankLineAfterUsingDirectiveList,
            block.GetLocation());
    }
}
