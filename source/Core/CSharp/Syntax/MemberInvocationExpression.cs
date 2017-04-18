// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    internal struct MemberInvocationExpression : IEquatable<MemberInvocationExpression>
    {
        public MemberInvocationExpression(ExpressionSyntax expression, SimpleNameSyntax name, ArgumentListSyntax argumentList)
        {
            Expression = expression;
            Name = name;
            ArgumentList = argumentList;
        }

        public ExpressionSyntax Expression { get; }
        public SimpleNameSyntax Name { get; }
        public ArgumentListSyntax ArgumentList { get; }

        public InvocationExpressionSyntax InvocationExpression
        {
            get { return (InvocationExpressionSyntax)Parent; }
        }

        public MemberAccessExpressionSyntax MemberAccessExpression
        {
            get { return (MemberAccessExpressionSyntax)Expression?.Parent; }
        }

        private SyntaxNode Parent
        {
            get { return ArgumentList?.Parent; }
        }

        public static MemberInvocationExpression Create(InvocationExpressionSyntax invocationExpression)
        {
            if (invocationExpression == null)
                throw new ArgumentNullException(nameof(invocationExpression));

            ExpressionSyntax expression = invocationExpression.Expression;

            if (expression == null
                || !expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                throw new ArgumentException("", nameof(invocationExpression));
            }

            var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

            return new MemberInvocationExpression(
                memberAccessExpression.Expression,
                memberAccessExpression.Name,
                invocationExpression.ArgumentList);
        }

        public static bool TryCreate(SyntaxNode invocationExpression, out MemberInvocationExpression result)
        {
            if (invocationExpression?.IsKind(SyntaxKind.InvocationExpression) == true)
                return TryCreateCore((InvocationExpressionSyntax)invocationExpression, out result);

            result = default(MemberInvocationExpression);
            return false;
        }

        public static bool TryCreate(InvocationExpressionSyntax invocationExpression, out MemberInvocationExpression result)
        {
            if (invocationExpression != null)
                return TryCreateCore(invocationExpression, out result);

            result = default(MemberInvocationExpression);
            return false;
        }

        public static bool TryCreateCore(InvocationExpressionSyntax invocationExpression, out MemberInvocationExpression result)
        {
            ExpressionSyntax expression = invocationExpression.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                var memberAccess = (MemberAccessExpressionSyntax)expression;

                result = new MemberInvocationExpression(
                    memberAccess.Expression,
                    memberAccess.Name,
                    invocationExpression.ArgumentList);

                return true;
            }

            result = default(MemberInvocationExpression);
            return false;
        }

        public bool Equals(MemberInvocationExpression other)
        {
            return Parent == other.Parent;
        }

        public override bool Equals(object obj)
        {
            return obj is MemberInvocationExpression
                && Equals((MemberInvocationExpression)obj);
        }

        public override int GetHashCode()
        {
            return Parent?.GetHashCode() ?? 0;
        }

        public static bool operator ==(MemberInvocationExpression left, MemberInvocationExpression right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MemberInvocationExpression left, MemberInvocationExpression right)
        {
            return !left.Equals(right);
        }
    }
}
