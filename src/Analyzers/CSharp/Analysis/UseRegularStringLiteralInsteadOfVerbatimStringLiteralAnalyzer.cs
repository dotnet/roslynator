// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseRegularStringLiteralInsteadOfVerbatimStringLiteralAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseRegularStringLiteralInsteadOfVerbatimStringLiteral); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeStringLiteralExpression, SyntaxKind.StringLiteralExpression);
            context.RegisterSyntaxNodeAction(AnalyzeInterpolatedStringExpression, SyntaxKind.InterpolatedStringExpression);
        }

        public static void AnalyzeStringLiteralExpression(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

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

            context.ReportDiagnostic(
                DiagnosticDescriptors.UseRegularStringLiteralInsteadOfVerbatimStringLiteral,
                Location.Create(node.SyntaxTree, new TextSpan(node.SpanStart, 1)));
        }

        public static void AnalyzeInterpolatedStringExpression(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (node.ContainsDiagnostics)
                return;

            if (node.SpanContainsDirectives())
                return;

            var interpolatedString = (InterpolatedStringExpressionSyntax)node;

            if (!interpolatedString.IsVerbatim())
                return;

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

            context.ReportDiagnostic(
                DiagnosticDescriptors.UseRegularStringLiteralInsteadOfVerbatimStringLiteral,
                Location.Create(node.SyntaxTree, new TextSpan(node.SpanStart + 1, 1)));
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
}
