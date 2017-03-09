// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Documentation
{
    public struct BaseDocumentationCommentInfo
    {
        internal BaseDocumentationCommentInfo(SyntaxTrivia trivia, BaseDocumentationCommentOrigin origin)
        {
            Trivia = trivia;
            Origin = origin;
        }

        public SyntaxTrivia Trivia { get; }
        public BaseDocumentationCommentOrigin Origin { get; }

        public bool Success
        {
            get { return Trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia); }
        }
    }
}
