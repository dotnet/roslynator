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
using Roslynator.CSharp.Syntax;

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
            if (body?.ContainsDiagnostics == false)
            {
                SyntaxList<StatementSyntax> statements = body.Statements;

                if (statements.Count == 2
                    && statements[0].IsKind(SyntaxKind.IfStatement)
                    && statements[1].IsKind(SyntaxKind.ReturnStatement))
                {
                    var ifStatement = (IfStatementSyntax)statements[0];
                    var returnStatement = (ReturnStatementSyntax)statements[1];

                    if (CanRefactor(context, ifStatement, returnStatement))
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

        private static bool CanRefactor(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement, ReturnStatementSyntax returnStatement)
        {
            SimpleIfStatementInfo simpleIf = SyntaxInfo.SimpleIfStatementInfo(ifStatement);

            if (simpleIf.Success)
            {
                StatementSyntax statement = simpleIf.Statement.SingleNonBlockStatementOrDefault();

                if (statement != null)
                {
                    SemanticModel semanticModel = context.SemanticModel;
                    CancellationToken cancellationToken = context.CancellationToken;

                    NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(simpleIf.Condition, allowedKinds: NullCheckKind.IsNull, semanticModel: semanticModel, cancellationToken: cancellationToken);
                    if (nullCheck.Success)
                    {
                        IdentifierNameSyntax identifierName = GetIdentifierName(nullCheck.Expression);

                        if (identifierName != null)
                        {
                            var fieldSymbol = semanticModel.GetSymbol(identifierName, cancellationToken) as IFieldSymbol;

                            if (fieldSymbol != null)
                            {
                                SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(statement);
                                if (assignmentInfo.Success)
                                {
                                    string fieldName = identifierName.Identifier.ValueText;

                                    return assignmentInfo.Right.IsSingleLine()
                                        && IsBackingField(GetIdentifierName(assignmentInfo.Left), fieldName, fieldSymbol, semanticModel, cancellationToken)
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

            BinaryExpressionSyntax coalesceExpression = CSharpFactory.CoalesceExpression(
                expression,
                CSharpFactory.SimpleAssignmentExpression(expression, assignment.Right.WithoutTrivia()).Parenthesize());

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
