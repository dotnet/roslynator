// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.Documentation
{
    public struct BaseDocumentationCommentData
    {
        internal BaseDocumentationCommentData(SyntaxTrivia comment, BaseDocumentationCommentOrigin origin)
        {
            Comment = comment;
            Origin = origin;
        }

        public SyntaxTrivia Comment { get; }
        public BaseDocumentationCommentOrigin Origin { get; }

        public bool Success
        {
            get { return Comment.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia); }
        }
    }
}
