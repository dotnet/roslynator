// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveBracesRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            BlockSyntax block = null;

            if (statement.IsKind(SyntaxKind.Block))
            {
                block = (BlockSyntax)statement;
            }
            else if (statement.IsParentKind(SyntaxKind.Block))
            {
                block = (BlockSyntax)statement.Parent;
            }

            if (block != null)
            {
                ComputeRefactoring(context, block);
            }
        }

        private static void ComputeRefactoring(RefactoringContext context, BlockSyntax block)
        {
            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.RemoveBraces,
                    RefactoringIdentifiers.RemoveBracesFromIfElse)
                && CanRefactor(context, block))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveBraces))
                {
                    context.RegisterRefactoring(
                        "Remove braces",
                        cancellationToken => RefactorAsync(context.Document, block, cancellationToken));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveBracesFromIfElse))
                {
                    IfStatementSyntax topmostIf = GetTopmostIf(block);

                    if (topmostIf?.Else != null
                        && CanRefactorIfElse(block, topmostIf))
                    {
                        context.RegisterRefactoring(
                            "Remove braces from if-else",
                            cancellationToken =>
                            {
                                return RemoveBracesFromIfElseElseRefactoring.RefactorAsync(
                                    context.Document,
                                    topmostIf,
                                    cancellationToken);
                            });
                    }
                }
            }
        }

        private static bool CanRefactorIfElse(BlockSyntax selectedBlock, IfStatementSyntax topmostIf)
        {
            bool success = false;

            foreach (BlockSyntax block in GetBlockStatements(topmostIf))
            {
                if (block == selectedBlock)
                {
                    continue;
                }
                else if (EmbeddedStatementHelper.IsEmbeddableBlock(block))
                {
                    success = true;
                }
                else
                {
                    return false;
                }
            }

            return success;
        }

        private static IEnumerable<BlockSyntax> GetBlockStatements(IfStatementSyntax ifStatement)
        {
            foreach (IfStatementOrElseClause ifOrElse in ifStatement.GetChain())
            {
                StatementSyntax statement = ifOrElse.Statement;

                if (statement?.IsKind(SyntaxKind.Block) == true)
                    yield return (BlockSyntax)statement;
            }
        }

        private static bool CanRefactor(RefactoringContext context, BlockSyntax block)
        {
            if (context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(block)
                && EmbeddedStatementHelper.IsEmbeddableBlock(block))
            {
                StatementSyntax statement = EmbeddedStatementHelper.GetEmbeddedStatement(block.Statements[0]);

                return statement == null
                    || !statement.FullSpan.Contains(context.Span);
            }

            return false;
        }

        private static IfStatementSyntax GetTopmostIf(BlockSyntax block)
        {
            SyntaxNode parent = block.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.IfStatement:
                    return ((IfStatementSyntax)parent).GetTopmostIf();
                case SyntaxKind.ElseClause:
                    return ((ElseClauseSyntax)parent).GetTopmostIf();
                default:
                    return null;
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementSyntax statement = block.Statements[0];

            if (block.IsParentKind(SyntaxKind.ElseClause)
                && statement.IsKind(SyntaxKind.IfStatement))
            {
                var elseClause = (ElseClauseSyntax)block.Parent;

                ElseClauseSyntax newElseClause = elseClause
                    .WithStatement(statement)
                    .WithElseKeyword(elseClause.ElseKeyword.WithoutTrailingTrivia())
                    .WithFormatterAnnotation();

                return document.ReplaceNodeAsync(elseClause, newElseClause, cancellationToken);
            }
            else
            {
                StatementSyntax newNode = statement.TrimLeadingTrivia()
                    .WithFormatterAnnotation();

                return document.ReplaceNodeAsync(block, newNode, cancellationToken);
            }
        }
    }
}
