// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslynator.CodeMetrics;

namespace Roslynator.VisualBasic.CodeMetrics
{
    internal class VisualBasicPhysicalLinesWalker : VisualBasicLinesWalker
    {
        public int BlockBoundaryLineCount { get; set; }

        public VisualBasicPhysicalLinesWalker(TextLineCollection lines, CodeMetricsOptions options, CancellationToken cancellationToken)
            : base(lines, options, cancellationToken)
        {
        }

        public override void VisitEndBlockStatement(EndBlockStatementSyntax node)
        {
            if (Options.IgnoreBlockBoundary)
            {
                TextSpan span = node.Span;

                TextLine line = Lines.GetLineFromPosition(span.Start);

                if (line.IsEmptyOrWhiteSpace(TextSpan.FromBounds(line.Start, span.Start)))
                {
                    if (Options.IncludeComments)
                    {
                        if (line.IsEmptyOrWhiteSpace(TextSpan.FromBounds(span.End, line.End)))
                        {
                            BlockBoundaryLineCount += Lines.GetLineCount(span);
                        }
                    }
                    else if (AnalyzeTrailingTrivia(node.GetTrailingTrivia()))
                    {
                        BlockBoundaryLineCount += Lines.GetLineCount(span);
                    }
                }
            }

            base.VisitEndBlockStatement(node);

            bool AnalyzeTrailingTrivia(in SyntaxTriviaList trailingTrivia)
            {
                SyntaxTriviaList.Enumerator en = trailingTrivia.GetEnumerator();

                while (en.MoveNext())
                {
                    switch (en.Current.Kind())
                    {
                        case SyntaxKind.EndOfLineTrivia:
                        case SyntaxKind.CommentTrivia:
                            {
                                return true;
                            }
                        case SyntaxKind.WhitespaceTrivia:
                            {
                                break;
                            }
                        default:
                            {
                                return false;
                            }
                    }
                }

                return false;
            }
        }
    }
}
