// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a null check expression.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct NullCheckExpressionInfo : IEquatable<NullCheckExpressionInfo>
    {
        private NullCheckExpressionInfo(
            ExpressionSyntax containingExpression,
            ExpressionSyntax expression,
            NullCheckStyles style)
        {
            NullCheckExpression = containingExpression;
            Expression = expression;
            Style = style;
        }

        /// <summary>
        /// The null check expression, e.g. "x == null".
        /// </summary>
        public ExpressionSyntax NullCheckExpression { get; }

        /// <summary>
        /// The expression that is evaluated whether is (not) null. for example "x" in "x == null".
        /// </summary>
        public ExpressionSyntax Expression { get; }

        /// <summary>
        /// The style of this null check.
        /// </summary>
        public NullCheckStyles Style { get; }

        /// <summary>
        /// Determines whether this null check is checking if the expression is null.
        /// </summary>
        public bool IsCheckingNull
        {
            get { return (Style & NullCheckStyles.CheckingNull) != 0; }
        }

        /// <summary>
        /// Determines whether this null check is checking if the expression is not null.
        /// </summary>
        public bool IsCheckingNotNull
        {
            get { return (Style & NullCheckStyles.CheckingNotNull) != 0; }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return Style != NullCheckStyles.None; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return (Success)
                    ? $"{GetType().Name} {Style} {Expression} {NullCheckExpression}"
                    : "Uninitialized";
            }
        }

        internal static NullCheckExpressionInfo Create(
            SyntaxNode node,
            NullCheckStyles allowedStyles = NullCheckStyles.ComparisonToNull | NullCheckStyles.IsPattern,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if ((allowedStyles & NullCheckStyles.HasValue) != 0)
                throw new ArgumentException($"'{nameof(NullCheckStyles.HasValue)}' style requires a SemanticModel to be provided.", nameof(allowedStyles));

            if ((allowedStyles & NullCheckStyles.NotHasValue) != 0)
                throw new ArgumentException($"'{nameof(NullCheckStyles.NotHasValue)}' style requires a SemanticModel to be provided.", nameof(allowedStyles));

            return CreateImpl(node, default(SemanticModel), allowedStyles, walkDownParentheses, allowMissing, default(CancellationToken));
        }

        internal static NullCheckExpressionInfo Create(
            SyntaxNode node,
            SemanticModel semanticModel,
            NullCheckStyles allowedStyles = NullCheckStyles.All,
            bool walkDownParentheses = true,
            bool allowMissing = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return CreateImpl(node, semanticModel, allowedStyles, walkDownParentheses, allowMissing, cancellationToken);
        }

        private static NullCheckExpressionInfo CreateImpl(
            SyntaxNode node,
            SemanticModel semanticModel,
            NullCheckStyles allowedStyles,
            bool walkDownParentheses,
            bool allowMissing,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = WalkAndCheck(node, walkDownParentheses, allowMissing);

            if (expression == null)
                return default;

            SyntaxKind kind = expression.Kind();

            switch (kind)
            {
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)expression;

                        ExpressionSyntax left = WalkAndCheck(binaryExpression.Left, walkDownParentheses, allowMissing);

                        if (left == null)
                            break;

                        ExpressionSyntax right = WalkAndCheck(binaryExpression.Right, walkDownParentheses, allowMissing);

                        if (right == null)
                            break;

                        NullCheckExpressionInfo info = Create(binaryExpression, kind, left, right, allowedStyles, allowMissing, semanticModel, cancellationToken);

                        if (info.Success)
                        {
                            return info;
                        }
                        else
                        {
                            return Create(binaryExpression, kind, right, left, allowedStyles, allowMissing, semanticModel, cancellationToken);
                        }
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        if ((allowedStyles & NullCheckStyles.HasValue) == 0)
                            break;

                        var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

                        if (!IsPropertyOfNullableOfT(memberAccessExpression.Name, "HasValue", semanticModel, cancellationToken))
                            break;

                        return new NullCheckExpressionInfo(expression, memberAccessExpression.Expression, NullCheckStyles.HasValue);
                    }
                case SyntaxKind.IsPatternExpression:
                    {
                        var isPatternExpression = (IsPatternExpressionSyntax)expression;

                        if (!(isPatternExpression.Pattern is ConstantPatternSyntax constantPattern))
                            break;

                        if (constantPattern.Expression?.IsKind(SyntaxKind.NullLiteralExpression) != true)
                            break;

                        ExpressionSyntax e = WalkAndCheck(isPatternExpression.Expression, walkDownParentheses, allowMissing);

                        if (e == null)
                            break;

                        return new NullCheckExpressionInfo(expression, e, NullCheckStyles.IsNull);
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        bool isNotHasValueAllowed = (allowedStyles & (NullCheckStyles.NotHasValue)) != 0;
                        bool isNotIsNullAllowed = (allowedStyles & (NullCheckStyles.NotIsNull)) != 0;

                        if (!isNotHasValueAllowed && !isNotIsNullAllowed)
                            break;

                        var logicalNotExpression = (PrefixUnaryExpressionSyntax)expression;

                        ExpressionSyntax operand = WalkAndCheck(logicalNotExpression.Operand, walkDownParentheses, allowMissing);

                        if (operand == null)
                            break;

                        switch (operand.Kind())
                        {
                            case SyntaxKind.SimpleMemberAccessExpression:
                                {
                                    if (!isNotHasValueAllowed)
                                        break;

                                    var memberAccessExpression = (MemberAccessExpressionSyntax)operand;

                                    if (!IsPropertyOfNullableOfT(memberAccessExpression.Name, "HasValue", semanticModel, cancellationToken))
                                        break;

                                    return new NullCheckExpressionInfo(expression, memberAccessExpression.Expression, NullCheckStyles.NotHasValue);
                                }
                            case SyntaxKind.IsPatternExpression:
                                {
                                    if (!isNotIsNullAllowed)
                                        break;

                                    var isPatternExpression = (IsPatternExpressionSyntax)operand;

                                    if (!(isPatternExpression.Pattern is ConstantPatternSyntax constantPattern))
                                        break;

                                    if (constantPattern.Expression?.IsKind(SyntaxKind.NullLiteralExpression) != true)
                                        break;

                                    ExpressionSyntax e = WalkAndCheck(isPatternExpression.Expression, walkDownParentheses, allowMissing);

                                    if (e == null)
                                        break;

                                    return new NullCheckExpressionInfo(expression, e, NullCheckStyles.NotIsNull);
                                }
                        }

                        break;
                    }
            }

            return default;
        }

        private static NullCheckExpressionInfo Create(
            BinaryExpressionSyntax binaryExpression,
            SyntaxKind binaryExpressionKind,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            NullCheckStyles allowedStyles,
            bool allowMissing,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (expression1.Kind())
            {
                case SyntaxKind.NullLiteralExpression:
                    {
                        NullCheckStyles style = (binaryExpressionKind == SyntaxKind.EqualsExpression) ? NullCheckStyles.EqualsToNull : NullCheckStyles.NotEqualsToNull;

                        if ((allowedStyles & style) == 0)
                            break;

                        return new NullCheckExpressionInfo(
                            binaryExpression,
                            expression2,
                            style);
                    }
                case SyntaxKind.TrueLiteralExpression:
                    {
                        NullCheckStyles style = (binaryExpressionKind == SyntaxKind.EqualsExpression) ? NullCheckStyles.HasValue : NullCheckStyles.NotHasValue;

                        return Create(
                            binaryExpression,
                            expression2,
                            style,
                            allowedStyles,
                            allowMissing,
                            semanticModel,
                            cancellationToken);
                    }
                case SyntaxKind.FalseLiteralExpression:
                    {
                        NullCheckStyles style = (binaryExpressionKind == SyntaxKind.EqualsExpression) ? NullCheckStyles.NotHasValue : NullCheckStyles.HasValue;

                        return Create(
                            binaryExpression,
                            expression2,
                            style,
                            allowedStyles,
                            allowMissing,
                            semanticModel,
                            cancellationToken);
                    }
            }

            return default;
        }

        private static NullCheckExpressionInfo Create(
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax expression,
            NullCheckStyles style,
            NullCheckStyles allowedStyles,
            bool allowMissing,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if ((allowedStyles & (NullCheckStyles.HasValueProperty)) == 0)
                return default;

            if (!(expression is MemberAccessExpressionSyntax memberAccessExpression))
                return default;

            if (memberAccessExpression.Kind() != SyntaxKind.SimpleMemberAccessExpression)
                return default;

            if (!IsPropertyOfNullableOfT(memberAccessExpression.Name, "HasValue", semanticModel, cancellationToken))
                return default;

            if ((allowedStyles & style) == 0)
                return default;

            ExpressionSyntax expression2 = memberAccessExpression.Expression;

            if (!Check(expression2, allowMissing))
                return default;

            return new NullCheckExpressionInfo(binaryExpression, expression2, style);
        }

        private static bool IsPropertyOfNullableOfT(
            ExpressionSyntax expression,
            string name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return expression?.Kind() == SyntaxKind.IdentifierName
                && string.Equals(((IdentifierNameSyntax)expression).Identifier.ValueText, name, StringComparison.Ordinal)
                && SyntaxUtility.IsPropertyOfNullableOfT(expression, name, semanticModel, cancellationToken);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return NullCheckExpression?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is NullCheckExpressionInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(NullCheckExpressionInfo other)
        {
            return EqualityComparer<ExpressionSyntax>.Default.Equals(NullCheckExpression, other.NullCheckExpression);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<ExpressionSyntax>.Default.GetHashCode(NullCheckExpression);
        }

        public static bool operator ==(in NullCheckExpressionInfo info1, in NullCheckExpressionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in NullCheckExpressionInfo info1, in NullCheckExpressionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
