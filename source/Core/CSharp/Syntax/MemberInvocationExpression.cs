// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

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

        public static bool TryCreate(StatementSyntax invocationStatement, out MemberInvocationExpression memberInvocation)
        {
            if (invocationStatement?.IsKind(SyntaxKind.ExpressionStatement) == true)
                return TryCreate((ExpressionStatementSyntax)invocationStatement, out memberInvocation);

            memberInvocation = default(MemberInvocationExpression);
            return false;
        }

        public static bool TryCreate(ExpressionStatementSyntax invocationStatement, out MemberInvocationExpression memberInvocation)
        {
            ExpressionSyntax expression = invocationStatement?.Expression;

            if (expression?.IsKind(SyntaxKind.InvocationExpression) == true)
                return TryCreate((InvocationExpressionSyntax)expression, out memberInvocation);

            memberInvocation = default(MemberInvocationExpression);
            return false;
        }

        public static bool TryCreate(InvocationExpressionSyntax invocationExpression, out MemberInvocationExpression memberInvocation)
        {
            ArgumentListSyntax argumentList = invocationExpression?.ArgumentList;

            if (argumentList?.IsMissing == false)
            {
                ExpressionSyntax expression = invocationExpression?.Expression;

                if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)expression;

                    ExpressionSyntax memberAccessExpression = memberAccess.Expression;

                    if (memberAccessExpression?.IsMissing == false)
                    {
                        SimpleNameSyntax memberAccessName = memberAccess.Name;

                        if (memberAccessName?.IsMissing == false)
                        {
                            memberInvocation = new MemberInvocationExpression(memberAccessExpression, memberAccessName, invocationExpression.ArgumentList);
                            return true;
                        }
                    }
                }
            }

            memberInvocation = default(MemberInvocationExpression);
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
