// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LineIsTooLongAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.LineIsTooLong);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxTreeAction(f => AnalyzeSyntaxTree(f));
    }

    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        SyntaxTree tree = context.Tree;

        if (!tree.TryGetText(out SourceText sourceText))
            return;

        AnalyzerConfigOptions configOptions = context.GetConfigOptions();
        int maxLength = configOptions.GetMaxLineLength();

        if (maxLength <= 0)
            return;

        SyntaxNode root = tree.GetRoot(context.CancellationToken);

        int i = 0;

        SyntaxTrivia trivia = root.FindTrivia(0);

        if (trivia.SpanStart == 0
            && trivia.IsKind(SyntaxKind.SingleLineCommentTrivia, SyntaxKind.MultiLineCommentTrivia))
        {
            SyntaxTriviaList leadingTrivia = trivia.Token.LeadingTrivia;

            int count = leadingTrivia.Count;

            if (count > 1)
            {
                int j = 0;

                while (j < leadingTrivia.Count - 1
                    && leadingTrivia[j].IsKind(SyntaxKind.SingleLineCommentTrivia, SyntaxKind.MultiLineCommentTrivia)
                    && leadingTrivia[j + 1].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    i++;

                    j += 2;
                }
            }
        }

        int indentSize = 0;
        TextLineCollection lines = sourceText.Lines;

        for (; i < lines.Count; i++)
        {
            TextLine line = lines[i];

            if (line.Span.Length <= maxLength)
                continue;

            int end = line.End;

            SyntaxToken token = root.FindToken(end);

            if (token.IsKind(SyntaxKind.StringLiteralToken, SyntaxKind.InterpolatedStringEndToken, SyntaxKind.InterpolatedRawStringEndToken))
            {
                TextSpan span = token.Parent.Span;

                if (span.End == end)
                {
                    if (span.Length >= maxLength)
                        continue;
                }
                else if (span.Contains(end)
                    && end - span.Start >= maxLength)
                {
                    continue;
                }
            }
            else
            {
                var isStringThanCannotBeWrapped = false;
                SyntaxToken token2 = token;
                while (true)
                {
                    token2 = token2.GetPreviousToken();

                    if (token2.IsKind(SyntaxKind.None)
                        || token2.Span.End < line.Start)
                    {
                        break;
                    }

                    if (token2.IsKind(SyntaxKind.StringLiteralToken, SyntaxKind.InterpolatedStringEndToken, SyntaxKind.InterpolatedRawStringEndToken))
                    {
                        TextSpan span = token2.Parent.Span;

                        if (span.Start < line.Start)
                        {
                            if ((span.End - line.Start) > maxLength)
                            {
                                isStringThanCannotBeWrapped = true;
                                break;
                            }
                        }
                        else
                        {
                            if (indentSize == 0)
                                indentSize = GetIndentSize(configOptions);

                            SyntaxTrivia indentationTrivia = SyntaxTriviaAnalysis.DetermineIndentation(token2, context.CancellationToken);

                            int length = span.Length + indentSize;

                            foreach (char ch in indentationTrivia.ToString())
                            {
                                length += (ch == '\t') ? indentSize : 1;
                            }

                            if (length > maxLength)
                            {
                                isStringThanCannotBeWrapped = true;
                                break;
                            }
                        }
                    }
                }

                if (isStringThanCannotBeWrapped)
                    continue;
            }

            SyntaxTriviaList list = default;

            if (token.LeadingTrivia.Span.Contains(end))
            {
                list = token.LeadingTrivia;
            }
            else if (token.TrailingTrivia.Span.Contains(end))
            {
                list = token.TrailingTrivia;
            }

            if (list.Any())
            {
                int index = -1;

                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].Span.Contains(end))
                    {
                        trivia = list[j];
                        index = j;
                    }
                }

                if (index >= 0)
                {
                    SyntaxKind kind = trivia.Kind();

                    if (kind == SyntaxKind.MultiLineCommentTrivia
                        || kind == SyntaxKind.SingleLineDocumentationCommentTrivia
                        || kind == SyntaxKind.MultiLineDocumentationCommentTrivia)
                    {
                        continue;
                    }

                    if (kind == SyntaxKind.EndOfLineTrivia
                        && index > 0
                        && list[index - 1].IsKind(SyntaxKind.SingleLineCommentTrivia))
                    {
                        continue;
                    }
                }
            }

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.LineIsTooLong,
                Location.Create(tree, line.Span),
                line.Span.Length);
        }
    }

    private static int GetIndentSize(AnalyzerConfigOptions configOptions)
    {
        int tabLength = 4;

        if (configOptions.TryGetIndentStyle(out IndentStyle indentStyle)
            && indentStyle == IndentStyle.Tab)
        {
            if (configOptions.TryGetTabLength(out int tabLength2))
                tabLength = tabLength2;

            return tabLength;
        }
        else if (configOptions.TryGetIndentSize(out int indentSize2))
        {
            return indentSize2;
        }

        return 4;
    }
}
