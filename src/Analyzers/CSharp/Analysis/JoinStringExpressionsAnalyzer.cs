// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class JoinStringExpressionsAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.JoinStringExpressions); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeAddExpression, SyntaxKind.AddExpression);
        }

        public static void AnalyzeAddExpression(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (node.ContainsDiagnostics)
                return;

            if (node.SpanContainsDirectives())
                return;

            if (node.IsParentKind(SyntaxKind.AddExpression))
                return;

            var addExpression = (BinaryExpressionSyntax)node;

            ExpressionSyntax firstExpression = null;
            ExpressionSyntax lastExpression = null;
            bool isLiteral = false;
            bool isVerbatim = false;

            foreach (ExpressionSyntax expression in addExpression.AsChain().Reverse())
            {
                context.ThrowIfCancellationRequested();

                switch (expression.Kind())
                {
                    case SyntaxKind.StringLiteralExpression:
                        {
                            StringLiteralExpressionInfo stringLiteral = SyntaxInfo.StringLiteralExpressionInfo(expression);

                            bool isVerbatim2 = stringLiteral.IsVerbatim;

                            if (firstExpression == null)
                            {
                                firstExpression = expression;
                                isLiteral = true;
                                isVerbatim = isVerbatim2;
                            }
                            else if (!isLiteral
                                || isVerbatim != isVerbatim2
                                || (!isVerbatim && !CheckHexadecimalEscapeSequence(stringLiteral)))
                            {
                                if (lastExpression != null)
                                    Analyze(context, firstExpression, lastExpression, isVerbatim);

                                firstExpression = null;
                                lastExpression = null;
                            }
                            else
                            {
                                lastExpression = expression;
                            }

                            break;
                        }
                    case SyntaxKind.InterpolatedStringExpression:
                        {
                            var interpolatedString = ((InterpolatedStringExpressionSyntax)expression);

                            bool isVerbatim2 = interpolatedString.IsVerbatim();

                            if (firstExpression == null)
                            {
                                firstExpression = expression;
                                isLiteral = false;
                                isVerbatim = isVerbatim2;
                            }
                            else if (isLiteral
                                || isVerbatim != isVerbatim2
                                || (!isVerbatim && !CheckHexadecimalEscapeSequence(interpolatedString)))
                            {
                                if (lastExpression != null)
                                    Analyze(context, firstExpression, lastExpression, isVerbatim);

                                firstExpression = null;
                                lastExpression = null;
                            }
                            else
                            {
                                lastExpression = expression;
                            }

                            break;
                        }
                    default:
                        {
                            if (lastExpression != null)
                                Analyze(context, firstExpression, lastExpression, isVerbatim);

                            firstExpression = null;
                            lastExpression = null;
                            break;
                        }
                }
            }

            if (lastExpression != null)
                Analyze(context, firstExpression, lastExpression, isVerbatim);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, ExpressionSyntax firstExpression, ExpressionSyntax lastExpression, bool isVerbatim)
        {
            CancellationToken cancellationToken = context.CancellationToken;

            TextSpan span = TextSpan.FromBounds(lastExpression.SpanStart, firstExpression.Span.End);

            if (span != firstExpression.Parent.Span)
            {
                var addExpression = (BinaryExpressionSyntax)lastExpression.Parent;

                cancellationToken.ThrowIfCancellationRequested();

                if (!CSharpUtility.IsStringConcatenation(addExpression, context.SemanticModel, cancellationToken))
                    return;
            }

            SyntaxTree tree = firstExpression.SyntaxTree;

            if (isVerbatim
                || tree.IsSingleLineSpan(span, cancellationToken))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.JoinStringExpressions,
                    Location.Create(tree, span));
            }
        }

        private static bool CheckHexadecimalEscapeSequence(in StringLiteralExpressionInfo stringLiteral)
        {
            string text = stringLiteral.Text;

            return CheckHexadecimalEscapeSequence(text, 1, text.Length - 2);
        }

        private static bool CheckHexadecimalEscapeSequence(InterpolatedStringExpressionSyntax interpolatedString)
        {
            InterpolatedStringContentSyntax content = interpolatedString.Contents.LastOrDefault();

            if (content.IsKind(SyntaxKind.InterpolatedStringText))
            {
                var interpolatedStringText = (InterpolatedStringTextSyntax)content;

                string text = interpolatedStringText.TextToken.Text;

                return CheckHexadecimalEscapeSequence(text, 0, text.Length);
            }

            return true;
        }

        private static bool CheckHexadecimalEscapeSequence(string text, int start, int length)
        {
            for (int pos = start; pos < start + length; pos++)
            {
                if (text[pos] == '\\')
                {
                    pos++;

                    if (pos >= text.Length)
                        return false;

                    switch (text[pos])
                    {
                        case '\'':
                        case '\"':
                        case '\\':
                        case '0':
                        case 'a':
                        case 'b':
                        case 'f':
                        case 'n':
                        case 'r':
                        case 't':
                        case 'v':
                            {
                                break;
                            }
                        case 'u':
                            {
                                pos += 4;
                                break;
                            }
                        case 'U':
                            {
                                pos += 8;
                                break;
                            }
                        case 'x':
                            {
                                pos++;

                                if (pos < text.Length
                                    && StringUtility.IsHexadecimalDigit(text[pos]))
                                {
                                    pos++;
                                    if (pos < text.Length
                                        && StringUtility.IsHexadecimalDigit(text[pos]))
                                    {
                                        pos++;
                                        if (pos < text.Length
                                            && StringUtility.IsHexadecimalDigit(text[pos]))
                                        {
                                            pos++;
                                            if (pos < text.Length
                                                && StringUtility.IsHexadecimalDigit(text[pos]))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (pos == start + length)
                                    return false;

                                pos--;
                                break;
                            }
                        default:
                            {
                                Debug.Fail(text[pos].ToString());
                                return false;
                            }
                    }
                }
            }

            return true;
        }
    }
}
