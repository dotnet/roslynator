// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(IfStatementCodeRefactoringProvider))]
    public class IfStatementCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            IfStatementSyntax ifStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<IfStatementSyntax>();

            if (ifStatement == null)
                return;

            AddBracesToIfElseChain(context, ifStatement);

            if (ifStatement.Condition != null
                && ifStatement.Condition.Span.Contains(context.Span)
                && context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
                AddBooleanComparisonRefactoring.Refactor(ifStatement.Condition, context, semanticModel);
            }

            FormatBinaryExpressionRefactoring.Refactor(context, ifStatement);

            SwapStatements(context, ifStatement);
        }

        private static void AddBracesToIfElseChain(CodeRefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (IfElseChainAnalysis.IsTopmostIf(ifStatement)
                && ifStatement.Else != null
                && ifStatement.IfKeyword.Span.Contains(context.Span))
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

        private static void SwapStatements(CodeRefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (IfElseChainAnalysis.IsTopmostIf(ifStatement)
                && ifStatement.Statement != null
                && ifStatement.Condition != null
                && ifStatement.Condition.Span.Contains(context.Span))
            {
                StatementSyntax falseStatement = ifStatement.Else?.Statement;
                if (falseStatement != null
                    && !falseStatement.IsKind(SyntaxKind.IfStatement))
                {
                    context.RegisterRefactoring(
                        "Swap if-else statements",
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