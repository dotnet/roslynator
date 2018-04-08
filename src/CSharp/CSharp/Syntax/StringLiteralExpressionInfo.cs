// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about string literal expression.
    /// </summary>
    public readonly struct StringLiteralExpressionInfo : IEquatable<StringLiteralExpressionInfo>
    {
        private StringLiteralExpressionInfo(LiteralExpressionSyntax expression)
        {
            Expression = expression;
        }

        private static StringLiteralExpressionInfo Default { get; } = new StringLiteralExpressionInfo();

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

        internal static StringLiteralExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true)
        {
            return Create(Walk(node, walkDownParentheses) as LiteralExpressionSyntax);
        }

        internal static StringLiteralExpressionInfo Create(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression?.Kind() != SyntaxKind.StringLiteralExpression)
                return Default;

            return new StringLiteralExpressionInfo(literalExpression);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Expression?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is StringLiteralExpressionInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(StringLiteralExpressionInfo other)
        {
            return EqualityComparer<LiteralExpressionSyntax>.Default.Equals(Expression, other.Expression);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return Expression?.GetHashCode() ?? 0;
        }

        public static bool operator ==(StringLiteralExpressionInfo info1, StringLiteralExpressionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(StringLiteralExpressionInfo info1, StringLiteralExpressionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
