// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ExtractStatementRefactoring
    {
        public static void Refactor(CodeRefactoringContext context, StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            if (statement.IsKind(SyntaxKind.Block)
                && ((BlockSyntax)statement).Statements.Count == 0)
            {
                return;
            }

            if (statement.Parent?.IsKind(SyntaxKind.Block) == true)
                statement = (BlockSyntax)statement.Parent;

            if (statement.Parent != null
                && (CheckContainingNode(statement.Parent)
                && GetContainingBlock(statement.Parent)?.IsKind(SyntaxKind.Block) == true))
            {
                string s = (UsePlural(statement)) ? "s" : "";

                context.RegisterRefactoring(
                    $"Extract statement{s} from {SyntaxHelper.GetSyntaxNodeName(statement.Parent)}",
                    cancellationToken => RefactorAsync(context.Document, statement, cancellationToken));
            }
        }

        private static SyntaxNode GetContainingBlock(SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.ElseClause))
            {
                return IfElseChainAnalysis.GetTopIf((ElseClauseSyntax)node)?.Parent;
            }
            else
            {
                return node.Parent;
            }
        }

        private static bool CheckContainingNode(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.UsingStatement:
                case SyntaxKind.WhileStatement:
                case SyntaxKind.DoStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.FixedStatement:
                case SyntaxKind.UnsafeStatement:
                case SyntaxKind.TryStatement:
                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UncheckedStatement:
                    return true;
                case SyntaxKind.IfStatement:
                    return IfElseChainAnalysis.IsTopIf((IfStatementSyntax)node);
                case SyntaxKind.ElseClause:
                    return IfElseChainAnalysis.IsEndOfChain((ElseClauseSyntax)node);
            }

            return false;
        }

        private static bool UsePlural(StatementSyntax statement)
        {
            if (statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement;

                if (block.Statements.Count > 1)
                    return true;
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            IEnumerable<StatementSyntax> newNodes = GetNewNodes(statement)
                .Select(f => f.WithAdditionalAnnotations(Formatter.Annotation));

            SyntaxNode newRoot = GetNewRoot(statement, oldRoot, newNodes);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode GetNewRoot(StatementSyntax statement, SyntaxNode oldRoot, IEnumerable<StatementSyntax> newNodes)
        {
            if (statement.Parent.IsKind(SyntaxKind.ElseClause))
            {
                IfStatementSyntax ifStatement = IfElseChainAnalysis.GetTopIf((ElseClauseSyntax)statement.Parent);
                var block = (BlockSyntax)ifStatement.Parent;
                int index = block.Statements.IndexOf(ifStatement);

                BlockSyntax newBlock = block.RemoveNode(statement.Parent, SyntaxRemoveOptions.KeepNoTrivia);

                newBlock = newBlock.WithStatements(newBlock.Statements.InsertRange(index + 1, newNodes));

                return oldRoot.ReplaceNode(block, newBlock);
            }
            else
            {
                return oldRoot.ReplaceNode(statement.Parent, newNodes);
            }
        }

        private static IEnumerable<StatementSyntax> GetNewNodes(StatementSyntax statement)
        {
            SyntaxTriviaList leadingTrivia = statement.Parent.GetLeadingTrivia();

            if (statement.IsKind(SyntaxKind.ElseClause))
                leadingTrivia.Insert(0, SyntaxHelper.NewLine);

            if (statement.IsKind(SyntaxKind.Block))
            {
                SyntaxList<StatementSyntax>.Enumerator en = ((BlockSyntax)statement).Statements.GetEnumerator();

                if (en.MoveNext())
                {
                    yield return en.Current.WithLeadingTrivia(leadingTrivia);

                    while (en.MoveNext())
                        yield return en.Current;
                }
            }
            else
            {
                yield return statement.WithLeadingTrivia(leadingTrivia);
            }
        }
    }
}
