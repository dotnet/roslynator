// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    public struct HexadecimalLiteralInfo
    {
        private HexadecimalLiteralInfo(LiteralExpressionSyntax literalExpression, SyntaxToken token)
        {
            LiteralExpression = literalExpression;
            Token = token;
        }

        private static HexadecimalLiteralInfo Default { get; } = new HexadecimalLiteralInfo();

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

        public string GetPrefix()
        {
            return Text.Remove(2);
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

        public HexadecimalLiteralSuffixKind GetSuffixKind()
        {
            string suffix = GetSuffix();

            if (suffix == null)
                return HexadecimalLiteralSuffixKind.None;

            if (string.Equals(GetSuffix(), "u", StringComparison.OrdinalIgnoreCase))
                return HexadecimalLiteralSuffixKind.UInt32OrUInt64;

            if (string.Equals(GetSuffix(), "l", StringComparison.OrdinalIgnoreCase))
                return HexadecimalLiteralSuffixKind.Int64OrUInt64;

            if (string.Equals(GetSuffix(), "ul", StringComparison.OrdinalIgnoreCase)
                || string.Equals(GetSuffix(), "lu", StringComparison.OrdinalIgnoreCase))
            {
                return HexadecimalLiteralSuffixKind.UInt64;
            }

            return HexadecimalLiteralSuffixKind.Unknown;
        }

        public bool Success
        {
            get { return LiteralExpression != null; }
        }

        internal static HexadecimalLiteralInfo Create(SyntaxNode node, bool walkDownParentheses = true)
        {
            return Create(Walk(node, walkDownParentheses) as LiteralExpressionSyntax);
        }

        internal static HexadecimalLiteralInfo Create(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                return Default;

            if (!literalExpression.IsKind(SyntaxKind.NumericLiteralExpression))
                return Default;

            SyntaxToken token = literalExpression.Token;

            string text = token.Text;

            if (!text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                return Default;

            return new HexadecimalLiteralInfo(literalExpression, token);
        }
    }
}