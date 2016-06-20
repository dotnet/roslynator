// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class IfStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (ifStatement.IfKeyword.Span.Contains(context.Span)
                && IfElseChainAnalysis.IsTopmostIf(ifStatement))
            {
                AddBracesToIfElseChain(context, ifStatement);
                SwapStatements(context, ifStatement);
            }

            if (ifStatement.Condition != null
                && ifStatement.Condition.Span.Contains(context.Span)
                && context.SupportsSemanticModel)
            {
                await AddBooleanComparisonRefactoring.RefactorAsync(context, ifStatement.Condition);
            }
        }

        private static void AddBracesToIfElseChain(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (ifStatement.Else != null)
            {
                var result = new IfElseChainAnalysisResult(ifStatement);

                if (result.AddBracesToChain)
                {
                    context.RegisterRefactoring(
                        "Add braces to if-else chain",
                        cancellationToken =>
                        {
                            return AddBracesToIfElseChainRefactoring.RefactorAsync(
                                context.Document,
                                ifStatement,
                                cancellationToken);
                        });
                }

                if (result.RemoveBracesFromChain)
                {
                    context.RegisterRefactoring(
                        "Remove braces from if-else chain",
                        cancellationToken =>
                        {
                            return RemoveBracesFromIfElseChainRefactoring.RefactorAsync(
                                context.Document,
                                ifStatement,
                                cancellationToken);
                        });
                }
            }
        }

        private static void SwapStatements(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (ifStatement.Condition != null
                && ifStatement.Statement != null)
            {
                StatementSyntax falseStatement = ifStatement.Else?.Statement;
                if (falseStatement != null
                    && !falseStatement.IsKind(SyntaxKind.IfStatement))
                {
                    context.RegisterRefactoring(
                        "Swap statements",
                        cancellationToken =>
                        {
                            return SwapStatementsAsync(
                                context.Document,
                                ifStatement,
                                cancellationToken);
                        });
                }
            }
        }

        private static async Task<Document> SwapStatementsAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            StatementSyntax trueStatement = ifStatement.Statement;

            StatementSyntax falseStatement = ifStatement.Else.Statement;

            IfStatementSyntax newIfStatement = ifStatement
                .WithCondition(ifStatement.Condition.Negate())
                .WithStatement(falseStatement.WithTriviaFrom(trueStatement))
                .WithElse(ifStatement.Else.WithStatement(trueStatement.WithTriviaFrom(falseStatement)))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(ifStatement, newIfStatement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}