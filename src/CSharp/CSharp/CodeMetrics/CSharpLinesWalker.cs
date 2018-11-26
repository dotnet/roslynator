// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeMetrics;

namespace Roslynator.CSharp.CodeMetrics
{
    internal abstract class CSharpLinesWalker : CSharpSyntaxWalker
    {
        public int CommentLineCount { get; set; }

        public int PreprocessorDirectiveLineCount { get; set; }

        public TextLineCollection Lines { get; }

        public CodeMetricsOptions Options { get; }

        public CancellationToken CancellationToken { get; }

        protected CSharpLinesWalker(TextLineCollection lines, CodeMetricsOptions options, CancellationToken cancellationToken)
            : base(SyntaxWalkerDepth.Trivia)
        {
            Lines = lines;
            Options = options;
            CancellationToken = cancellationToken;
        }

        public override void VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.IsDirective)
            {
                if (!Options.IncludePreprocessorDirectives)
                {
                    PreprocessorDirectiveLineCount++;
                }
            }
            else if (!Options.IncludeComments)
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.SingleLineCommentTrivia:
                        {
                            TextSpan span = trivia.Span;

                            TextLine line = Lines.GetLineFromPosition(span.Start);

                            if (line.IsEmptyOrWhiteSpace(TextSpan.FromBounds(line.Start, span.Start)))
                            {
                                CommentLineCount++;
                            }

                            break;
                        }
                    case SyntaxKind.SingleLineDocumentationCommentTrivia:
                        {
                            CommentLineCount += Lines.GetLineCount(trivia.Span) - 1;
                            break;
                        }
                    case SyntaxKind.MultiLineCommentTrivia:
                        {
                            TextSpan span = trivia.Span;

                            TextLine line = Lines.GetLineFromPosition(span.Start);

                            if (line.IsEmptyOrWhiteSpace(TextSpan.FromBounds(line.Start, span.Start)))
                            {
                                int lineCount = Lines.GetLineCount(trivia.Span);

                                if (lineCount == 1
                                    || line.IsEmptyOrWhiteSpace(TextSpan.FromBounds(Lines.GetLineFromPosition(span.End).End, span.End)))
                                {
                                    CommentLineCount += lineCount;
                                }
                            }

                            break;
                        }
                    case SyntaxKind.MultiLineDocumentationCommentTrivia:
                        {
                            CommentLineCount += Lines.GetLineCount(trivia.Span);
                            break;
                        }
                }
            }

            base.VisitTrivia(trivia);
        }
    }
}
