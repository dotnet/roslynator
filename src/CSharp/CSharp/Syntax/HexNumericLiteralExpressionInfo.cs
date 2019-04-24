// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct HexNumericLiteralExpressionInfo
    {
        private HexNumericLiteralExpressionInfo(LiteralExpressionSyntax literalExpression, SyntaxToken token)
        {
            LiteralExpression = literalExpression;
            Token = token;
        }

        public LiteralExpressionSyntax LiteralExpression { get; }

        public SyntaxToken Token { get; }

        public string Text
        {
            get { return Token.Text; }
        }

        public string ValueText
        {
            get { return Token.ValueText; }
        }

        public object Value
        {
            get { return Token.Value; }
        }

        public string GetSuffix()
        {
            int startIndex = 0;

            for (int i = Text.Length - 1; i >= 0; i--)
            {
                char ch = Text[i];

                if (ch == 'u'
                    || ch == 'U'
                    || ch == 'l'
                    || ch == 'L')
                {
                    startIndex = i;
                }
                else
                {
                    break;
                }
            }

            return Text.Substring(startIndex);
        }

        public HexNumericLiteralSuffixKind GetSuffixKind()
        {
            string suffix = GetSuffix();

            if (suffix == null)
                return HexNumericLiteralSuffixKind.None;

            if (string.Equals(GetSuffix(), "u", StringComparison.OrdinalIgnoreCase))
                return HexNumericLiteralSuffixKind.UIntOrULong;

            if (string.Equals(GetSuffix(), "l", StringComparison.OrdinalIgnoreCase))
                return HexNumericLiteralSuffixKind.LongOrULong;

            if (string.Equals(GetSuffix(), "ul", StringComparison.OrdinalIgnoreCase)
                || string.Equals(GetSuffix(), "lu", StringComparison.OrdinalIgnoreCase))
            {
                return HexNumericLiteralSuffixKind.ULong;
            }

            return HexNumericLiteralSuffixKind.Unknown;
        }

        public bool Success
        {
            get { return LiteralExpression != null; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, LiteralExpression); }
        }

        internal static HexNumericLiteralExpressionInfo Create(SyntaxNode node, bool walkDownParentheses = true)
        {
            return Create(Walk(node, walkDownParentheses) as LiteralExpressionSyntax);
        }

        internal static HexNumericLiteralExpressionInfo Create(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                return default;

            if (!literalExpression.IsKind(SyntaxKind.NumericLiteralExpression))
                return default;

            SyntaxToken token = literalExpression.Token;

            string text = token.Text;

            if (!text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                return default;

            return new HexNumericLiteralExpressionInfo(literalExpression, token);
        }
    }
}