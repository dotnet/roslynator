// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RemoveBracesFromNestedUsingStatementRefactoring
    {
        public static bool CanRefactor(UsingStatementSyntax usingStatement)
        {
            return UsingStatementAnalysis.ContainsEmbeddableUsingStatement(usingStatement)
                && !usingStatement
                    .Ancestors()
                    .Any(f => f.IsKind(SyntaxKind.UsingStatement)
                        && UsingStatementAnalysis.ContainsEmbeddableUsingStatement((UsingStatementSyntax)f));
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            UsingStatementSyntax usingStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            UsingStatementSyntax newNode = SyntaxRewriter.VisitNode(usingStatement)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(usingStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            private static readonly SyntaxRewriter _instance = new SyntaxRewriter();

            public static UsingStatementSyntax VisitNode(UsingStatementSyntax usingStatement)
            {
                return (UsingStatementSyntax)_instance.Visit(usingStatement);
            }

            public override SyntaxNode VisitUsingStatement(UsingStatementSyntax node)
            {
                if (UsingStatementAnalysis.ContainsEmbeddableUsingStatement(node))
                {
                    var block = (BlockSyntax)node.Statement;

                    node = node.WithStatement(block.Statements[0]);
                }

                return base.VisitUsingStatement(node);
            }
        }
    }
}
