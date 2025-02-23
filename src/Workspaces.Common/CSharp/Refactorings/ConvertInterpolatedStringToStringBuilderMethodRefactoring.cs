﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings;

internal static class ConvertInterpolatedStringToStringBuilderMethodRefactoring
{
    public static (SyntaxKind contentKind, string methodName, ImmutableArray<ArgumentSyntax> arguments)
        Refactor(InterpolatedStringContentSyntax content, bool isVerbatim)
    {
        if (content is null)
            throw new ArgumentNullException(nameof(content));

        SyntaxKind kind = content.Kind();

        switch (kind)
        {
            case SyntaxKind.Interpolation:
            {
                var interpolation = (InterpolationSyntax)content;

                InterpolationAlignmentClauseSyntax alignmentClause = interpolation.AlignmentClause;
                InterpolationFormatClauseSyntax formatClause = interpolation.FormatClause;

                if (alignmentClause is not null
                    || formatClause is not null)
                {
                    StringBuilder sb = StringBuilderCache.GetInstance();

                    if (isVerbatim)
                        sb.Append('@');

                    sb.Append("\"{0");

                    if (alignmentClause is not null)
                    {
                        sb.Append(',');
                        sb.Append(alignmentClause.Value.ToString());
                    }

                    if (formatClause is not null)
                    {
                        sb.Append(':');
                        sb.Append(formatClause.FormatStringToken.Text);
                    }

                    sb.Append("}\"");

                    ExpressionSyntax expression = ParseExpression(StringBuilderCache.GetStringAndFree(sb));

                    return (kind, "AppendFormat", ImmutableArray.Create(Argument(expression), Argument(interpolation.Expression)));
                }
                else
                {
                    return (kind, "Append", ImmutableArray.Create(Argument(interpolation.Expression)));
                }
            }
            case SyntaxKind.InterpolatedStringText:
            {
                var interpolatedStringText = (InterpolatedStringTextSyntax)content;

                string text = interpolatedStringText.TextToken.Text;

                text = StringUtility.ReplaceDoubleBracesWithSingleBrace(text);

#if ROSLYN_4_2
                if (content.Parent is InterpolatedStringExpressionSyntax interpolatedStringExpression
                    && interpolatedStringExpression.StringStartToken.IsKind(SyntaxKind.InterpolatedSingleLineRawStringStartToken))
                {
                    text = interpolatedStringExpression.StringStartToken.ValueText.Substring(1)
                        + text
                        + interpolatedStringExpression.StringEndToken.ValueText;
                }
                else if (content.Parent is InterpolatedStringExpressionSyntax interpolatedStringExpression2
                    && interpolatedStringExpression2.StringStartToken.IsKind(SyntaxKind.InterpolatedMultiLineRawStringStartToken))
                {
                    text = interpolatedStringExpression2.StringStartToken.ValueText.Substring(1)
                        + text
                        + interpolatedStringExpression2.StringEndToken.ValueText;
                }
                else
#endif
                if (isVerbatim)
                {
                    text = "@\"" + text + "\"";
                }
                else
                {
                    text = "\"" + text + "\"";
                }

                ExpressionSyntax stringLiteral = ParseExpression(text).WithTriviaFrom(interpolatedStringText);

                return (kind, "Append", ImmutableArray.Create(Argument(stringLiteral)));
            }
            default:
            {
                throw new ArgumentException("", nameof(content));
            }
        }
    }
}
