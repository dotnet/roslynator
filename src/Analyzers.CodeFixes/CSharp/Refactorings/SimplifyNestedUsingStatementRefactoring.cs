// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyNestedUsingStatementRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            UsingStatementSyntax usingStatement,
            CancellationToken cancellationToken)
        {
            var rewriter = new SyntaxRewriter();

            var newNode = (UsingStatementSyntax)rewriter.Visit(usingStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(usingStatement, newNode, cancellationToken);
        }

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitUsingStatement(UsingStatementSyntax node)
            {
                node = (UsingStatementSyntax)base.VisitUsingStatement(node);

                if (SimplifyNestedUsingStatementAnalyzer.ContainsEmbeddableUsingStatement(node))
                {
                    var block = (BlockSyntax)node.Statement;

                    SyntaxToken closeParen = node.CloseParenToken;

                    SyntaxTriviaList trailing = closeParen.TrailingTrivia;

                    if (!trailing.Any(SyntaxKind.EndOfLineTrivia))
                    {
                        trailing = trailing.EmptyIfWhitespace().AddRange(block.OpenBraceToken.TrailingTrivia);
                        closeParen = closeParen.WithTrailingTrivia(trailing);
                    }

                    node = node.Update(
                        usingKeyword: node.UsingKeyword,
                        openParenToken: node.OpenParenToken,
                        declaration: node.Declaration,
                        expression: node.Expression,
                        closeParenToken: closeParen,
                        statement: block.Statements[0]);
                }

                return node;
            }
        }
    }
}
