// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractStatementRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            if (!context.Span.IsEmpty)
                return;

            if (statement.Kind() == SyntaxKind.Block
                && !((BlockSyntax)statement).Statements.Any())
            {
                return;
            }

            SyntaxNode parent = statement.Parent;

            if (parent?.Kind() == SyntaxKind.Block)
            {
                statement = (BlockSyntax)parent;
                parent = statement.Parent;
            }

            if (parent == null)
                return;

            if (!CheckContainingNode(parent))
                return;

            if (GetContainingBlock(parent)?.Kind() != SyntaxKind.Block)
                return;

            context.RegisterRefactoring(
                (parent.Kind() == SyntaxKind.ElseClause)
                    ? "Extract from containing else clause"
                    : "Extract from containing statement",
                cancellationToken => RefactorAsync(context.Document, statement, cancellationToken));
        }

        private static SyntaxNode GetContainingBlock(SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.ElseClause))
            {
                return ((ElseClauseSyntax)node).GetTopmostIf()?.Parent;
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
                case SyntaxKind.ForEachVariableStatement:
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
                    return ((IfStatementSyntax)node).IsTopmostIf();
                case SyntaxKind.ElseClause:
                    return ((ElseClauseSyntax)node).Statement?.Kind() != SyntaxKind.IfStatement;
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<StatementSyntax> newNodes = GetNewNodes(statement).Select(f => f.WithFormatterAnnotation());

            if (statement.IsParentKind(SyntaxKind.ElseClause))
            {
                IfStatementSyntax ifStatement = ((ElseClauseSyntax)statement.Parent).GetTopmostIf();
                var block = (BlockSyntax)ifStatement.Parent;
                int index = block.Statements.IndexOf(ifStatement);

                BlockSyntax newBlock = block.RemoveNode(statement.Parent, SyntaxRemoveOptions.KeepNoTrivia);

                newBlock = newBlock.WithStatements(newBlock.Statements.InsertRange(index + 1, newNodes));

                return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
            }
            else
            {
                return document.ReplaceNodeAsync(statement.Parent, newNodes, cancellationToken);
            }
        }

        private static IEnumerable<StatementSyntax> GetNewNodes(StatementSyntax statement)
        {
            List<SyntaxTrivia> list = null;

            if (statement.IsParentKind(SyntaxKind.ElseClause))
            {
                list = new List<SyntaxTrivia>() { CSharpFactory.NewLine() };
            }
            else
            {
                list = statement.Parent.GetLeadingTrivia()
                    .Reverse()
                    .SkipWhile(f => f.IsWhitespaceTrivia())
                    .Reverse()
                    .ToList();
            }

            if (statement.IsKind(SyntaxKind.Block))
            {
                SyntaxList<StatementSyntax>.Enumerator en = ((BlockSyntax)statement).Statements.GetEnumerator();

                if (en.MoveNext())
                {
                    list.AddRange(en.Current.GetLeadingTrivia());

                    yield return en.Current.WithLeadingTrivia(list);

                    while (en.MoveNext())
                        yield return en.Current;
                }
            }
            else
            {
                list.AddRange(statement.GetLeadingTrivia());

                yield return statement.WithLeadingTrivia(list);
            }
        }
    }
}
