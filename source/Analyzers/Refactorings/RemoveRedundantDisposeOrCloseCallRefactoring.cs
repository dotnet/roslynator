// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantDisposeOrCloseCallRefactoring
    {
        public static void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            var usingStatement = (UsingStatementSyntax)context.Node;

            StatementSyntax statement = usingStatement.Statement;

            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)statement;

                StatementSyntax lastStatement = block.Statements.LastOrDefault();

                if (lastStatement?.IsKind(SyntaxKind.ExpressionStatement) == true)
                {
                    var expressionStatement = (ExpressionStatementSyntax)lastStatement;

                    ExpressionSyntax expression = expressionStatement.Expression;

                    if (expression?.IsKind(SyntaxKind.InvocationExpression) == true)
                    {
                        var invocation = (InvocationExpressionSyntax)expression;

                        ExpressionSyntax invocationExpression = invocation.Expression;

                        if (invocationExpression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                        {
                            var memberAccess = (MemberAccessExpressionSyntax)invocationExpression;

                            if (invocation.ArgumentList?.Arguments.Count == 0)
                            {
                                string methodName = memberAccess.Name?.Identifier.ValueText;

                                if (methodName == "Dispose" || methodName == "Close")
                                {
                                    ExpressionSyntax memberAccessExpression = memberAccess.Expression;

                                    if (memberAccessExpression != null)
                                    {
                                        ExpressionSyntax usingExpression = usingStatement.Expression;

                                        if (usingExpression != null)
                                        {
                                            if (SyntaxComparer.AreEquivalent(memberAccessExpression, usingExpression))
                                                ReportDiagnostic(context, expressionStatement, methodName);
                                        }
                                        else if (memberAccessExpression.IsKind(SyntaxKind.IdentifierName))
                                        {
                                            VariableDeclarationSyntax usingDeclaration = usingStatement.Declaration;

                                            if (usingDeclaration != null)
                                            {
                                                var identifierName = (IdentifierNameSyntax)memberAccessExpression;

                                                string name = identifierName.Identifier.ValueText;

                                                VariableDeclaratorSyntax declarator = usingDeclaration.Variables.LastOrDefault();

                                                if (declarator != null
                                                    && declarator.Identifier.ValueText == name)
                                                {
                                                    ISymbol symbol = context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);

                                                    if (symbol?.Equals(context.SemanticModel.GetSymbol(identifierName, context.CancellationToken)) == true)
                                                        ReportDiagnostic(context, expressionStatement, methodName);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ExpressionStatementSyntax expressionStatement, string methodName)
        {
            if (!expressionStatement.SpanContainsDirectives())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveRedundantDisposeOrCloseCall,
                    expressionStatement,
                    methodName);
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            CancellationToken cancellationToken)
        {
            var block = (BlockSyntax)expressionStatement.Parent;

            BlockSyntax newBlock = block.RemoveStatement(expressionStatement);

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }
    }
}
