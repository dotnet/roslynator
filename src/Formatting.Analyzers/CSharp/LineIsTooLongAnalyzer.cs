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
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, Roslynator.Formatting.CSharp.FormattingDiagnosticRules.LineIsTooLong);

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

        int maxLength = context.GetConfigOptions().GetMaxLineLength();

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

        TextLineCollection lines = sourceText.Lines;

        for (; i < lines.Count; i++)
        {
            TextLine line = lines[i];

            if (line.Span.Length <= maxLength)
                continue;

            int start = line.Start;
            int end = line.End;

            SyntaxToken token = root.FindToken(end);

            if (token.IsKind(SyntaxKind.None)
                || token.Span.End < start)
            {
                continue;
            }

            SyntaxToken token2 = token;

            if (token2.IsKind(SyntaxKind.CommaToken, SyntaxKind.SemicolonToken))
                token2 = token2.GetPreviousToken();

            if (!token2.IsKind(SyntaxKind.None)
                && token2.Span.End >= start)
            {
                while (token2.IsKind(
                    SyntaxKind.CloseParenToken,
                    SyntaxKind.CloseBraceToken,
                    SyntaxKind.CloseBracketToken))
                {
                    token2 = token2.GetPreviousToken();
                }

                if (token2.IsKind(
                    SyntaxKind.StringLiteralToken,
#if ROSLYN_4_2
                    SyntaxKind.InterpolatedRawStringEndToken,
                    SyntaxKind.MultiLineRawStringLiteralToken,
#endif
                    SyntaxKind.InterpolatedStringTextToken,
                    SyntaxKind.InterpolatedStringEndToken))
                {
                    SyntaxNode parent = token2.Parent;

                    if (parent.SpanStart <= start)
                        continue;

                    token2 = parent.GetFirstToken().GetPreviousToken();

                    if (token2.IsKind(SyntaxKind.None)
                        || token2.Span.End < start)
                    {
                        continue;
                    }

                    if (parent.IsParentKind(
                        SyntaxKind.ArrowExpressionClause,
                        SyntaxKind.Argument,
                        SyntaxKind.AttributeArgument))
                    {
                        SyntaxToken firstToken = parent.Parent.GetFirstToken();

                        if (firstToken.SpanStart >= start)
                        {
                            SyntaxToken token3 = firstToken.GetPreviousToken();

                            if (token3.IsKind(SyntaxKind.None)
                                || token3.Span.End < start)
                            {
                                continue;
                            }
                        }
                    }

                    if (parent.Span.End > end)
                    {
                        i = lines.IndexOf(parent.Span.End) - 1;
                    }
                }
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
                Roslynator.Formatting.CSharp.FormattingDiagnosticRules.LineIsTooLong,
                Location.Create(tree, line.Span),
                line.Span.Length);
        }
    }
}
