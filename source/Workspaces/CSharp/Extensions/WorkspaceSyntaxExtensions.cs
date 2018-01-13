// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    public static class WorkspaceSyntaxExtensions
    {
        private const string NavigationAnnotationKind = "CodeAction_Navigation";

        public static SyntaxAnnotation NavigationAnnotation { get; } = new SyntaxAnnotation(NavigationAnnotationKind);

        private static readonly SyntaxAnnotation[] _formatterAnnotationArray = new SyntaxAnnotation[] { Formatter.Annotation };

        private static readonly SyntaxAnnotation[] _simplifierAnnotationArray = new SyntaxAnnotation[] { Simplifier.Annotation };

        private static readonly SyntaxAnnotation[] _navigationAnnotationArray = new SyntaxAnnotation[] { NavigationAnnotation };

        private static readonly SyntaxAnnotation[] _formatterAndSimplifierAnnotationArray = new SyntaxAnnotation[] { Formatter.Annotation, Simplifier.Annotation };

        #region ExpressionSyntax
        public static ParenthesizedExpressionSyntax Parenthesize(
            this ExpressionSyntax expression,
            bool includeElasticTrivia = true,
            bool simplifiable = true)
        {
            ParenthesizedExpressionSyntax parenthesizedExpression = null;

            if (includeElasticTrivia)
            {
                parenthesizedExpression = ParenthesizedExpression(expression.WithoutTrivia());
            }
            else
            {
                parenthesizedExpression = ParenthesizedExpression(
                    Token(SyntaxTriviaList.Empty, SyntaxKind.OpenParenToken, SyntaxTriviaList.Empty),
                    expression.WithoutTrivia(),
                    Token(SyntaxTriviaList.Empty, SyntaxKind.CloseParenToken, SyntaxTriviaList.Empty));
            }

            return parenthesizedExpression
                .WithTriviaFrom(expression)
                .WithSimplifierAnnotationIf(simplifiable);
        }

        internal static ExpressionSyntax ParenthesizeIf(
            this ExpressionSyntax expression,
            bool condition,
            bool includeElasticTrivia = true,
            bool simplifiable = true)
        {
            return (condition) ? Parenthesize(expression, includeElasticTrivia, simplifiable) : expression;
        }
        #endregion ExpressionSyntax

        #region SimpleNameSyntax
        public static MemberAccessExpressionSyntax QualifyWithThis(this SimpleNameSyntax simpleName, bool simplifiable = true)
        {
            return SimpleMemberAccessExpression(ThisExpression(), simpleName).WithSimplifierAnnotationIf(simplifiable);
        }
        #endregion SimpleNameSyntax

        #region SyntaxNode
        public static TNode WithFormatterAnnotation<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithAdditionalAnnotations(_formatterAnnotationArray);
        }

        public static TNode WithSimplifierAnnotation<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithAdditionalAnnotations(_simplifierAnnotationArray);
        }

        internal static TNode WithNavigationAnnotation<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxToken token = node.GetFirstToken();

            if (token.Kind() == SyntaxKind.None)
                return node;

            return node.ReplaceToken(token, token.WithNavigationAnnotation());
        }

        internal static TNode WithSimplifierAnnotationIf<TNode>(this TNode node, bool condition) where TNode : SyntaxNode
        {
            return (condition) ? node.WithAdditionalAnnotations(_simplifierAnnotationArray) : node;
        }

        internal static TNode WithFormatterAndSimplifierAnnotations<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithAdditionalAnnotations(_formatterAndSimplifierAnnotationArray);
        }
        #endregion SyntaxNode

        #region SyntaxToken
        public static SyntaxToken WithFormatterAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(_formatterAnnotationArray);
        }

        public static SyntaxToken WithSimplifierAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(_simplifierAnnotationArray);
        }

        public static SyntaxToken WithNavigationAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(_navigationAnnotationArray);
        }

        internal static SyntaxToken WithFormatterAndSimplifierAnnotations(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(_formatterAndSimplifierAnnotationArray);
        }

        public static SyntaxToken WithRenameAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(RenameAnnotation.Create());
        }
        #endregion SyntaxToken
    }
}
