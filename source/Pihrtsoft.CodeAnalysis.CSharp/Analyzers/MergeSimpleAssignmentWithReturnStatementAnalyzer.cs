// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Analyzers
{
    internal static class MergeSimpleAssignmentWithReturnStatementAnalyzer
    {
        public static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var returnStatement = (ReturnStatementSyntax)context.Node;

            if (returnStatement.Expression?.IsKind(SyntaxKind.IdentifierName) != true
                || returnStatement.Parent?.IsKind(SyntaxKind.Block) != true
                || returnStatement.Parent.Parent?.IsAnyKind(
                    SyntaxKind.MethodDeclaration,
                    SyntaxKind.IndexerDeclaration,
                    SyntaxKind.SimpleLambdaExpression,
                    SyntaxKind.ParenthesizedLambdaExpression) != true)
            {
                return;
            }

            ExpressionStatementSyntax statement = GetExpressionStatement(returnStatement);

            if (statement == null)
                return;

            AssignmentExpressionSyntax assignment = GetSimpleAssignment(statement);

            if (assignment == null)
                return;

            ISymbol symbol = GetSymbol(returnStatement.Expression, context);

            if (symbol == null)
                return;

            ISymbol symbol2 = context.SemanticModel.GetSymbolInfo(assignment.Left, context.CancellationToken).Symbol;

            if (symbol.Equals(symbol2)
                && IsLocalSymbol(symbol, returnStatement, context))
            {
                TextSpan span = TextSpan.FromBounds(statement.Span.Start, returnStatement.Span.End);

                if (returnStatement.Parent
                    .DescendantTrivia(span, descendIntoTrivia: false)
                    .All(f => f.IsWhitespaceOrEndOfLine()))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.MergeSimpleAssignmentWithReturnStatement,
                        Location.Create(context.Node.SyntaxTree, span));
                }

                DiagnosticHelper.FadeOutNode(context, assignment.Left, DiagnosticDescriptors.MergeSimpleAssignmentWithReturnStatementFadeOut);
                DiagnosticHelper.FadeOutToken(context, assignment.OperatorToken, DiagnosticDescriptors.MergeSimpleAssignmentWithReturnStatementFadeOut);
                DiagnosticHelper.FadeOutToken(context, statement.SemicolonToken, DiagnosticDescriptors.MergeSimpleAssignmentWithReturnStatementFadeOut);
                DiagnosticHelper.FadeOutNode(context, returnStatement.Expression, DiagnosticDescriptors.MergeSimpleAssignmentWithReturnStatementFadeOut);
            }
        }

        private static bool IsLocalSymbol(ISymbol symbol, ReturnStatementSyntax returnStatement, SyntaxNodeAnalysisContext context)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Parameter:
                    return IsLocalParameterSymbol(symbol, returnStatement.Parent.Parent, context);
                case SymbolKind.Local:
                    return IsLocalDeclarationSymbol(symbol, (BlockSyntax)returnStatement.Parent, context);
            }

            return false;
        }

        private static ISymbol GetSymbol(ExpressionSyntax expression, SyntaxNodeAnalysisContext context)
        {
            ISymbol symbol = context.SemanticModel
                .GetSymbolInfo(expression, context.CancellationToken)
                .Symbol;

            switch (symbol?.Kind)
            {
                case SymbolKind.Parameter:
                    {
                        if (((IParameterSymbol)symbol).RefKind == RefKind.None)
                            return symbol;

                        break;
                    }
                case SymbolKind.Local:
                    {
                        return symbol;
                    }
            }

            return null;
        }

        private static ExpressionStatementSyntax GetExpressionStatement(ReturnStatementSyntax returnStatement)
        {
            var block = (BlockSyntax)returnStatement.Parent;

            if (block.Statements.Count > 1)
            {
                int index = block.Statements.IndexOf(returnStatement);

                if (index > 0)
                {
                    StatementSyntax statement = block.Statements[index - 1];

                    if (statement.IsKind(SyntaxKind.ExpressionStatement))
                        return (ExpressionStatementSyntax)statement;
                }
            }

            return null;
        }

        private static AssignmentExpressionSyntax GetSimpleAssignment(ExpressionStatementSyntax expressionStatement)
        {
            if (expressionStatement.Expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
            {
                var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                if (assignment.Left?.IsKind(SyntaxKind.IdentifierName) == true
                    && assignment.Right?.IsMissing == false)
                {
                    return assignment;
                }
            }

            return null;
        }

        private static bool IsLocalParameterSymbol(ISymbol symbol, SyntaxNode node, SyntaxNodeAnalysisContext context)
        {
            if (node.IsKind(SyntaxKind.SimpleLambdaExpression))
            {
                ParameterSyntax parameter = ((SimpleLambdaExpressionSyntax)node).Parameter;
                if (parameter != null)
                {
                    ISymbol symbol2 = context.SemanticModel
                        .GetDeclaredSymbol(parameter, context.CancellationToken);

                    return symbol.Equals(symbol2);
                }
            }
            else
            {
                BaseParameterListSyntax parameterList = node.GetParameterList();
                if (parameterList != null)
                {
                    foreach (ParameterSyntax parameter in parameterList.Parameters)
                    {
                        ISymbol symbol2 = context.SemanticModel
                            .GetDeclaredSymbol(parameter, context.CancellationToken);

                        if (symbol.Equals(symbol2))
                            return true;
                    }
                }
            }

            return false;
        }

        private static bool IsLocalDeclarationSymbol(ISymbol symbol, BlockSyntax block, SyntaxNodeAnalysisContext context)
        {
            foreach (StatementSyntax statement in block.Statements)
            {
                if (statement.IsKind(SyntaxKind.LocalDeclarationStatement))
                {
                    var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                    if (localDeclaration.Declaration != null)
                    {
                        foreach (VariableDeclaratorSyntax declarator in localDeclaration.Declaration.Variables)
                        {
                            ISymbol symbol2 = context.SemanticModel
                                .GetDeclaredSymbol(declarator, context.CancellationToken);

                            if (symbol.Equals(symbol2))
                                return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
