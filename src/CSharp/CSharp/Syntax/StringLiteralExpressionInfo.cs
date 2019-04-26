// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about string literal expression.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct StringLiteralExpressionInfo
    {
        private StringLiteralExpressionInfo(LiteralExpressionSyntax expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// The string literal expression.
        /// </summary>
        public LiteralExpressionSyntax Expression { get; }

        /// <summary>
        /// The token representing the string literal expression.
        /// </summary>
        public SyntaxToken Token
        {
            get { return Expression?.Token ?? default(SyntaxToken); }
        }

        /// <summary>
        /// The token text.
        /// </summary>
        public string Text
        {
            get { return Token.Text; }
        }

        /// <summary>
        /// The token text, not including leading ampersand, if any, and enclosing quotation marks.
        /// </summary>
        public string InnerText
        {
            get
            {
                string text = Text;

                int length = text.Length;

                if (length == 0)
                    return "";

                if (text[0] == '@')
                    return text.Substring(2, length - 3);

                return text.Substring(1, length - 2);
            }
        }

        /// <summary>
        /// The token value text.
        /// </summary>
        public string ValueText
        {
            get { return Token.ValueText; }
        }

        /// <summary>
        /// True if this instance is regular string literal expression.
        /// </summary>
        public bool IsRegular
        {
            get { return Text.StartsWith("\"", StringComparison.Ordinal); }
        }

        /// <summary>
        /// True if this instance is verbatim string literal expression.
        /// </summary>
        public bool IsVerbatim
        {
            get { return Text.StartsWith("@", StringComparison.Ordinal); }
        }

        /// <summary>
        /// True if the string literal contains linefeed.
        /// </summary>
        public bool ContainsLinefeed
        {
            get
            {
                if (IsRegular)
                    return ValueText.Contains("\n");

                if (IsVerbatim)
                    return Text.Contains("\n");

                return false;
            }
        }

        /// <summary>
        /// True if the string literal expression contains escape sequence.
        /// </summary>
        public bool ContainsEscapeSequence
        {
            get
            {
                if (IsRegular)
                    return Text.Contains("\\");

                if (IsVerbatim)
                    return InnerText.Contains("\"\"");

                return false;
            }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return Expression != null; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, Expression); }
        }

        internal static StringLiteralExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true)
        {
            return Create(Walk(node, walkDownParentheses) as LiteralExpressionSyntax);
        }

        internal static StringLiteralExpressionInfo Create(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression?.Kind() != SyntaxKind.StringLiteralExpression)
                return default;

            return new StringLiteralExpressionInfo(literalExpression);
        }
    }
}
