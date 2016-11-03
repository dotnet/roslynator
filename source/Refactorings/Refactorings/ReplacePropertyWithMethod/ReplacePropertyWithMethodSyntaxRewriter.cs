// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.ReplacePropertyWithMethod
{
    internal class ReplacePropertyWithMethodSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly HashSet<TextSpan> _textSpans;
        private readonly string _methodName;
        private readonly PropertyDeclarationSyntax _propertyDeclaration;

        public ReplacePropertyWithMethodSyntaxRewriter(TextSpan[] textSpans, string methodName, PropertyDeclarationSyntax propertyDeclaration = null)
        {
            _textSpans = new HashSet<TextSpan>(textSpans);
            _methodName = methodName;
            _propertyDeclaration = propertyDeclaration;
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (_textSpans.Contains(node.Span))
            {
                _textSpans.Remove(node.Span);

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

            if (name != null && _textSpans.Contains(name.Span))
            {
                _textSpans.Remove(node.Span);

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

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (_propertyDeclaration != null
                && node.Span == _propertyDeclaration.Span)
            {
                node = (PropertyDeclarationSyntax)base.VisitPropertyDeclaration(node);

                return ReplacePropertyWithMethod(node, _methodName);
            }

            return base.VisitPropertyDeclaration(node);
        }

        public static MethodDeclarationSyntax ReplacePropertyWithMethod(PropertyDeclarationSyntax property, string methodName)
        {
            AccessorDeclarationSyntax getter = property.Getter();

            BlockSyntax getterBody = getter.Body;

            BlockSyntax methodBody = null;

            if (getterBody != null)
            {
                methodBody = Block(getterBody.Statements)
                    .WithTrailingTrivia(property.GetTrailingTrivia());
            }
            else
            {
                methodBody = Block(
                    ReturnStatement(property.Initializer.Value)
                        .WithTrailingTrivia(property.GetTrailingTrivia()));
            }

            MethodDeclarationSyntax method = MethodDeclaration(
                property.AttributeLists,
                property.Modifiers,
                property.Type,
                property.ExplicitInterfaceSpecifier,
                Identifier(methodName).WithLeadingTrivia(property.Identifier.LeadingTrivia),
                default(TypeParameterListSyntax),
                ParameterList().WithTrailingTrivia(property.Identifier.TrailingTrivia),
                List<TypeParameterConstraintClauseSyntax>(),
                methodBody,
                default(ArrowExpressionClauseSyntax));

            return method
                .WithLeadingTrivia(property.GetLeadingTrivia())
                .WithFormatterAnnotation();
        }
    }
}
