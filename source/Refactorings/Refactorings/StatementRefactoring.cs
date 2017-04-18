// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class StatementRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, BlockSyntax block)
        {
            if (context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.RemoveStatement,
                RefactoringIdentifiers.DuplicateStatement,
                RefactoringIdentifiers.CommentOutStatement))
            {
                StatementSyntax statement = GetStatement(context, block, block.Parent);

                if (statement != null)
                {
                    if (!EmbeddedStatementHelper.IsEmbeddedStatement(statement)
                        && statement.IsParentKind(SyntaxKind.Block))
                    {
                        RegisterRefactoring(context, statement);
                    }

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.CommentOutStatement))
                        CommentOutRefactoring.RegisterRefactoring(context, statement);
                }
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            if (switchStatement.OpenBraceToken.Span.Contains(context.Span)
                || switchStatement.CloseBraceToken.Span.Contains(context.Span))
            {
                if (switchStatement.IsParentKind(SyntaxKind.Block))
                    RegisterRefactoring(context, switchStatement);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.CommentOutStatement))
                    CommentOutRefactoring.RegisterRefactoring(context, switchStatement);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllSwitchSections)
                    && switchStatement.Sections.Any())
                {
                    context.RegisterRefactoring(
                        "Remove all sections",
                        cancellationToken => RemoveAllSwitchSectionsAsync(context.Document, switchStatement, cancellationToken));
                }
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveStatement))
            {
                context.RegisterRefactoring(
                    "Remove statement",
                    cancellationToken => RemoveStatementAsync(context.Document, statement, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.DuplicateStatement))
            {
                context.RegisterRefactoring(
                    "Duplicate statement",
                    cancellationToken => DuplicateStatementAsync(context.Document, statement, cancellationToken));
            }
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
                case SyntaxKind.ForEachVariableStatement:
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

                                while (usingStatement.IsParentKind(SyntaxKind.UsingStatement))
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

                        if (ifStatement.IsTopmostIf()
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
                            return elseClause.GetTopmostIf();

                        break;
                    }
                case SyntaxKind.CatchClause:
                    {
                        var catchClause = (CatchClauseSyntax)parent;

                        if (catchClause.IsParentKind(SyntaxKind.TryStatement))
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

                        if (finallyClause.IsParentKind(SyntaxKind.TryStatement))
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

        private static Task<Document> RemoveStatementAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.RemoveNodeAsync(statement, GetRemoveOptions(statement), cancellationToken);
        }

        private static Task<Document> DuplicateStatementAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var block = (BlockSyntax)statement.Parent;

            int index = block.Statements.IndexOf(statement);

            if (index == 0
                && block.OpenBraceToken.GetFullSpanEndLine() == statement.GetFullSpanStartLine())
            {
                statement = statement.WithLeadingTrivia(statement.GetLeadingTrivia().Insert(0, CSharpFactory.NewLine()));
            }

            BlockSyntax newBlock = block.WithStatements(block.Statements.Insert(index + 1, statement));

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }

        private static SyntaxRemoveOptions GetRemoveOptions(StatementSyntax statement)
        {
            SyntaxRemoveOptions removeOptions = RemoveHelper.DefaultRemoveOptions;

            if (statement.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (statement.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return removeOptions;
        }

        private static Task<Document> RemoveAllSwitchSectionsAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SwitchStatementSyntax newSwitchStatement = switchStatement
                .WithSections(default(SyntaxList<SwitchSectionSyntax>))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken);
        }
    }
}
