// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Analysis.AddExceptionToDocumentationComment
{
    internal readonly struct AddExceptionToDocumentationCommentAnalysisResult
    {
        internal AddExceptionToDocumentationCommentAnalysisResult(ThrowInfo info, SyntaxTrivia documentationComment)
        {
            ThrowInfo = info;
            DocumentationComment = documentationComment;
        }

        internal ThrowInfo ThrowInfo { get; }

        public bool Success
        {
            get { return ThrowInfo != null; }
        }

        public ISymbol DeclarationSymbol
        {
            get { return ThrowInfo?.DeclarationSymbol; }
        }

        public SyntaxTrivia DocumentationComment { get; }
    }
}