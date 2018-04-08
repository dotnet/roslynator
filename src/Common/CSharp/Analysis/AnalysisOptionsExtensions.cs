// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    internal static class AnalysisOptionsExtensions
    {
        public static bool CheckSpanDirectives(this AnalysisOptions options, SyntaxNode node)
        {
            return options.CanContainDirectives || !node.SpanContainsDirectives();
        }

        public static bool CheckSpanDirectives(this AnalysisOptions options, SyntaxNode node, TextSpan span)
        {
            if (!options.CanContainDirectives)
            {
                if (node.Span.Contains(span))
                    return !node.ContainsDirectives(span);

                foreach (SyntaxNode ancestor in node.Ancestors())
                {
                    if (ancestor.Span.Contains(span))
                        return !ancestor.ContainsDirectives(span);
                }
            }

            return true;
        }
    }
}
