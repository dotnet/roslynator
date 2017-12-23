// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    internal struct InterpolatedStringContentConversion
    {
        private InterpolatedStringContentConversion(
            SyntaxKind kind,
            string methodName,
            SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            Kind = kind;
            MethodName = methodName;
            Arguments = arguments;
        }

        public SyntaxKind Kind { get; }

        public string MethodName { get; }

        public SeparatedSyntaxList<ArgumentSyntax> Arguments { get; }

        public static InterpolatedStringContentConversion Create(InterpolatedStringContentSyntax content, bool isVerbatim)
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
                            var sb = new StringBuilder();
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

                            return new InterpolatedStringContentConversion(kind,"AppendFormat", SeparatedList(new ArgumentSyntax[] { Argument(ParseExpression(sb.ToString())), Argument(interpolation.Expression) }));
                        }
                        else
                        {
                            return new InterpolatedStringContentConversion(kind ,"Append", SingletonSeparatedList(Argument(interpolation.Expression)));
                        }
                    }
                case SyntaxKind.InterpolatedStringText:
                    {
                        var interpolatedStringText = (InterpolatedStringTextSyntax)content;

                        string text = interpolatedStringText.TextToken.Text;

                        text = (isVerbatim)
                            ? "@\"" + text + "\""
                            : "\"" + text + "\"";

                        ExpressionSyntax stringLiteral = ParseExpression(text);

                        return new InterpolatedStringContentConversion(kind, "Append", SingletonSeparatedList(Argument(stringLiteral)));
                    }
                default:
                    {
                        throw new ArgumentException("", nameof(content));
                    }
            }
        }
    }
}
