// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnnecessaryUsageOfVerbatimStringLiteralAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UnnecessaryUsageOfVerbatimStringLiteral);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        // Skip base.Initialize: Flags.None also skips user syntax when the containing symbol is generated
        // (e.g. [GeneratedRegex] partial). Visit those nodes, then skip generated trees below.
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.RegisterSyntaxNodeAction(f => AnalyzeStringLiteralExpression(f), SyntaxKind.StringLiteralExpression);
        context.RegisterSyntaxNodeAction(f => AnalyzeInterpolatedStringExpression(f), SyntaxKind.InterpolatedStringExpression);
    }

    private static void AnalyzeStringLiteralExpression(SyntaxNodeAnalysisContext context)
    {
        SyntaxNode node = context.Node;

        if (IsGeneratedTree(context))
            return;

        if (node.ContainsDiagnostics)
            return;

        if (node.SpanContainsDirectives())
            return;

        var literalExpression = (LiteralExpressionSyntax)node;

        string text = literalExpression.Token.Text;

        if (!text.StartsWith("@", StringComparison.Ordinal))
            return;

        if (ContainsQuoteOrBackslashOrCarriageReturnOrLinefeed(text, 2, text.Length - 3))
            return;

        Debug.Assert(string.Compare(text, 2, literalExpression.Token.ValueText, 0, text.Length - 3, StringComparison.Ordinal) == 0, text);

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UnnecessaryUsageOfVerbatimStringLiteral,
            Location.Create(node.SyntaxTree, new TextSpan(node.SpanStart, 1)));
    }

    private static void AnalyzeInterpolatedStringExpression(SyntaxNodeAnalysisContext context)
    {
        SyntaxNode node = context.Node;

        if (IsGeneratedTree(context))
            return;

        if (node.ContainsDiagnostics)
            return;

        if (node.SpanContainsDirectives())
            return;

        var interpolatedString = (InterpolatedStringExpressionSyntax)node;

        if (!interpolatedString.IsVerbatim())
            return;

        foreach (InterpolatedStringContentSyntax content in interpolatedString.Contents)
        {
            if (content is InterpolationSyntax interpolation)
            {
                string text = interpolation.FormatClause?.FormatStringToken.Text;

                if (text?.Contains("\\") == true)
                    return;
            }
        }

        if (interpolatedString.SyntaxTree.IsMultiLineSpan(interpolatedString.Span, context.CancellationToken))
            return;

        foreach (InterpolatedStringContentSyntax content in interpolatedString.Contents)
        {
            if (content is InterpolatedStringTextSyntax interpolatedStringText)
            {
                string text = interpolatedStringText.TextToken.Text;

                if (ContainsQuoteOrBackslashOrCarriageReturnOrLinefeed(text, 0, text.Length))
                    return;

                Debug.Assert(text == interpolatedStringText.TextToken.ValueText, text);
            }
        }

        int start = node.SpanStart;

        if (interpolatedString.StringStartToken.ValueText.StartsWith("$"))
            start++;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UnnecessaryUsageOfVerbatimStringLiteral,
            Location.Create(node.SyntaxTree, new TextSpan(start, 1)));
    }

    private static bool IsGeneratedTree(SyntaxNodeAnalysisContext context)
    {
        return GeneratedCodeUtility.IsGeneratedCode(
            context.Node.SyntaxTree,
            static trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || trivia.IsKind(SyntaxKind.MultiLineCommentTrivia),
            context.CancellationToken);
    }

    private static bool ContainsQuoteOrBackslashOrCarriageReturnOrLinefeed(
        string text,
        int start,
        int length)
    {
        for (int pos = start; pos < start + length; pos++)
        {
            switch (text[pos])
            {
                case '\"':
                case '\\':
                case '\r':
                case '\n':
                    return true;
            }
        }

        return false;
    }
}
