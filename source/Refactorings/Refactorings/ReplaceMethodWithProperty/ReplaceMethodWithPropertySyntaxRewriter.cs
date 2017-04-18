// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.ReplaceMethodWithProperty
{
    internal class ReplaceMethodWithPropertySyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly TextSpan[] _textSpans;
        private readonly string _propertyName;
        private readonly MethodDeclarationSyntax _methodDeclaration;

        public ReplaceMethodWithPropertySyntaxRewriter(TextSpan[] textSpans, string propertyName, MethodDeclarationSyntax methodDeclaration = null)
        {
            _textSpans = textSpans;
            _propertyName = propertyName;
            _methodDeclaration = methodDeclaration;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            ExpressionSyntax expression = node.Expression;

            if (expression != null)
            {
                if (_textSpans.Contains(expression.Span))
                {
                    expression = IdentifierName(_propertyName).WithTriviaFrom(expression);

                    expression = AppendToTrailingTrivia(expression, node);

                    return expression;
                }
                else if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)expression;

                    SimpleNameSyntax name = memberAccess.Name;

                    if (name != null && _textSpans.Contains(name.Span))
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

        private static PropertyDeclarationSyntax ReplaceMethodWithProperty(MethodDeclarationSyntax methodDeclaration, string propertyName)
        {
            SyntaxToken identifier = Identifier(propertyName).WithTriviaFrom(methodDeclaration.Identifier);

            ParameterListSyntax parameterList = methodDeclaration.ParameterList;

            if (parameterList?.IsMissing == false)
            {
                identifier = identifier.AppendToTrailingTrivia(
                    parameterList.OpenParenToken.GetLeadingAndTrailingTrivia().Concat(
                        parameterList.CloseParenToken.GetLeadingAndTrailingTrivia()));
            }

            if (methodDeclaration.ExpressionBody != null)
            {
                return PropertyDeclaration(
                    methodDeclaration.AttributeLists,
                    methodDeclaration.Modifiers,
                    methodDeclaration.ReturnType,
                    methodDeclaration.ExplicitInterfaceSpecifier,
                    identifier,
                    default(AccessorListSyntax),
                    methodDeclaration.ExpressionBody,
                    default(EqualsValueClauseSyntax),
                    methodDeclaration.SemicolonToken);
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
            BlockSyntax body = method.Body;

            if (body != null)
            {
                SyntaxList<StatementSyntax> statements = body.Statements;

                bool singleline = statements.Count == 1
                    && body.DescendantTrivia(body.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia())
                    && statements[0].IsSingleLine();

                return CreateAccessorList(Block(body.Statements), singleline)
                    .WithOpenBraceToken(body.OpenBraceToken)
                    .WithCloseBraceToken(body.CloseBraceToken);
            }

            return CreateAccessorList(Block(), singleline: true);
        }

        private static AccessorListSyntax CreateAccessorList(BlockSyntax block, bool singleline)
        {
            AccessorListSyntax accessorList = AccessorList(GetAccessorDeclaration(block));

            if (singleline)
                accessorList = accessorList.RemoveWhitespaceOrEndOfLineTrivia();

            return accessorList;
        }
    }
}
