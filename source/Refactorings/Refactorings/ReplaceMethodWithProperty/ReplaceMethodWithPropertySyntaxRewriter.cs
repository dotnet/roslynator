// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings.ReplaceMethodWithProperty
{
    internal class ReplaceMethodWithPropertySyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly TextSpan[] _textSpans;
        private readonly MethodDeclarationSyntax _methodDeclaration;
        private readonly string _propertyName;

        public ReplaceMethodWithPropertySyntaxRewriter(TextSpan[] textSpans, MethodDeclarationSyntax methodDeclaration = null, string propertyName = null)
        {
            _textSpans = textSpans;
            _methodDeclaration = methodDeclaration;
            _propertyName = propertyName;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            ExpressionSyntax expression = node.Expression;

            if (expression != null)
            {
                if (_textSpans.Contains(expression.Span))
                {
                    expression = AppendTrailingTrivia(expression, node);

                    if (_propertyName != null)
                        expression = IdentifierName(_propertyName).WithTriviaFrom(expression);

                    return expression;
                }
                else if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)expression;

                    SimpleNameSyntax name = memberAccess.Name;

                    if (name != null && _textSpans.Contains(name.Span))
                    {
                        expression = (ExpressionSyntax)base.Visit(memberAccess.Expression);

                        if (_propertyName != null)
                            memberAccess = memberAccess.WithName(IdentifierName(_propertyName).WithTriviaFrom(name));

                        memberAccess = AppendTrailingTrivia(memberAccess, node);

                        return memberAccess.WithExpression(expression);
                    }
                }
            }

            return base.VisitInvocationExpression(node);
        }

        private static TNode AppendTrailingTrivia<TNode>(TNode node, InvocationExpressionSyntax invocation) where TNode : SyntaxNode
        {
            ArgumentListSyntax argumentList = invocation.ArgumentList;

            if (argumentList != null)
            {
                node = node.AppendTrailingTrivia(
                    argumentList.OpenParenToken.GetLeadingAndTrailingTrivia()
                        .Concat(argumentList.CloseParenToken.GetLeadingAndTrailingTrivia()));
            }

            return node;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (_methodDeclaration != null
                && node.Span == _methodDeclaration.Span)
            {
                node = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);

                return ReplaceMethodWithProperty(node, _propertyName)
                    .WithTriviaFrom(node)
                    .WithFormatterAnnotation();
            }

            return base.VisitMethodDeclaration(node);
        }

        private static PropertyDeclarationSyntax ReplaceMethodWithProperty(MethodDeclarationSyntax methodDeclaration, string propertyName = null)
        {
            SyntaxToken identifier = methodDeclaration.Identifier;

            if (propertyName != null)
                identifier = Identifier(propertyName).WithTriviaFrom(identifier);

            if (methodDeclaration.ExpressionBody != null)
            {
                return PropertyDeclaration(
                    methodDeclaration.AttributeLists,
                    methodDeclaration.Modifiers,
                    methodDeclaration.ReturnType,
                    methodDeclaration.ExplicitInterfaceSpecifier,
                    identifier,
                    null,
                    methodDeclaration.ExpressionBody,
                    null);
            }
            else
            {
                return PropertyDeclaration(
                    methodDeclaration.AttributeLists,
                    methodDeclaration.Modifiers,
                    methodDeclaration.ReturnType,
                    methodDeclaration.ExplicitInterfaceSpecifier,
                    identifier,
                    CreateAccessorList(methodDeclaration));
            }
        }

        private static AccessorListSyntax CreateAccessorList(MethodDeclarationSyntax method)
        {
            if (method.Body != null)
            {
                bool singleline = method.Body.Statements.Count == 1
                    && method.Body.Statements[0].IsSingleLine();

                return CreateAccessorList(Block(method.Body?.Statements), singleline)
                    .WithOpenBraceToken(method.Body.OpenBraceToken)
                    .WithCloseBraceToken(method.Body.CloseBraceToken);
            }

            return CreateAccessorList(Block(), singleline: true);
        }

        private static AccessorListSyntax CreateAccessorList(BlockSyntax block, bool singleline)
        {
            AccessorListSyntax accessorList =
                AccessorList(
                    SingletonList(
                        AccessorDeclaration(
                            SyntaxKind.GetAccessorDeclaration,
                            block)));

            if (singleline)
                accessorList = SyntaxRemover.RemoveWhitespaceOrEndOfLine(accessorList);

            return accessorList;
        }
    }
}
