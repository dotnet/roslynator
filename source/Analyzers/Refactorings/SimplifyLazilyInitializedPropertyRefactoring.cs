// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLazilyInitializedPropertyRefactoring
    {
        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            Analyze(context, methodDeclaration, methodDeclaration.Body);
        }

        public static void AnalyzeGetAccessorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var accessor = (AccessorDeclarationSyntax)context.Node;

            Analyze(context, accessor, accessor.Body);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode node, BlockSyntax body)
        {
            if (body != null)
            {
                SyntaxList<StatementSyntax> statements = body.Statements;

                if (statements.Count == 2)
                {
                    var ifStatement = statements[0] as IfStatementSyntax;

                    if (ifStatement != null)
                    {
                        var returnStatement = statements[1] as ReturnStatementSyntax;

                        if (returnStatement != null
                            && CanRefactor(context, ifStatement, returnStatement))
                        {
                            TextSpan span = TextSpan.FromBounds(ifStatement.SpanStart, returnStatement.Span.End);

                            if (!body.ContainsDirectives(TextSpan.FromBounds(ifStatement.SpanStart, returnStatement.Span.End)))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.SimplifyLazilyInitializedProperty,
                                    Location.Create(node.SyntaxTree, span));
                            }
                        }
                    }
                }
            }
        }

        private static bool CanRefactor(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement, ReturnStatementSyntax returnStatement)
        {
            ExpressionSyntax condition = ifStatement.Condition;

            if (condition?.IsKind(SyntaxKind.EqualsExpression) == true)
            {
                var equalsExpression = (BinaryExpressionSyntax)condition;

                if (equalsExpression.Right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
                {
                    IdentifierNameSyntax identifierName = GetIdentifierName(equalsExpression.Left);

                    if (identifierName != null)
                    {
                        SemanticModel semanticModel = context.SemanticModel;
                        CancellationToken cancellationToken = context.CancellationToken;

                        var fieldSymbol = semanticModel.GetSymbol(identifierName, cancellationToken) as IFieldSymbol;

                        if (fieldSymbol != null)
                        {
                            string fieldName = identifierName.Identifier.ValueText;

                            StatementSyntax statement = ifStatement.GetSingleStatementOrDefault();

                            if (statement?.IsKind(SyntaxKind.ExpressionStatement) == true)
                            {
                                var expressionStatement = (ExpressionStatementSyntax)statement;

                                ExpressionSyntax expression = expressionStatement.Expression;

                                if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                                {
                                    var assignment = (AssignmentExpressionSyntax)expression;

                                    ExpressionSyntax right = assignment.Right;

                                    return right?.IsMissing == false
                                        && right.IsSingleLine()
                                        && IsBackingField(GetIdentifierName(assignment.Left), fieldName, fieldSymbol, semanticModel, cancellationToken)
                                        && IsBackingField(GetIdentifierName(returnStatement.Expression), fieldName, fieldSymbol, semanticModel, cancellationToken);
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsBackingField(
            IdentifierNameSyntax identifierName,
            string name,
            IFieldSymbol fieldSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return string.Equals(identifierName?.Identifier.ValueText, name, StringComparison.Ordinal)
                && semanticModel.GetSymbol(identifierName, cancellationToken)?.Equals(fieldSymbol) == true;
        }

        private static IdentifierNameSyntax GetIdentifierName(ExpressionSyntax expression)
        {
            if (expression != null)
            {
                SyntaxKind kind = expression.Kind();

                if (kind == SyntaxKind.IdentifierName)
                {
                    return (IdentifierNameSyntax)expression;
                }
                else if (kind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)expression;

                    if (memberAccess.Expression?.IsKind(SyntaxKind.ThisExpression) == true)
                    {
                        SimpleNameSyntax name = memberAccess.Name;

                        if (name?.IsKind(SyntaxKind.IdentifierName) == true)
                        {
                            return (IdentifierNameSyntax)name;
                        }
                    }
                }
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            SyntaxList<StatementSyntax> statements = block.Statements;

            var ifStatement = (IfStatementSyntax)statements[0];

            var returnStatement = (ReturnStatementSyntax)statements[1];

            var expressionStatement = (ExpressionStatementSyntax)ifStatement.GetSingleStatementOrDefault();

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            ExpressionSyntax expression = returnStatement.Expression.WithoutTrivia();

            BinaryExpressionSyntax coalesceExpression = CoalesceExpression(
                expression,
                SimpleAssignmentExpression(expression, assignment.Right.WithoutTrivia()).Parenthesize());

            ReturnStatementSyntax newReturnStatement = returnStatement
                .WithExpression(coalesceExpression)
                .WithLeadingTrivia(ifStatement.GetLeadingTrivia());

            SyntaxList<StatementSyntax> newStatements = statements
                .Replace(returnStatement, newReturnStatement)
                .RemoveAt(0);

            BlockSyntax newBlock = block.WithStatements(newStatements);

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }
    }
}
