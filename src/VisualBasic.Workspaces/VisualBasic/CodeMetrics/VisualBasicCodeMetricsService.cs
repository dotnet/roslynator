// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeMetrics;

namespace Roslynator.VisualBasic.CodeMetrics
{
    [Export(typeof(ILanguageService))]
    [ExportMetadata("Language", LanguageNames.VisualBasic)]
    [ExportMetadata("ServiceType", "Roslynator.CodeMetrics.ICodeMetricsService")]
    internal class VisualBasicCodeMetricsService : CodeMetricsService
    {
        public override ISyntaxFactsService SyntaxFacts => VisualBasicSyntaxFactsService.Instance;

        public override CodeMetricsInfo CountPhysicalLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken)
        {
            TextLineCollection lines = sourceText.Lines;

            var walker = new VisualBasicPhysicalLinesWalker(lines, options, cancellationToken);

            walker.Visit(node);

            int whitespaceLineCount = (options.IncludeWhitespace) ? 0 : CountWhitespaceLines(node, sourceText);

            return new CodeMetricsInfo(
                totalLineCount: lines.Count,
                codeLineCount: lines.Count - whitespaceLineCount - walker.CommentLineCount - walker.PreprocessorDirectiveLineCount - walker.BlockBoundaryLineCount,
                whitespaceLineCount: whitespaceLineCount,
                commentLineCount: walker.CommentLineCount,
                preprocessorDirectiveLineCount: walker.PreprocessorDirectiveLineCount,
                blockBoundaryLineCount: walker.BlockBoundaryLineCount);
        }

        public override CodeMetricsInfo CountLogicalLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken)
        {
            TextLineCollection lines = sourceText.Lines;

            var walker = new VisualBasicLogicalLinesWalker(lines, options, cancellationToken);

            walker.Visit(node);

            int whitespaceLineCount = (options.IncludeWhitespace) ? 0 : CountWhitespaceLines(node, sourceText);

            return new CodeMetricsInfo(
                totalLineCount: lines.Count,
                codeLineCount: walker.LogicalLineCount,
                whitespaceLineCount: whitespaceLineCount,
                commentLineCount: walker.CommentLineCount,
                preprocessorDirectiveLineCount: walker.PreprocessorDirectiveLineCount,
                blockBoundaryLineCount: 0);
        }
    }
}
