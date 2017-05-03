// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveBracesFromIfElseElseRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            IfStatementSyntax newNode = SyntaxRewriter.VisitNode(ifStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken);
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

                if (_previousIf == null
                    || _previousIf.Equals(node.GetPreviousIf()))
                {
                    if (node.Statement?.IsKind(SyntaxKind.Block) == true)
                    {
                        IfStatementSyntax ifStatement = node.WithStatement(((BlockSyntax)node.Statement).Statements[0]);

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
                    && node.Statement.IsKind(SyntaxKind.Block))
                {
                    return node.WithStatement(((BlockSyntax)node.Statement).Statements[0]);
                }

                return base.VisitElseClause(node);
            }
        }
    }
}
