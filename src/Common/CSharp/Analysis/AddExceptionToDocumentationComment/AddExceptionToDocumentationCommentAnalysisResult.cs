// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        public SyntaxNode Node
        {
            get { return ThrowInfo?.Node; }
        }

        public ITypeSymbol ExceptionSymbol
        {
            get { return ThrowInfo?.ExceptionSymbol; }
        }

        public ExpressionSyntax Expression
        {
            get { return ThrowInfo?.Expression; }
        }

        public ISymbol DeclarationSymbol
        {
            get { return ThrowInfo?.DeclarationSymbol; }
        }

        public SyntaxTrivia DocumentationComment { get; }

        public IParameterSymbol GetParameterSymbol(SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return ThrowInfo?.GetParameterSymbol(semanticModel, cancellationToken);
        }
    }
}