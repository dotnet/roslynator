// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseLinefeedAsNewLineRefactoring
    {
        public static void Analyze(SyntaxTreeAnalysisContext context)
        {
            if (!context.Tree.TryGetText(out SourceText sourceText))
                return;

            if (!context.Tree.TryGetRoot(out SyntaxNode root))
                return;

            foreach (TextLine textLine in sourceText.Lines)
            {
                int end = textLine.End;

                if (textLine.EndIncludingLineBreak - end == 2
                    && textLine.Text[end] == '\r'
                    && textLine.Text[end + 1] == '\n')
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseLinefeedAsNewLine,
                        Location.Create(context.Tree, new TextSpan(end, 2)));
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.WithTextChangeAsync(new TextChange(span, "\n"), cancellationToken);
        }
    }
}
