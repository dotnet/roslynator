// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Utilities;

namespace Roslynator.CSharp
{
    internal struct StringExpressionChain
    {
        private StringExpressionChain(BinaryExpressionSyntax addExpression, IEnumerable<ExpressionSyntax> expressions, TextSpan? span = null)
        {
            ContainsNonSpecificExpression = false;
            ContainsRegularLiteralExpression = false;
            ContainsVerbatimLiteralExpression = false;
            ContainsRegularInterpolatedStringExpression = false;
            ContainsVerbatimInterpolatedStringExpression = false;

            OriginalExpression = addExpression;
            Expressions = ImmutableArray.CreateRange(expressions);
            Span = span;

            foreach (ExpressionSyntax expression in expressions)
            {
                SyntaxKind kind = expression.Kind();

                if (kind == SyntaxKind.StringLiteralExpression)
                {
                    if (((LiteralExpressionSyntax)expression).IsVerbatimStringLiteral())
                    {
                        ContainsVerbatimLiteralExpression = true;
                    }
                    else
                    {
                        ContainsRegularLiteralExpression = true;
                    }
                }
                else
                {
                    if (kind == SyntaxKind.InterpolatedStringExpression)
                    {
                        if (((InterpolatedStringExpressionSyntax)expression).IsVerbatim())
                        {
                            ContainsVerbatimInterpolatedStringExpression = true;
                        }
                        else
                        {
                            ContainsRegularInterpolatedStringExpression = true;
                        }
                    }
                    else
                    {
                        ContainsNonSpecificExpression = true;
                    }
                }
            }
        }

        public ImmutableArray<ExpressionSyntax> Expressions { get; }

        public BinaryExpressionSyntax OriginalExpression { get; }

        public TextSpan? Span { get; }

        public bool ContainsNonSpecificExpression { get; }

        public bool ContainsNonLiteralExpression
        {
            get { return ContainsInterpolatedStringExpression || ContainsNonSpecificExpression; }
        }

        public bool ContainsLiteralExpression
        {
            get { return ContainsRegularLiteralExpression || ContainsVerbatimLiteralExpression; }
        }

        public bool ContainsRegularLiteralExpression { get; }

        public bool ContainsVerbatimLiteralExpression { get; }

        public bool ContainsInterpolatedStringExpression
        {
            get { return ContainsRegularInterpolatedStringExpression || ContainsVerbatimInterpolatedStringExpression; }
        }

        public bool ContainsRegularInterpolatedStringExpression { get; }

        public bool ContainsVerbatimInterpolatedStringExpression { get; }

        public bool ContainsRegular
        {
            get { return ContainsRegularLiteralExpression || ContainsRegularInterpolatedStringExpression; }
        }

        public bool ContainsVerbatim
        {
            get { return ContainsVerbatimLiteralExpression || ContainsVerbatimInterpolatedStringExpression; }
        }

        public static bool TryCreate(
            BinaryExpressionSyntax binaryExpression,
            SemanticModel semanticModel,
            out StringExpressionChain chain)
        {
            return TryCreate(binaryExpression, semanticModel, default(CancellationToken), out chain);
        }

        public static bool TryCreate(
            BinaryExpressionSyntax binaryExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            out StringExpressionChain chain)
        {
            if (binaryExpression.IsKind(SyntaxKind.AddExpression))
            {
                List<ExpressionSyntax> expressions = GetExpressions(binaryExpression, semanticModel, cancellationToken);

                if (expressions != null)
                {
                    expressions.Reverse();
                    chain = new StringExpressionChain(binaryExpression, expressions);
                    return true;
                }
            }

            chain = default(StringExpressionChain);
            return false;
        }

        public static bool TryCreate(
            BinaryExpressionSelection binaryExpressionSelection,
            SemanticModel semanticModel,
            out StringExpressionChain chain)
        {
            return TryCreate(binaryExpressionSelection, semanticModel, default(CancellationToken), out chain);
        }

        public static bool TryCreate(
            BinaryExpressionSelection binaryExpressionSelection,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            out StringExpressionChain chain)
        {
            BinaryExpressionSyntax binaryExpression = binaryExpressionSelection.BinaryExpression;
            ImmutableArray<ExpressionSyntax> expressions = binaryExpressionSelection.Expressions;

            if (binaryExpression.IsKind(SyntaxKind.AddExpression)
                && expressions.All(expression => IsStringExpression(expression, semanticModel, cancellationToken)))
            {
                chain = new StringExpressionChain(binaryExpression, expressions, binaryExpressionSelection.Span);
                return true;
            }

            chain = default(StringExpressionChain);
            return false;
        }

        private static bool IsStringExpression(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return expression != null
                && (expression.IsKind(SyntaxKind.StringLiteralExpression)
                    || IsString(expression, semanticModel, cancellationToken));
        }

        private static List<ExpressionSyntax> GetExpressions(
            BinaryExpressionSyntax binaryExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            List<ExpressionSyntax> expressions = null;
            bool success = true;

            while (success)
            {
                success = false;

                if (binaryExpression.IsKind(SyntaxKind.AddExpression))
                {
                    ExpressionSyntax right = binaryExpression.Right;

                    if (right?.IsMissing == false)
                    {
                        if (right.IsKind(SyntaxKind.StringLiteralExpression, SyntaxKind.InterpolatedStringExpression)
                            || IsString(right, semanticModel, cancellationToken))
                        {
                            (expressions ?? (expressions = new List<ExpressionSyntax>())).Add(right);

                            ExpressionSyntax left = binaryExpression.Left;

                            if (left?.IsMissing == false)
                            {
                                switch (left.Kind())
                                {
                                    case SyntaxKind.StringLiteralExpression:
                                    case SyntaxKind.InterpolatedStringExpression:
                                        {
                                            expressions.Add(left);
                                            return expressions;
                                        }
                                    case SyntaxKind.AddExpression:
                                        {
                                            binaryExpression = (BinaryExpressionSyntax)left;
                                            success = true;
                                            break;
                                        }
                                    default:
                                        {
                                            if (IsString(left, semanticModel, cancellationToken))
                                            {
                                                expressions.Add(left);
                                                return expressions;
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public InterpolatedStringExpressionSyntax ToInterpolatedString()
        {
            var sb = new StringBuilder();

            sb.Append('$');

            if (!ContainsRegular)
                sb.Append('@');

            sb.Append('"');

            for (int i = 0; i < Expressions.Length; i++)
            {
                SyntaxKind kind = Expressions[i].Kind();

                if (kind == SyntaxKind.StringLiteralExpression)
                {
                    var literal = (LiteralExpressionSyntax)Expressions[i];

                    if (ContainsRegular
                        && literal.IsVerbatimStringLiteral())
                    {
                        string s = literal.Token.ValueText;
                        s = StringUtility.DoubleBackslash(s);
                        s = StringUtility.EscapeQuote(s);
                        s = StringUtility.DoubleBraces(s);
                        s = s.Replace("\n", @"\n");
                        s = s.Replace("\r", @"\r");
                        sb.Append(s);
                    }
                    else
                    {
                        string s = GetInnerText(literal.Token.Text);
                        s = StringUtility.DoubleBraces(s);
                        sb.Append(s);
                    }
                }
                else if (kind == SyntaxKind.InterpolatedStringExpression)
                {
                    var interpolatedString = (InterpolatedStringExpressionSyntax)Expressions[i];

                    bool isVerbatimInterpolatedString = interpolatedString.IsVerbatim();

                    foreach (InterpolatedStringContentSyntax content in interpolatedString.Contents)
                    {
                        Debug.Assert(content.IsKind(SyntaxKind.Interpolation, SyntaxKind.InterpolatedStringText), content.Kind().ToString());

                        switch (content.Kind())
                        {
                            case SyntaxKind.InterpolatedStringText:
                                {
                                    var text = (InterpolatedStringTextSyntax)content;

                                    if (ContainsRegular
                                        && isVerbatimInterpolatedString)
                                    {
                                        string s = text.TextToken.ValueText;
                                        s = StringUtility.DoubleBackslash(s);
                                        s = StringUtility.EscapeQuote(s);
                                        s = s.Replace("\n", @"\n");
                                        s = s.Replace("\r", @"\r");
                                        sb.Append(s);
                                    }
                                    else
                                    {
                                        sb.Append(content.ToString());
                                    }

                                    break;
                                }
                            case SyntaxKind.Interpolation:
                                {
                                    sb.Append(content.ToString());
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    sb.Append('{');
                    sb.Append(Expressions[i].ToString());
                    sb.Append('}');
                }
            }

            sb.Append("\"");

            return (InterpolatedStringExpressionSyntax)SyntaxFactory.ParseExpression(sb.ToString());
        }

        public LiteralExpressionSyntax ToStringLiteral()
        {
            if (ContainsNonLiteralExpression)
                throw new InvalidOperationException();

            var sb = new StringBuilder();

            if (!ContainsRegular)
                sb.Append('@');

            sb.Append('"');

            foreach (ExpressionSyntax expression in Expressions)
            {
                if (expression.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    var literal = (LiteralExpressionSyntax)expression;

                    if (ContainsRegular
                        && literal.IsVerbatimStringLiteral())
                    {
                        string s = literal.Token.ValueText;
                        s = StringUtility.DoubleBackslash(s);
                        s = StringUtility.EscapeQuote(s);
                        s = s.Replace("\n", @"\n");
                        s = s.Replace("\r", @"\r");
                        sb.Append(s);
                    }
                    else
                    {
                        sb.Append(GetInnerText(literal.Token.Text));
                    }
                }
            }

            sb.Append('"');

            return (LiteralExpressionSyntax)SyntaxFactory.ParseExpression(sb.ToString());
        }

        public LiteralExpressionSyntax ToMultilineStringLiteral()
        {
            if (ContainsNonLiteralExpression)
                throw new InvalidOperationException();

            var sb = new StringBuilder();

            sb.Append('@');
            sb.Append('"');

            for (int i = 0; i < Expressions.Length; i++)
            {
                if (Expressions[i].IsKind(SyntaxKind.StringLiteralExpression))
                {
                    var literal = (LiteralExpressionSyntax)Expressions[i];

                    string s = StringUtility.DoubleQuote(literal.Token.ValueText);

                    int charCount = 0;

                    if (s.Length > 0
                        && s[s.Length - 1] == '\n')
                    {
                        charCount = 1;

                        if (s.Length > 1
                            && s[s.Length - 2] == '\r')
                        {
                            charCount = 2;
                        }
                    }

                    sb.Append(s, 0, s.Length - charCount);

                    if (charCount > 0)
                    {
                        sb.AppendLine();
                    }
                    else if (i < Expressions.Length - 1)
                    {
                        TextSpan span = TextSpan.FromBounds(Expressions[i].Span.End, Expressions[i + 1].SpanStart);

                        if (OriginalExpression.SyntaxTree.IsMultiLineSpan(span))
                            sb.AppendLine();
                    }
                }
            }

            sb.Append('"');

            return (LiteralExpressionSyntax)SyntaxFactory.ParseExpression(sb.ToString());
        }

        public override string ToString()
        {
            if (Span != null)
            {
                TextSpan span = Span.Value;

                return OriginalExpression
                    .ToString()
                    .Substring(span.Start - OriginalExpression.SpanStart, span.Length);
            }
            else
            {
                return OriginalExpression.ToString();
            }
        }

        private static string GetInnerText(string s)
        {
            return (s[0] == '@')
                ? s.Substring(2, s.Length - 3)
                : s.Substring(1, s.Length - 2);
        }

        private static bool IsString(ExpressionSyntax right, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return semanticModel
                .GetTypeInfo(right, cancellationToken)
                .ConvertedType?
                .IsString() == true;
        }
    }
}
