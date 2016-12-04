// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveBracesFromUsingStatementRefactoring
    {
        public static bool CanRefactor(UsingStatementSyntax usingStatement)
        {
            return ContainsEmbeddableUsingStatement(usingStatement)
                && !usingStatement
                    .Ancestors()
                    .Any(f => f.IsKind(SyntaxKind.UsingStatement)
                        && ContainsEmbeddableUsingStatement((UsingStatementSyntax)f));
        }

        public static bool ContainsEmbeddableUsingStatement(UsingStatementSyntax usingStatement)
        {
            StatementSyntax statement = usingStatement.Statement;

            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)statement;
                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Count == 1
                    && statements[0].IsKind(SyntaxKind.UsingStatement))
                {
                    var usingStatement2 = (UsingStatementSyntax)statements[0];

                    return block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && usingStatement2.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && usingStatement2.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia());
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            UsingStatementSyntax usingStatement,
            CancellationToken cancellationToken)
        {
            var rewriter = new SyntaxRewriter();

            var newNode = (UsingStatementSyntax)rewriter.Visit(usingStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(usingStatement, newNode, cancellationToken).ConfigureAwait(false);
        }

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitUsingStatement(UsingStatementSyntax node)
            {
                if (ContainsEmbeddableUsingStatement(node))
                {
                    var block = (BlockSyntax)node.Statement;

                    node = node.WithStatement(block.Statements[0]);
                }

                return base.VisitUsingStatement(node);
            }
        }
    }
}
