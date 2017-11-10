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
        private InterpolatedStringContentConversion(string name, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }

        public SeparatedSyntaxList<ArgumentSyntax> Arguments { get; }

        public static InterpolatedStringContentConversion Create(InterpolatedStringContentSyntax content, bool isVerbatim)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            if (!TryCreate(content, isVerbatim, out InterpolatedStringContentConversion conversion))
                throw new ArgumentException("", nameof(content));

            return conversion;
        }

        public static bool TryCreate(InterpolatedStringContentSyntax content, bool isVerbatim, out InterpolatedStringContentConversion conversion)
        {
            switch (content?.Kind())
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

                            conversion = new InterpolatedStringContentConversion("AppendFormat", SeparatedList(new ArgumentSyntax[] { Argument(ParseExpression(sb.ToString())), Argument(interpolation.Expression) }));
                            return true;
                        }
                        else
                        {
                            conversion = new InterpolatedStringContentConversion("Append", SingletonSeparatedList(Argument(interpolation.Expression)));
                            return true;
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

                        conversion = new InterpolatedStringContentConversion("Append", SingletonSeparatedList(Argument(stringLiteral)));
                        return true;
                    }
            }

            conversion = default(InterpolatedStringContentConversion);
            return false;
        }
    }
}
