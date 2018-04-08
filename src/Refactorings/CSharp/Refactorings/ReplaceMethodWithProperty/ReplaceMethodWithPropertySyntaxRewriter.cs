// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.ReplaceMethodWithProperty
{
    internal class ReplaceMethodWithPropertySyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly ImmutableArray<SyntaxNode> _nodes;
        private readonly string _propertyName;
        private readonly MethodDeclarationSyntax _methodDeclaration;

        public ReplaceMethodWithPropertySyntaxRewriter(ImmutableArray<SyntaxNode> nodes, string propertyName, MethodDeclarationSyntax methodDeclaration = null)
        {
            _nodes = nodes;
            _propertyName = propertyName;
            _methodDeclaration = methodDeclaration;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            ExpressionSyntax expression = node.Expression;

            if (expression != null)
            {
                if (_nodes.Contains(expression))
                {
                    expression = IdentifierName(_propertyName).WithTriviaFrom(expression);

                    return AppendToTrailingTrivia(expression, node);
                }
                else if (expression?.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)expression;

                    SimpleNameSyntax name = memberAccess.Name;

                    if (name != null && _nodes.Contains(name))
                    {
                        expression = (ExpressionSyntax)base.Visit(memberAccess.Expression);

                        memberAccess = memberAccess
                            .WithExpression(expression)
                            .WithName(IdentifierName(_propertyName).WithTriviaFrom(name));

                        return AppendToTrailingTrivia(memberAccess, node);
                    }
                }
            }

            return base.VisitInvocationExpression(node);
        }

        private static TNode AppendToTrailingTrivia<TNode>(TNode node, InvocationExpressionSyntax invocation) where TNode : SyntaxNode
        {
            ArgumentListSyntax argumentList = invocation.ArgumentList;

            if (argumentList != null)
            {
                node = node.AppendToTrailingTrivia(
                    argumentList.OpenParenToken.GetAllTrivia()
                        .Concat(argumentList.CloseParenToken.GetAllTrivia()));
            }

            return node;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node == _methodDeclaration)
            {
                node = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);

                return ReplaceMethodWithPropertyRefactoring.ToPropertyDeclaration(node);
            }

            return base.VisitMethodDeclaration(node);
        }
    }
}
