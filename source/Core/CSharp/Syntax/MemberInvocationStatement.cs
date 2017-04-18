// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    internal struct MemberInvocationStatement : IEquatable<MemberInvocationStatement>
    {
        public MemberInvocationStatement(
            InvocationExpressionSyntax invocationExpression,
            ExpressionSyntax expression,
            SimpleNameSyntax name,
            ArgumentListSyntax argumentList)
        {
            InvocationExpression = invocationExpression;
            Expression = expression;
            Name = name;
            ArgumentList = argumentList;
        }

        public InvocationExpressionSyntax InvocationExpression { get; }
        public ExpressionSyntax Expression { get; }
        public SimpleNameSyntax Name { get; }
        public ArgumentListSyntax ArgumentList { get; }

        public ExpressionStatementSyntax ExpressionStatement
        {
            get { return (ExpressionStatementSyntax)Parent; }
        }

        public MemberAccessExpressionSyntax MemberAccessExpression
        {
            get { return (MemberAccessExpressionSyntax)Expression?.Parent; }
        }

        private SyntaxNode Parent
        {
            get { return InvocationExpression?.Parent; }
        }

        public static MemberInvocationStatement Create(ExpressionStatementSyntax expressionStatement)
        {
            if (expressionStatement == null)
                throw new ArgumentNullException(nameof(expressionStatement));

            ExpressionSyntax expression = expressionStatement.Expression;

            if (expression == null
                || !expression.IsKind(SyntaxKind.InvocationExpression))
            {
                throw new ArgumentException("", nameof(expressionStatement));
            }

            var invocationExpression = (InvocationExpressionSyntax)expression;

            ExpressionSyntax expression2 = invocationExpression.Expression;

            if (expression2 == null
                || !expression2.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                throw new ArgumentException("", nameof(expressionStatement));
            }

            var memberAccessExpression = (MemberAccessExpressionSyntax)expression2;

            return new MemberInvocationStatement(
                invocationExpression,
                memberAccessExpression.Expression,
                memberAccessExpression.Name,
                invocationExpression.ArgumentList);
        }

        public static bool TryCreate(SyntaxNode invocationStatement, out MemberInvocationStatement result)
        {
            if (invocationStatement?.IsKind(SyntaxKind.ExpressionStatement) == true)
                return TryCreateCore((ExpressionStatementSyntax)invocationStatement, out result);

            result = default(MemberInvocationStatement);
            return false;
        }

        public static bool TryCreate(ExpressionStatementSyntax invocationStatement, out MemberInvocationStatement result)
        {
            if (invocationStatement != null)
                return TryCreateCore(invocationStatement, out result);

            result = default(MemberInvocationStatement);
            return false;
        }

        public static bool TryCreateCore(ExpressionStatementSyntax expressionStatement, out MemberInvocationStatement result)
        {
            ExpressionSyntax expression = expressionStatement.Expression;

            if (expression?.IsKind(SyntaxKind.InvocationExpression) == true)
            {
                var invocationExpression = (InvocationExpressionSyntax)expression;

                ExpressionSyntax expression2 = invocationExpression.Expression;

                if (expression2?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    var simpleMemberAccess = (MemberAccessExpressionSyntax)expression2;

                    result = new MemberInvocationStatement(
                        invocationExpression,
                        simpleMemberAccess.Expression,
                        simpleMemberAccess.Name,
                        invocationExpression.ArgumentList);

                    return true;
                }
            }

            result = default(MemberInvocationStatement);
            return false;
        }

        public bool Equals(MemberInvocationStatement other)
        {
            return Parent == other.Parent;
        }

        public override bool Equals(object obj)
        {
            return obj is MemberInvocationStatement
                && Equals((MemberInvocationStatement)obj);
        }

        public override int GetHashCode()
        {
            return Parent?.GetHashCode() ?? 0;
        }

        public static bool operator ==(MemberInvocationStatement left, MemberInvocationStatement right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MemberInvocationStatement left, MemberInvocationStatement right)
        {
            return !left.Equals(right);
        }
    }
}
