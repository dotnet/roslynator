// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PutExpressionBodyOnItsOwnLineAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.PutExpressionBodyOnItsOwnLine);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeArrowExpressionClause(f), SyntaxKind.ArrowExpressionClause);
    }

    private static void AnalyzeArrowExpressionClause(SyntaxNodeAnalysisContext context)
    {
        var arrowExpressionClause = (ArrowExpressionClauseSyntax)context.Node;

        switch (arrowExpressionClause.Parent.Kind())
        {
            case SyntaxKind.MethodDeclaration:
            case SyntaxKind.ConstructorDeclaration:
            case SyntaxKind.DestructorDeclaration:
            case SyntaxKind.PropertyDeclaration:
            case SyntaxKind.IndexerDeclaration:
            case SyntaxKind.OperatorDeclaration:
            case SyntaxKind.ConversionOperatorDeclaration:
                AnalyzeArrowExpressionClause(arrowExpressionClause.ArrowToken, context);
                break;
        }
    }

    private static void AnalyzeArrowExpressionClause(SyntaxToken arrowToken, SyntaxNodeAnalysisContext context)
    {
        NewLinePosition newLinePosition = context.GetArrowTokenNewLinePosition();

        SyntaxToken first;
        SyntaxToken second;
        if (newLinePosition == NewLinePosition.After)
        {
            first = arrowToken;
            second = arrowToken.GetNextToken();
        }
        else
        {
            first = arrowToken.GetPreviousToken();
            second = arrowToken;
        }

        TriviaBlock block = TriviaBlock.FromBetween(first, second);

        if (block.Kind == TriviaBlockKind.NoNewLine)
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.PutExpressionBodyOnItsOwnLine,
                block.GetLocation());
        }
    }
}
