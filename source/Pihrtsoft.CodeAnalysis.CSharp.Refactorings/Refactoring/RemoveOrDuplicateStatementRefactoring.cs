// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RemoveOrDuplicateStatementRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, BlockSyntax block)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemoveStatement))
            {
                StatementSyntax statement = GetStatement(context, block, block.Parent);

                if (statement != null
                    && !EmbeddedStatementAnalysis.IsEmbeddedStatement(statement)
                    && statement.Parent?.IsKind(SyntaxKind.Block) == true)
                {
                    RegisterRefactoring(context, statement);
                }
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemoveStatement)
                && switchStatement.Parent?.IsKind(SyntaxKind.Block) == true
                && (switchStatement.OpenBraceToken.Span.Contains(context.Span)
                    || switchStatement.CloseBraceToken.Span.Contains(context.Span)))
            {
                RegisterRefactoring(context, switchStatement);
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            context.RegisterRefactoring(
                "Remove statement",
                cancellationToken => RemoveStatementAsync(context.Document, statement, cancellationToken));

            context.RegisterRefactoring(
                "Duplicate statement",
                cancellationToken => DuplicateStatementAsync(context.Document, statement, cancellationToken));
        }

        private static StatementSyntax GetStatement(
            RefactoringContext context,
            BlockSyntax block,
            SyntaxNode parent)
        {
            switch (parent?.Kind())
            {
                case SyntaxKind.WhileStatement:
                case SyntaxKind.DoStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.FixedStatement:
                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UncheckedStatement:
                case SyntaxKind.UnsafeStatement:
                case SyntaxKind.UsingStatement:
                case SyntaxKind.LockStatement:
                    {
                        if (block.OpenBraceToken.Span.Contains(context.Span)
                            || block.CloseBraceToken.Span.Contains(context.Span))
                        {
                            if (parent.IsKind(SyntaxKind.UsingStatement))
                            {
                                var usingStatement = (UsingStatementSyntax)parent;

                                while (usingStatement.Parent?.IsKind(SyntaxKind.UsingStatement) == true)
                                    usingStatement = (UsingStatementSyntax)usingStatement.Parent;

                                return usingStatement;
                            }

                            return (StatementSyntax)parent;
                        }

                        break;
                    }
                case SyntaxKind.TryStatement:
                    {
                        var tryStatement = (TryStatementSyntax)parent;

                        if (tryStatement.Block?.OpenBraceToken.Span.Contains(context.Span) == true)
                            return (StatementSyntax)parent;

                        break;
                    }
                case SyntaxKind.IfStatement:
                    {
                        var ifStatement = (IfStatementSyntax)parent;

                        if (IfElseChainAnalysis.IsTopmostIf(ifStatement)
                            && block.OpenBraceToken.Span.Contains(context.Span))
                        {
                            return ifStatement;
                        }

                        if (ifStatement.Else == null
                            && block.CloseBraceToken.Span.Contains(context.Span))
                        {
                            return ifStatement;
                        }

                        break;
                    }
                case SyntaxKind.ElseClause:
                    {
                        var elseClause = (ElseClauseSyntax)parent;

                        if (block.CloseBraceToken.Span.Contains(context.Span))
                            return IfElseChainAnalysis.GetTopmostIf(elseClause);

                        break;
                    }
                case SyntaxKind.CatchClause:
                    {
                        var catchClause = (CatchClauseSyntax)parent;

                        if (catchClause.Parent?.IsKind(SyntaxKind.TryStatement) == true)
                        {
                            var tryStatement = (TryStatementSyntax)catchClause.Parent;

                            if (tryStatement.Finally == null
                                && catchClause.Block?.CloseBraceToken.Span.Contains(context.Span) == true)
                            {
                                return tryStatement;
                            }
                        }

                        break;
                    }
                case SyntaxKind.FinallyClause:
                    {
                        var finallyClause = (FinallyClauseSyntax)parent;

                        if (finallyClause.Parent?.IsKind(SyntaxKind.TryStatement) == true)
                        {
                            var tryStatement = (TryStatementSyntax)finallyClause.Parent;

                            if (finallyClause.Block?.CloseBraceToken.Span.Contains(context.Span) == true)
                                return tryStatement;
                        }

                        break;
                    }
            }

            return null;
        }

        public static async Task<Document> RemoveStatementAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            root = root.RemoveNode(statement, GetRemoveOptions(statement));

            return document.WithSyntaxRoot(root);
        }

        public static async Task<Document> DuplicateStatementAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            var block = (BlockSyntax)statement.Parent;

            int index = block.Statements.IndexOf(statement);

            BlockSyntax newBlock = block.WithStatements(block.Statements.Insert(index, statement));

            root = root.ReplaceNode(block, newBlock);

            return document.WithSyntaxRoot(root);
        }

        internal static SyntaxRemoveOptions GetRemoveOptions(StatementSyntax statement)
        {
            SyntaxRemoveOptions removeOptions = RemoveMemberDeclarationRefactoring.DefaultRemoveOptions;

            if (statement.GetLeadingTrivia().IsWhitespaceOrEndOfLine())
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (statement.GetTrailingTrivia().IsWhitespaceOrEndOfLine())
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return removeOptions;
        }
    }
}
