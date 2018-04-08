// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.ReplacePropertyWithMethod
{
    internal class ReplacePropertyWithMethodSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly ImmutableArray<SyntaxNode> _nodes;
        private readonly string _methodName;
        private readonly PropertyDeclarationSyntax _propertyDeclaration;

        public ReplacePropertyWithMethodSyntaxRewriter(ImmutableArray<SyntaxNode> nodes, string methodName, PropertyDeclarationSyntax propertyDeclaration = null)
        {
            _nodes = nodes;
            _methodName = methodName;
            _propertyDeclaration = propertyDeclaration;
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (_nodes.Contains(node))
            {
                node = IdentifierName(_methodName).WithTriviaFrom(node);

                return InvocationExpression(
                    node.WithoutTrailingTrivia(),
                    ArgumentList().WithTrailingTrivia(node.GetTrailingTrivia()));
            }

            return base.VisitIdentifierName(node);
        }

        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            SimpleNameSyntax name = node.Name;

            if (name != null && _nodes.Contains(name))
            {
                var expression = (ExpressionSyntax)base.Visit(node.Expression);

                node = node
                    .WithExpression(expression)
                    .WithName(IdentifierName(_methodName).WithTriviaFrom(name));

                return InvocationExpression(
                    node.WithoutTrailingTrivia(),
                    ArgumentList().WithTrailingTrivia(node.GetTrailingTrivia()));
            }

            return base.VisitMemberAccessExpression(node);
        }

        public override SyntaxNode VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            ExpressionSyntax whenNotNull = node.WhenNotNull;

            if (whenNotNull?.Kind() == SyntaxKind.MemberBindingExpression)
            {
                var memberBinding = (MemberBindingExpressionSyntax)whenNotNull;

                SimpleNameSyntax name = memberBinding.Name;

                if (name != null
                    && _nodes.Contains(name))
                {
                    var expression = (ExpressionSyntax)base.Visit(node.Expression);

                    InvocationExpressionSyntax invocation = InvocationExpression(
                        memberBinding.WithName(IdentifierName(_methodName).WithLeadingTrivia(name.GetLeadingTrivia())),
                        ArgumentList().WithTrailingTrivia(memberBinding.GetTrailingTrivia()));

                    return node
                        .WithExpression(expression)
                        .WithWhenNotNull(invocation);
                }
            }

            return base.VisitConditionalAccessExpression(node);
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (_propertyDeclaration != null
                && node.Span == _propertyDeclaration.Span)
            {
                node = (PropertyDeclarationSyntax)base.VisitPropertyDeclaration(node);

                return ReplacePropertyWithMethodRefactoring.ToMethodDeclaration(node);
            }

            return base.VisitPropertyDeclaration(node);
        }
    }
}
