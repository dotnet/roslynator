// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    public struct MemberInvocationExpressionInfo
    {
        private static MemberInvocationExpressionInfo Default { get; } = new MemberInvocationExpressionInfo();

        private MemberInvocationExpressionInfo(
            ExpressionSyntax expression,
            SimpleNameSyntax name,
            ArgumentListSyntax argumentList)
        {
            Expression = expression;
            Name = name;
            ArgumentList = argumentList;
        }

        public ExpressionSyntax Expression { get; }

        public SimpleNameSyntax Name { get; }

        public ArgumentListSyntax ArgumentList { get; }

        public SeparatedSyntaxList<ArgumentSyntax> Arguments
        {
            get { return ArgumentList?.Arguments ?? default(SeparatedSyntaxList<ArgumentSyntax>); }
        }

        public InvocationExpressionSyntax InvocationExpression
        {
            get { return (InvocationExpressionSyntax)ArgumentList?.Parent; }
        }

        public MemberAccessExpressionSyntax MemberAccessExpression
        {
            get { return (MemberAccessExpressionSyntax)Expression?.Parent; }
        }

        public SyntaxToken OperatorToken
        {
            get { return MemberAccessExpression?.OperatorToken ?? default(SyntaxToken); }
        }

        public string NameText
        {
            get { return Name?.Identifier.ValueText; }
        }

        public bool Success
        {
            get { return Expression != null; }
        }

        internal static MemberInvocationExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateCore(
                Walk(node, walkDownParentheses) as InvocationExpressionSyntax,
                allowMissing);
        }

        internal static MemberInvocationExpressionInfo Create(
            InvocationExpressionSyntax invocationExpression,
            bool allowMissing = false)
        {
            return CreateCore(invocationExpression, allowMissing);
        }

        private static MemberInvocationExpressionInfo CreateCore(
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

            return new MemberInvocationExpressionInfo(
                expression,
                name,
                argumentList);
        }

        public override string ToString()
        {
            return InvocationExpression?.ToString() ?? base.ToString();
        }
    }
}
