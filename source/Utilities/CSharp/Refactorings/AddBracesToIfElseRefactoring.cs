// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    public static class AddBracesToIfElseRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            IfStatementSyntax newNode = SyntaxRewriter.VisitNode(ifStatement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(ifStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            private IfStatementSyntax _previousIf;

            private SyntaxRewriter()
            {
            }

            public static IfStatementSyntax VisitNode(IfStatementSyntax node)
            {
                return (IfStatementSyntax)new SyntaxRewriter().Visit(node);
            }

            public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                if (_previousIf == null || _previousIf.Equals(IfElseAnalysis.GetPreviousIf(node)))
                {
                    if (node.Statement != null
                        && !node.Statement.IsKind(SyntaxKind.Block))
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
                    && !node.Statement.IsKind(SyntaxKind.Block)
                    && !node.Statement.IsKind(SyntaxKind.IfStatement))
                {
                    return node.WithStatement(SyntaxFactory.Block(node.Statement));
                }

                return base.VisitElseClause(node);
            }
        }
    }
}
