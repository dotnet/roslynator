// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CodeMetrics
{
    internal abstract class CodeMetricsCounter
    {
        internal abstract SyntaxFactsService SyntaxFacts { get; }

        public abstract CodeMetricsInfo CountLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken);

        private protected int CountWhitespaceLines(SyntaxNode root, SourceText sourceText)
        {
            int whitespaceLineCount = 0;

            foreach (TextLine line in sourceText.Lines)
            {
                if (line.IsEmptyOrWhiteSpace())
                {
                    if (line.End == sourceText.Length
                        || SyntaxFacts.IsEndOfLineTrivia(root.FindTrivia(line.End)))
                    {
                        whitespaceLineCount++;
                    }
                }
            }

            return whitespaceLineCount;
        }
    }
}
