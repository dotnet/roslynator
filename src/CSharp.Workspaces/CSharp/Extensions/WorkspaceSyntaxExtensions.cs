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
    /// <summary>
    /// A set of extension methods for syntax. These methods are dependent on the workspace layer.
    /// </summary>
    public static class WorkspaceSyntaxExtensions
    {
        private const string NavigationAnnotationKind = "CodeAction_Navigation";

        internal static SyntaxAnnotation NavigationAnnotation { get; } = new SyntaxAnnotation(NavigationAnnotationKind);

        private static readonly SyntaxAnnotation[] _formatterAnnotationArray = new SyntaxAnnotation[] { Formatter.Annotation };

        private static readonly SyntaxAnnotation[] _simplifierAnnotationArray = new SyntaxAnnotation[] { Simplifier.Annotation };

        private static readonly SyntaxAnnotation[] _renameAnnotationArray = new SyntaxAnnotation[] { RenameAnnotation.Create() };

        private static readonly SyntaxAnnotation[] _navigationAnnotationArray = new SyntaxAnnotation[] { NavigationAnnotation };

        private static readonly SyntaxAnnotation[] _formatterAndSimplifierAnnotationArray = new SyntaxAnnotation[] { Formatter.Annotation, Simplifier.Annotation };

        #region ExpressionSyntax
        /// <summary>
        /// Creates parenthesized expression that is parenthesizing the specified expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="includeElasticTrivia">If true, add elastic trivia.</param>
        /// <param name="simplifiable">If true, attach <see cref="Simplifier.Annotation"/> to the parenthesized expression.</param>
        /// <returns></returns>
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
        internal static MemberAccessExpressionSyntax QualifyWithThis(this SimpleNameSyntax simpleName, bool simplifiable = true)
        {
            return SimpleMemberAccessExpression(ThisExpression(), simpleName).WithSimplifierAnnotationIf(simplifiable);
        }
        #endregion SimpleNameSyntax

        #region SyntaxNode
        /// <summary>
        /// Creates a new node with the <see cref="Formatter.Annotation"/> attached.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static TNode WithFormatterAnnotation<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithAdditionalAnnotations(_formatterAnnotationArray);
        }

        /// <summary>
        /// Creates a new node with the <see cref="Simplifier.Annotation"/> attached.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
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

        internal static TNode WithFormatterAndSimplifierAnnotation<TNode>(this TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node.WithAdditionalAnnotations(_formatterAndSimplifierAnnotationArray);
        }
        #endregion SyntaxNode

        #region SyntaxToken
        /// <summary>
        /// Adds <see cref="Formatter.Annotation"/> to the specified token, creating a new token of the same type with the <see cref="Formatter.Annotation"/> on it.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static SyntaxToken WithFormatterAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(_formatterAnnotationArray);
        }

        /// <summary>
        /// Adds <see cref="Simplifier.Annotation"/> to the specified token, creating a new token of the same type with the <see cref="Simplifier.Annotation"/> on it.
        /// "Rename" annotation is specified by <see cref="RenameAnnotation.Kind"/>.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static SyntaxToken WithSimplifierAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(_simplifierAnnotationArray);
        }

        internal static SyntaxToken WithNavigationAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(_navigationAnnotationArray);
        }

        internal static SyntaxToken WithFormatterAndSimplifierAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(_formatterAndSimplifierAnnotationArray);
        }

        /// <summary>
        /// Adds "rename" annotation to the specified token, creating a new token of the same type with the "rename" annotation on it.
        /// "Rename" annotation is specified by <see cref="RenameAnnotation.Kind"/>.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static SyntaxToken WithRenameAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(_renameAnnotationArray);
        }
        #endregion SyntaxToken
    }
}
