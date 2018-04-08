// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddBracesToIfElseRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var rewriter = new SyntaxRewriter();

            var newNode = (IfStatementSyntax)rewriter.Visit(ifStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken);
        }

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            private IfStatementSyntax _previousIf;

            public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                if (_previousIf == null || _previousIf.Equals(node.GetPreviousIf()))
                {
                    if (node.Statement?.IsKind(SyntaxKind.Block) == false)
                    {
                        IfStatementSyntax ifStatement = node.WithStatement(SyntaxFactory.Block(node.Statement));

                        _previousIf = ifStatement;

                        return base.VisitIfStatement(ifStatement);
                    }
                    else
                    {
                        _previousIf = node;
                    }
                }

                return base.VisitIfStatement(node);
            }

            public override SyntaxNode VisitElseClause(ElseClauseSyntax node)
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                if (_previousIf.Equals(node.Parent)
                    && node.Statement != null
                    && !node.Statement.IsKind(SyntaxKind.Block, SyntaxKind.IfStatement))
                {
                    return node.WithStatement(SyntaxFactory.Block(node.Statement));
                }

                return base.VisitElseClause(node);
            }
        }
    }
}
