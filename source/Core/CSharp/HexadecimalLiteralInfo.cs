// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    internal class HexadecimalLiteralInfo
    {
        private string _suffix;

        private HexadecimalLiteralInfo(LiteralExpressionSyntax literalExpression, SyntaxToken token, string text)
        {
            LiteralExpression = literalExpression;
            Token = token;
            Text = text;
        }

        public string Prefix
        {
            get { return Text.Remove(2); }
        }

        public string Suffix
        {
            get { return _suffix ?? (_suffix = ParseSuffix()); }
        }

        public bool HasSuffix
        {
            get { return Suffix != null; }
        }

        public HexadecimalLiteralSuffixKind SuffixKind
        {
            get
            {
                if (HasSuffix)
                {
                    if (string.Equals(Suffix, "u", StringComparison.OrdinalIgnoreCase))
                    {
                        return HexadecimalLiteralSuffixKind.UInt32OrUInt64;
                    }
                    else if (string.Equals(Suffix, "l", StringComparison.OrdinalIgnoreCase))
                    {
                        return HexadecimalLiteralSuffixKind.Int64OrUInt64;
                    }
                    else if (string.Equals(Suffix, "ul", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Suffix, "lu", StringComparison.OrdinalIgnoreCase))
                    {
                        return HexadecimalLiteralSuffixKind.UInt64;
                    }
                    else
                    {
                        return HexadecimalLiteralSuffixKind.Unknown;
                    }
                }
                else
                {
                    return HexadecimalLiteralSuffixKind.None;
                }
            }
        }

        public object Value
        {
            get { return LiteralExpression.Token.Value; }
        }

        public LiteralExpressionSyntax LiteralExpression { get; }

        public SyntaxToken Token { get; }

        public string Text { get; }

        public static bool TryCreate(LiteralExpressionSyntax literalExpression, out HexadecimalLiteralInfo info)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            if (literalExpression.IsKind(SyntaxKind.NumericLiteralExpression))
            {
                SyntaxToken token = literalExpression.Token;
                string text = token.Text;

                if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    info = new HexadecimalLiteralInfo(literalExpression, token, text);
                    return true;
                }
            }

            info = null;
            return false;
        }

        internal LiteralExpressionSyntax ToDecimalLiteral()
        {
            return LiteralExpression(Value);
        }

        private string ParseSuffix()
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
    }
}