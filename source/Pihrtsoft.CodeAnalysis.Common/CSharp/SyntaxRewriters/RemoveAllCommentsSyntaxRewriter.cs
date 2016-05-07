// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters
{
    public class RemoveAllCommentsSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly CompilationUnitSyntax _compilationUnit;
        private readonly bool _keepXmlComment;

        private RemoveAllCommentsSyntaxRewriter(CompilationUnitSyntax compilationUnit, bool keepXmlComment = false)
            : base(visitIntoStructuredTrivia: true)
        {
            _compilationUnit = compilationUnit;
            _keepXmlComment = keepXmlComment;
        }

        public static CompilationUnitSyntax VisitNode(CompilationUnitSyntax compilationUnit, bool keepXmlComment = false)
        {
            return (CompilationUnitSyntax)new RemoveAllCommentsSyntaxRewriter(compilationUnit, keepXmlComment).Visit(compilationUnit);
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    {
                        return SyntaxHelper.EmptyTrivia;
                    }
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    {
                        if (!_keepXmlComment)
                            return SyntaxHelper.EmptyTrivia;

                        break;
                    }
                case SyntaxKind.EndOfLineTrivia:
                    {
                        if (trivia.SpanStart > 0)
                        {
                            SyntaxTrivia trivia2 = _compilationUnit.FindTrivia(trivia.SpanStart - 1);

                            if (trivia2.IsKind(SyntaxKind.SingleLineCommentTrivia))
                                return SyntaxHelper.EmptyTrivia;
                        }

                        break;
                    }
            }

            return base.VisitTrivia(trivia);
        }
    }
}
