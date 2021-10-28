// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeMetrics;

namespace Roslynator.CSharp.CodeMetrics
{
    [Export(typeof(ILanguageService))]
    [ExportMetadata("Language", LanguageNames.CSharp)]
    [ExportMetadata("ServiceType", "Roslynator.CodeMetrics.ICodeMetricsService")]
    internal class CSharpCodeMetricsService : CodeMetricsService
    {
        public override ISyntaxFactsService SyntaxFacts => CSharpSyntaxFactsService.Instance;

        public override CodeMetricsInfo CountPhysicalLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken)
        {
            TextLineCollection lines = sourceText.Lines;

            var walker = new CSharpPhysicalLinesWalker(lines, options, cancellationToken);

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

            var walker = new CSharpLogicalLinesWalker(lines, options, cancellationToken);

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
