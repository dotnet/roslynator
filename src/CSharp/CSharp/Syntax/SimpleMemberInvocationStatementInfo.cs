// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about invocation expression in an expression statement.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct SimpleMemberInvocationStatementInfo : IEquatable<SimpleMemberInvocationStatementInfo>
    {
        private readonly SimpleMemberInvocationExpressionInfo _info;

        private SimpleMemberInvocationStatementInfo(in SimpleMemberInvocationExpressionInfo info)
        {
            _info = info;
        }

        /// <summary>
        /// The invocation expression.
        /// </summary>
        public InvocationExpressionSyntax InvocationExpression => _info.InvocationExpression;

        /// <summary>
        /// The member access expression.
        /// </summary>
        public MemberAccessExpressionSyntax MemberAccessExpression => _info.MemberAccessExpression;

        /// <summary>
        /// The expression that contains the member being invoked.
        /// </summary>
        public ExpressionSyntax Expression => _info.Expression;

        /// <summary>
        /// The name of the member being invoked.
        /// </summary>
        public SimpleNameSyntax Name => _info.Name;

        /// <summary>
        /// The argument list.
        /// </summary>
        public ArgumentListSyntax ArgumentList => _info.ArgumentList;

        /// <summary>
        /// A list of arguments.
        /// </summary>
        public SeparatedSyntaxList<ArgumentSyntax> Arguments => _info.Arguments;

        /// <summary>
        /// The name of the member being invoked.
        /// </summary>
        public string NameText => _info.NameText;

        /// <summary>
        /// The expression statement that contains the invocation expression.
        /// </summary>
        public ExpressionStatementSyntax Statement
        {
            get { return (ExpressionStatementSyntax)InvocationExpression?.Parent; }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success => _info.Success;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, Statement); }
        }

        internal static SimpleMemberInvocationStatementInfo Create(
            SyntaxNode node,
            bool allowMissing = false)
        {
            switch (node)
            {
                case ExpressionStatementSyntax expressionStatement:
                    return Create(expressionStatement, allowMissing);
                case InvocationExpressionSyntax invocationExpression:
                    return Create(invocationExpression, allowMissing);
            }

            return default;
        }

        internal static SimpleMemberInvocationStatementInfo Create(
            ExpressionStatementSyntax expressionStatement,
            bool allowMissing = false)
        {
            if (!(expressionStatement?.Expression is InvocationExpressionSyntax invocationExpression))
                return default;

            return CreateImpl(invocationExpression, allowMissing);
        }

        internal static SimpleMemberInvocationStatementInfo Create(
            InvocationExpressionSyntax invocationExpression,
            bool allowMissing = false)
        {
            if (!invocationExpression.IsParentKind(SyntaxKind.ExpressionStatement))
                return default;

            return CreateImpl(invocationExpression, allowMissing);
        }

        private static SimpleMemberInvocationStatementInfo CreateImpl(InvocationExpressionSyntax invocationExpression, bool allowMissing)
        {
            SimpleMemberInvocationExpressionInfo info = SimpleMemberInvocationExpressionInfo.Create(invocationExpression, allowMissing);

            return new SimpleMemberInvocationStatementInfo(info);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return InvocationExpression?.Parent.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is SimpleMemberInvocationStatementInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(SimpleMemberInvocationStatementInfo other)
        {
            return EqualityComparer<SyntaxNode>.Default.Equals(InvocationExpression?.Parent, other.InvocationExpression?.Parent);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<SyntaxNode>.Default.GetHashCode(InvocationExpression?.Parent);
        }

        public static bool operator ==(in SimpleMemberInvocationStatementInfo info1, in SimpleMemberInvocationStatementInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in SimpleMemberInvocationStatementInfo info1, in SimpleMemberInvocationStatementInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
