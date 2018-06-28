// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about string concatenation, i.e. a binary expression that binds to string '+' operator.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct StringConcatenationExpressionInfo : IEquatable<StringConcatenationExpressionInfo>
    {
        private readonly ExpressionChain _chain;

        private StringConcatenationExpressionInfo(in ExpressionChain chain)
        {
            _chain = chain;
        }

        /// <summary>
        /// The binary expression that represents the string concatenation.
        /// </summary>
        public BinaryExpressionSyntax BinaryExpression
        {
            get { return _chain.BinaryExpression; }
        }

        internal TextSpan? Span
        {
            get { return _chain.Span; }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return BinaryExpression != null; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return ToDebugString(Success, this, BinaryExpression, ToString()); }
        }

        internal StringConcatenationAnalysis Analyze()
        {
            return StringConcatenationAnalysis.Create(this);
        }

        internal static StringConcatenationExpressionInfo Create(
            SyntaxNode node,
            SemanticModel semanticModel,
            bool walkDownParentheses = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Create(
                Walk(node, walkDownParentheses) as BinaryExpressionSyntax,
                semanticModel,
                cancellationToken);
        }

        internal static StringConcatenationExpressionInfo Create(
            BinaryExpressionSyntax binaryExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionChain chain = binaryExpression.AsChain();

            if (!chain.Reverse().IsStringConcatenation(semanticModel, cancellationToken))
                return default;

            return new StringConcatenationExpressionInfo(chain);
        }

        internal static StringConcatenationExpressionInfo Create(
            in ExpressionChain chain,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!chain.Reverse().IsStringConcatenation(semanticModel, cancellationToken))
                return default;

            return new StringConcatenationExpressionInfo(chain);
        }

        /// <summary>
        /// Returns expressions of this binary expression, including expressions of nested binary expressions of the same kind as parent binary expression.
        /// </summary>
        /// <param name="leftToRight">If true expressions are enumerated as they are displayed in the source code.</param>
        /// <returns></returns>
        [Obsolete("This method is obsolete. Use method 'AsChain' instead.")]
        public IEnumerable<ExpressionSyntax> Expressions(bool leftToRight = false)
        {
            ThrowInvalidOperationIfNotInitialized();

            if (leftToRight)
            {
                return _chain;
            }
            else
            {
                return _chain.Reverse();
            }
        }

        public ExpressionChain AsChain()
        {
            return _chain;
        }

        internal InterpolatedStringExpressionSyntax ToInterpolatedStringExpression()
        {
            ThrowInvalidOperationIfNotInitialized();

            StringBuilder sb = StringBuilderCache.GetInstance();

            var builder = new StringTextBuilder(sb, isVerbatim: !Analyze().ContainsNonVerbatimExpression, isInterpolated: true);

            builder.AppendStart();

            foreach (ExpressionSyntax expression in _chain)
            {
                switch (expression.Kind())
                {
                    case SyntaxKind.StringLiteralExpression:
                        {
                            builder.Append((LiteralExpressionSyntax)expression);
                            break;
                        }
                    case SyntaxKind.InterpolatedStringExpression:
                        {
                            builder.Append((InterpolatedStringExpressionSyntax)expression);
                            break;
                        }
                    default:
                        {
                            sb.Append('{');
                            sb.Append(expression.ToString());
                            sb.Append('}');
                            break;
                        }
                }
            }

            builder.AppendEnd();

            return (InterpolatedStringExpressionSyntax)ParseExpression(StringBuilderCache.GetStringAndFree(sb));
        }

        internal LiteralExpressionSyntax ToStringLiteralExpression()
        {
            ThrowInvalidOperationIfNotInitialized();

            StringConcatenationAnalysis analysis = Analyze();

            ThrowIfContainsNonStringLiteralExpression(analysis);

            StringBuilder sb = StringBuilderCache.GetInstance();

            var builder = new StringTextBuilder(sb, isVerbatim: !analysis.ContainsNonVerbatimExpression);

            builder.AppendStart();

            foreach (ExpressionSyntax expression in _chain)
                builder.Append((LiteralExpressionSyntax)expression);

            builder.AppendEnd();

            return (LiteralExpressionSyntax)ParseExpression(StringBuilderCache.GetStringAndFree(sb));
        }

        internal LiteralExpressionSyntax ToMultiLineStringLiteralExpression()
        {
            ThrowInvalidOperationIfNotInitialized();

            ThrowIfContainsNonStringLiteralExpression(Analyze());

            StringBuilder sb = StringBuilderCache.GetInstance();

            var builder = new StringTextBuilder(sb, isVerbatim: true);

            builder.AppendStart();

            ExpressionChain.Enumerator en = _chain.GetEnumerator();

            if (en.MoveNext())
            {
                while (true)
                {
                    ExpressionSyntax expression = en.Current;

                    int length = sb.Length;

                    builder.Append((LiteralExpressionSyntax)expression);

                    bool isLast = !en.MoveNext();

                    if (sb.Length > length
                        && sb[sb.Length - 1] == '\n')
                    {
                        sb.Remove(sb.Length - 1, 1);

                        if (sb.Length - length > 1
                            && sb[sb.Length - 1] == '\r')
                        {
                            sb.Remove(sb.Length - 1, 1);
                        }

                        sb.AppendLine();
                    }
                    else if (!isLast)
                    {
                        TextSpan span = TextSpan.FromBounds(expression.Span.End, en.Current.SpanStart);

                        if (BinaryExpression.SyntaxTree.IsMultiLineSpan(span))
                            sb.AppendLine();
                    }

                    if (isLast)
                        break;
                }
            }

            builder.AppendEnd();

            return (LiteralExpressionSyntax)ParseExpression(StringBuilderCache.GetStringAndFree(sb));
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _chain.ToString();
        }

        private void ThrowInvalidOperationIfNotInitialized()
        {
            if (BinaryExpression == null)
                throw new InvalidOperationException($"{nameof(StringConcatenationExpressionInfo)} is not initalized.");
        }

        private static void ThrowIfContainsNonStringLiteralExpression(in StringConcatenationAnalysis analysis)
        {
            if (analysis.ContainsNonStringLiteral)
                throw new InvalidOperationException("String concatenation contains an expression that is not a string literal.");
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is StringConcatenationExpressionInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(StringConcatenationExpressionInfo other)
        {
            return EqualityComparer<BinaryExpressionSyntax>.Default.Equals(BinaryExpression, other.BinaryExpression)
                && EqualityComparer<TextSpan?>.Default.Equals(Span, other.Span);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return Hash.Combine(Span.GetHashCode(), Hash.Create(BinaryExpression));
        }

        public static bool operator ==(in StringConcatenationExpressionInfo info1, in StringConcatenationExpressionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in StringConcatenationExpressionInfo info1, in StringConcatenationExpressionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
