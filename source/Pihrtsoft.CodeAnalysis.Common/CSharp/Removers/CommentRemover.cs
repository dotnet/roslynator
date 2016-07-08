// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Removers
{
    public sealed class CommentRemover : CSharpSyntaxRewriter
    {
        private readonly CompilationUnitSyntax _compilationUnit;
        private readonly CommentRemoveOptions _removeOptions;

        private CommentRemover(CompilationUnitSyntax compilationUnit, CommentRemoveOptions removeOptions)
            : base(visitIntoStructuredTrivia: true)
        {
            _compilationUnit = compilationUnit;
            _removeOptions = removeOptions;
        }

        public static CompilationUnitSyntax RemoveFrom(CompilationUnitSyntax compilationUnit, CommentRemoveOptions removeOptions)
        {
            return (CompilationUnitSyntax)new CommentRemover(compilationUnit, removeOptions).Visit(compilationUnit);
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    {
                        if (_removeOptions != CommentRemoveOptions.Documentation)
                            return CSharpFactory.EmptyTrivia;

                        break;
                    }
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    {
                        if (_removeOptions != CommentRemoveOptions.AllExceptDocumentation)
                            return CSharpFactory.EmptyTrivia;

                        break;
                    }
                case SyntaxKind.EndOfLineTrivia:
                    {
                        if (_removeOptions != CommentRemoveOptions.Documentation
                            && trivia.SpanStart > 0)
                        {
                            SyntaxTrivia trivia2 = _compilationUnit.FindTrivia(trivia.SpanStart - 1);

                            if (trivia2.IsKind(SyntaxKind.SingleLineCommentTrivia))
                                return CSharpFactory.EmptyTrivia;
                        }

                        break;
                    }
            }

            return base.VisitTrivia(trivia);
        }
    }
}
