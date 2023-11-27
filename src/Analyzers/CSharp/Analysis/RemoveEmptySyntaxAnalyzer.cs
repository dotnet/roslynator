// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveEmptySyntaxAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.RemoveEmptySyntax);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeDestructorDeclaration(f), SyntaxKind.DestructorDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeElseClause(f), SyntaxKind.ElseClause);
        context.RegisterSyntaxNodeAction(f => AnalyzeFinallyClause(f), SyntaxKind.FinallyClause);
        context.RegisterSyntaxNodeAction(f => AnalyzeObjectCreationExpression(f), SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(f => AnalyzeNamespaceDeclaration(f), SyntaxKind.NamespaceDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeRegionDirective(f), SyntaxKind.RegionDirectiveTrivia);
        context.RegisterSyntaxNodeAction(f => AnalyzeEmptyStatement(f), SyntaxKind.EmptyStatement);
    }

    private static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
    {
        var destructor = (DestructorDeclarationSyntax)context.Node;

        if (!destructor.ContainsDiagnostics
            && !destructor.ContainsUnbalancedIfElseDirectives()
            && !destructor.AttributeLists.Any()
            && destructor.Body?.Statements.Count == 0)
        {
            ReportDiagnostic(context, destructor, "destructor");
        }
    }

    private static void AnalyzeElseClause(SyntaxNodeAnalysisContext context)
    {
        var elseClause = (ElseClauseSyntax)context.Node;

        StatementSyntax statement = elseClause.Statement;

        if (statement is not BlockSyntax block)
            return;

        if (block.Statements.Any())
            return;

        IfStatementSyntax topmostIf = elseClause.GetTopmostIf();

        if (topmostIf.Parent is IfStatementSyntax parentIf
            && parentIf.Else is not null)
        {
            return;
        }

        if (!elseClause.ElseKeyword.TrailingTrivia.IsEmptyOrWhitespace())
            return;

        if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(block.OpenBraceToken))
            return;

        if (!block.CloseBraceToken.LeadingTrivia.IsEmptyOrWhitespace())
            return;

        ReportDiagnostic(context, elseClause, "'else' clause");
    }

    private static void AnalyzeFinallyClause(SyntaxNodeAnalysisContext context)
    {
        var finallyClause = (FinallyClauseSyntax)context.Node;

        if (finallyClause.Parent is not TryStatementSyntax tryStatement)
            return;

        BlockSyntax finallyBlock = finallyClause.Block;

        if (finallyBlock?.Statements.Any() != false)
            return;

        if (!finallyClause.FinallyKeyword.TrailingTrivia.IsEmptyOrWhitespace())
            return;

        if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(finallyBlock.OpenBraceToken))
            return;

        if (!finallyBlock.CloseBraceToken.LeadingTrivia.IsEmptyOrWhitespace())
            return;

        if (tryStatement.Catches.Any())
        {
            ReportDiagnostic(context, finallyClause, "'finally' clause");
        }
        else
        {
            BlockSyntax tryBlock = tryStatement.Block;

            if (tryBlock?.Statements.Any() != true)
                return;

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(tryStatement.TryKeyword))
                return;

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(tryBlock.OpenBraceToken))
                return;

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(tryBlock.CloseBraceToken))
                return;

            if (!finallyClause.FinallyKeyword.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            ReportDiagnostic(context, finallyClause, "'finally' clause");
        }
    }

    private static void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
    {
        var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

        if (objectCreationExpression.ContainsDiagnostics)
            return;

        InitializerExpressionSyntax initializer = objectCreationExpression.Initializer;

        if (initializer?.Expressions.Any() != false)
            return;

        if (!initializer.OpenBraceToken.TrailingTrivia.IsEmptyOrWhitespace())
            return;

        if (!initializer.CloseBraceToken.LeadingTrivia.IsEmptyOrWhitespace())
            return;

        if (initializer.IsInExpressionTree(context.SemanticModel, context.CancellationToken))
            return;

        ReportDiagnostic(context, initializer, "initializer");
    }

    private static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
    {
        var declaration = (NamespaceDeclarationSyntax)context.Node;

        if (declaration.Members.Any())
            return;

        SyntaxToken openBrace = declaration.OpenBraceToken;
        SyntaxToken closeBrace = declaration.CloseBraceToken;

        if (openBrace.IsMissing)
            return;

        if (closeBrace.IsMissing)
            return;

        if (!openBrace.TrailingTrivia.IsEmptyOrWhitespace())
            return;

        if (!closeBrace.LeadingTrivia.IsEmptyOrWhitespace())
            return;

        ReportDiagnostic(context, declaration, "namespace declaration");
    }

    private static void AnalyzeRegionDirective(SyntaxNodeAnalysisContext context)
    {
        var regionDirective = (RegionDirectiveTriviaSyntax)context.Node;

        RegionInfo region = SyntaxInfo.RegionInfo(regionDirective);

        if (region.Success
            && region.IsEmpty)
        {
            ReportDiagnostic(context, regionDirective, "#region");
        }
    }

    private static void AnalyzeEmptyStatement(SyntaxNodeAnalysisContext context)
    {
        SyntaxNode emptyStatement = context.Node;

        if (emptyStatement.Parent?.IsKind(SyntaxKind.LabeledStatement) == false
            && !CSharpFacts.CanHaveEmbeddedStatement(emptyStatement.Parent.Kind()))
        {
            ReportDiagnostic(context, emptyStatement, "statement");
        }
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node, string name)
    {
        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.RemoveEmptySyntax, node, name);
    }
}
