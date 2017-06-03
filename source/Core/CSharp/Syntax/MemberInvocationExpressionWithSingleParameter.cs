// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    internal struct MemberInvocationExpressionWithSingleParameter : IEquatable<MemberInvocationExpressionWithSingleParameter>
    {
        public MemberInvocationExpressionWithSingleParameter(ExpressionSyntax expression, SimpleNameSyntax name, ArgumentSyntax argument)
        {
            Expression = expression;
            Name = name;
            Argument = argument;
        }

        public ExpressionSyntax Expression { get; }
        public SimpleNameSyntax Name { get; }
        public ArgumentSyntax Argument { get; }

        public InvocationExpressionSyntax InvocationExpression
        {
            get { return (InvocationExpressionSyntax)Node; }
        }

        public MemberAccessExpressionSyntax MemberAccessExpression
        {
            get { return (MemberAccessExpressionSyntax)Expression?.Parent; }
        }

        public string NameText
        {
            get { return Name?.Identifier.ValueText; }
        }

        private SyntaxNode Node
        {
            get { return Argument?.Parent?.Parent; }
        }

        public ArgumentListSyntax ArgumentList
        {
            get { return (ArgumentListSyntax)Argument?.Parent; }
        }

        public static MemberInvocationExpressionWithSingleParameter Create(InvocationExpressionSyntax invocationExpression)
        {
            if (invocationExpression == null)
                throw new ArgumentNullException(nameof(invocationExpression));

            var memberAccess = (MemberAccessExpressionSyntax)invocationExpression.Expression;

            ArgumentListSyntax argumentList = invocationExpression.ArgumentList;

            if (argumentList == null)
                throw new ArgumentException("", nameof(invocationExpression));

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count != 1)
                throw new ArgumentException("", nameof(invocationExpression));

            return new MemberInvocationExpressionWithSingleParameter(
                memberAccess.Expression,
                memberAccess.Name,
                arguments[0]);
        }

        public static bool TryCreate(SyntaxNode invocationExpression, out MemberInvocationExpressionWithSingleParameter result)
        {
            if (invocationExpression?.IsKind(SyntaxKind.InvocationExpression) == true)
                return TryCreate((InvocationExpressionSyntax)invocationExpression, out result);

            result = default(MemberInvocationExpressionWithSingleParameter);
            return false;
        }

        public static bool TryCreate(InvocationExpressionSyntax invocationExpression, out MemberInvocationExpressionWithSingleParameter result)
        {
            ArgumentListSyntax argumentList = invocationExpression?.ArgumentList;

            if (argumentList != null)
            {
                SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

                if (arguments.Count == 1)
                {
                    ExpressionSyntax expression = invocationExpression.Expression;

                    if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                    {
                        var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

                        result = new MemberInvocationExpressionWithSingleParameter(memberAccessExpression.Expression, memberAccessExpression.Name, arguments[0]);
                        return true;
                    }
                }
            }

            result = default(MemberInvocationExpressionWithSingleParameter);
            return false;
        }

        public override string ToString()
        {
            return Node?.ToString() ?? base.ToString();
        }

        public bool Equals(MemberInvocationExpressionWithSingleParameter other)
        {
            return Node == other.Node;
        }

        public override bool Equals(object obj)
        {
            return obj is MemberInvocationExpressionWithSingleParameter
                && Equals((MemberInvocationExpressionWithSingleParameter)obj);
        }

        public override int GetHashCode()
        {
            return Node?.GetHashCode() ?? 0;
        }

        public static bool operator ==(MemberInvocationExpressionWithSingleParameter left, MemberInvocationExpressionWithSingleParameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MemberInvocationExpressionWithSingleParameter left, MemberInvocationExpressionWithSingleParameter right)
        {
            return !left.Equals(right);
        }
    }
}
