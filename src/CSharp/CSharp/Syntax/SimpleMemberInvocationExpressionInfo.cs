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
    /// Provides information about invocation expression.
    /// </summary>
    public readonly struct SimpleMemberInvocationExpressionInfo : IEquatable<SimpleMemberInvocationExpressionInfo>
    {
        private SimpleMemberInvocationExpressionInfo(
            InvocationExpressionSyntax invocationExpression,
            MemberAccessExpressionSyntax memberAccessExpression)
        {
            InvocationExpression = invocationExpression;
            MemberAccessExpression = memberAccessExpression;
        }

        private static SimpleMemberInvocationExpressionInfo Default { get; } = new SimpleMemberInvocationExpressionInfo();

        /// <summary>
        /// The invocation expression.
        /// </summary>
        public InvocationExpressionSyntax InvocationExpression { get; }

        /// <summary>
        /// The member access expression.
        /// </summary>
        public MemberAccessExpressionSyntax MemberAccessExpression { get; }

        /// <summary>
        /// The expression that contains the member being invoked.
        /// </summary>
        public ExpressionSyntax Expression
        {
            get { return MemberAccessExpression?.Expression; }
        }

        /// <summary>
        /// The name of the member being invoked.
        /// </summary>
        public SimpleNameSyntax Name
        {
            get { return MemberAccessExpression?.Name; }
        }

        /// <summary>
        /// The argumet list.
        /// </summary>
        public ArgumentListSyntax ArgumentList
        {
            get { return InvocationExpression?.ArgumentList; }
        }

        /// <summary>
        /// The list of the arguments.
        /// </summary>
        public SeparatedSyntaxList<ArgumentSyntax> Arguments
        {
            get { return InvocationExpression?.ArgumentList.Arguments ?? default(SeparatedSyntaxList<ArgumentSyntax>); }
        }

        /// <summary>
        /// The operator in the member access expression.
        /// </summary>
        public SyntaxToken OperatorToken
        {
            get { return MemberAccessExpression?.OperatorToken ?? default(SyntaxToken); }
        }

        /// <summary>
        /// The name of the member being invoked.
        /// </summary>
        public string NameText
        {
            get { return Name?.Identifier.ValueText; }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return InvocationExpression != null; }
        }

        internal static SimpleMemberInvocationExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateImpl(
                Walk(node, walkDownParentheses) as InvocationExpressionSyntax,
                allowMissing);
        }

        internal static SimpleMemberInvocationExpressionInfo Create(
            InvocationExpressionSyntax invocationExpression,
            bool allowMissing = false)
        {
            return CreateImpl(invocationExpression, allowMissing);
        }

        private static SimpleMemberInvocationExpressionInfo CreateImpl(
            InvocationExpressionSyntax invocationExpression,
            bool allowMissing = false)
        {
            if (!(invocationExpression?.Expression is MemberAccessExpressionSyntax memberAccessExpression))
                return Default;

            if (memberAccessExpression.Kind() != SyntaxKind.SimpleMemberAccessExpression)
                return Default;

            ExpressionSyntax expression = memberAccessExpression.Expression;

            if (!Check(expression, allowMissing))
                return Default;

            SimpleNameSyntax name = memberAccessExpression.Name;

            if (!Check(name, allowMissing))
                return Default;

            ArgumentListSyntax argumentList = invocationExpression.ArgumentList;

            if (argumentList == null)
                return Default;

            return new SimpleMemberInvocationExpressionInfo(invocationExpression, memberAccessExpression);
        }

        internal SimpleMemberInvocationExpressionInfo WithName(string name)
        {
            MemberAccessExpressionSyntax newMemberAccess = MemberAccessExpression.WithName(SyntaxFactory.IdentifierName(name).WithTriviaFrom(Name));

            InvocationExpressionSyntax newInvocation = InvocationExpression.WithExpression(newMemberAccess);

            return new SimpleMemberInvocationExpressionInfo(newInvocation, newMemberAccess);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return InvocationExpression?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is SimpleMemberInvocationExpressionInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(SimpleMemberInvocationExpressionInfo other)
        {
            return EqualityComparer<InvocationExpressionSyntax>.Default.Equals(InvocationExpression, other.InvocationExpression);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<InvocationExpressionSyntax>.Default.GetHashCode(InvocationExpression);
        }

        public static bool operator ==(SimpleMemberInvocationExpressionInfo info1, SimpleMemberInvocationExpressionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(SimpleMemberInvocationExpressionInfo info1, SimpleMemberInvocationExpressionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
