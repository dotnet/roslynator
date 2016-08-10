// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Removers
{
    public sealed class CommentRemover : CSharpSyntaxRewriter
    {
        private readonly SyntaxNode _node;
        private readonly CommentRemoveOptions _removeOptions;

        internal CommentRemover(SyntaxNode node, CommentRemoveOptions removeOptions)
            : base(visitIntoStructuredTrivia: true)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            _node = node;
            _removeOptions = removeOptions;
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    {
                        if (_removeOptions != CommentRemoveOptions.Documentation)
                            return CSharpFactory.EmptyWhitespaceTrivia;

                        break;
                    }
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    {
                        if (_removeOptions != CommentRemoveOptions.AllExceptDocumentation)
                            return CSharpFactory.EmptyWhitespaceTrivia;

                        break;
                    }
                case SyntaxKind.EndOfLineTrivia:
                    {
                        if (_removeOptions != CommentRemoveOptions.Documentation
                            && trivia.SpanStart > 0)
                        {
                            SyntaxTrivia trivia2 = _node.FindTrivia(trivia.SpanStart - 1);

                            if (trivia2.IsKind(SyntaxKind.SingleLineCommentTrivia))
                                return CSharpFactory.EmptyWhitespaceTrivia;
                        }

                        break;
                    }
            }

            return base.VisitTrivia(trivia);
        }
    }
}
