// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertInterpolatedStringToStringBuilderMethodRefactoring
    {
        public static (SyntaxKind contentKind, string methodName, ImmutableArray<ArgumentSyntax> arguments)
            Refactor(InterpolatedStringContentSyntax content, bool isVerbatim)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            SyntaxKind kind = content.Kind();

            switch (kind)
            {
                case SyntaxKind.Interpolation:
                    {
                        var interpolation = (InterpolationSyntax)content;

                        InterpolationAlignmentClauseSyntax alignmentClause = interpolation.AlignmentClause;
                        InterpolationFormatClauseSyntax formatClause = interpolation.FormatClause;

                        if (alignmentClause != null
                            || formatClause != null)
                        {
                            StringBuilder sb = StringBuilderCache.GetInstance();

                            sb.Append("\"{0");

                            if (alignmentClause != null)
                            {
                                sb.Append(',');
                                sb.Append(alignmentClause.Value.ToString());
                            }

                            if (formatClause != null)
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

                        text = (isVerbatim)
                            ? "@\"" + text + "\""
                            : "\"" + text + "\"";

                        ExpressionSyntax stringLiteral = ParseExpression(text);

                        return (kind, "Append", ImmutableArray.Create(Argument(stringLiteral)));
                    }
                default:
                    {
                        throw new ArgumentException("", nameof(content));
                    }
            }
        }
    }
}
