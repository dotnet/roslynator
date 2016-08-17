// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters
{
    public sealed class CommentRemover : CSharpSyntaxRewriter
    {
        private CommentRemover(SyntaxNode node, CommentRemoveOptions removeOptions, TextSpan span)
            : base(visitIntoStructuredTrivia: true)
        {
            Node = node;
            RemoveOptions = removeOptions;
            Span = span;
        }

        public SyntaxNode Node { get; }
        public CommentRemoveOptions RemoveOptions { get; }
        public TextSpan Span { get; }

        public static CommentRemover Create(SyntaxNode node, CommentRemoveOptions removeOptions)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return new CommentRemover(node, removeOptions, node.FullSpan);
        }

        public static CommentRemover Create(SyntaxNode node, CommentRemoveOptions removeOptions, TextSpan span)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return new CommentRemover(node, removeOptions, span);
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            TextSpan span = trivia.Span;

            if (Span.Contains(span))
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.SingleLineCommentTrivia:
                    case SyntaxKind.MultiLineCommentTrivia:
                        {
                            if (RemoveOptions != CommentRemoveOptions.Documentation)
                                return CSharpFactory.EmptyWhitespaceTrivia;

                            break;
                        }
                    case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    case SyntaxKind.MultiLineDocumentationCommentTrivia:
                        {
                            if (RemoveOptions != CommentRemoveOptions.AllExceptDocumentation)
                                return CSharpFactory.EmptyWhitespaceTrivia;

                            break;
                        }
                    case SyntaxKind.EndOfLineTrivia:
                        {
                            if (RemoveOptions != CommentRemoveOptions.Documentation
                                && span.Start > 0
                                && Node
                                    .FindTrivia(span.Start - 1)
                                    .IsSingleLineCommentTrivia())
                            {
                                return CSharpFactory.EmptyWhitespaceTrivia;
                            }

                            break;
                        }
                }
            }

            return base.VisitTrivia(trivia);
        }
    }
}
