// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    internal readonly struct HexNumericLiteralExpressionInfo : IEquatable<HexNumericLiteralExpressionInfo>
    {
        private HexNumericLiteralExpressionInfo(LiteralExpressionSyntax literalExpression, SyntaxToken token)
        {
            LiteralExpression = literalExpression;
            Token = token;
        }

        private static HexNumericLiteralExpressionInfo Default { get; } = new HexNumericLiteralExpressionInfo();

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

        internal static HexNumericLiteralExpressionInfo Create(SyntaxNode node, bool walkDownParentheses = true)
        {
            return Create(Walk(node, walkDownParentheses) as LiteralExpressionSyntax);
        }

        internal static HexNumericLiteralExpressionInfo Create(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                return Default;

            if (!literalExpression.IsKind(SyntaxKind.NumericLiteralExpression))
                return Default;

            SyntaxToken token = literalExpression.Token;

            string text = token.Text;

            if (!text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                return Default;

            return new HexNumericLiteralExpressionInfo(literalExpression, token);
        }

        public override string ToString()
        {
            return LiteralExpression?.ToString() ?? "";
        }

        public override bool Equals(object obj)
        {
            return obj is HexNumericLiteralExpressionInfo other && Equals(other);
        }

        public bool Equals(HexNumericLiteralExpressionInfo other)
        {
            return EqualityComparer<LiteralExpressionSyntax>.Default.Equals(LiteralExpression, other.LiteralExpression);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<LiteralExpressionSyntax>.Default.GetHashCode(LiteralExpression);
        }

        public static bool operator ==(HexNumericLiteralExpressionInfo info1, HexNumericLiteralExpressionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(HexNumericLiteralExpressionInfo info1, HexNumericLiteralExpressionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}