// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class InitializerExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, InitializerExpressionSyntax initializer)
        {
            if (initializer.IsKind(SyntaxKind.ComplexElementInitializerExpression)
                && initializer.Parent?.IsKind(SyntaxKind.CollectionInitializerExpression) == true)
            {
                initializer = (InitializerExpressionSyntax)initializer.Parent;
            }

            if (initializer.Expressions.Count > 0
                && !initializer.IsKind(SyntaxKind.ComplexElementInitializerExpression)
                && initializer.Parent?.IsAnyKind(
                    SyntaxKind.ArrayCreationExpression,
                    SyntaxKind.ImplicitArrayCreationExpression,
                    SyntaxKind.ObjectCreationExpression,
                    SyntaxKind.CollectionInitializerExpression) == true)
            {
                if (initializer.IsSingleline(includeExteriorTrivia: false))
                {
                    context.RegisterRefactoring(
                        "Format initializer on multiple lines",
                        cancellationToken => FormatInitializerOnMultipleLinesAsync(
                            context.Document,
                            initializer,
                            cancellationToken));
                }
                else if (initializer.Expressions.All(expression => expression.IsSingleline()))
                {
                    context.RegisterRefactoring(
                        "Format initializer on a single line",
                        cancellationToken => FormatInitializerOnSingleLineAsync(
                            context.Document,
                            initializer,
                            cancellationToken));
                }
            }

            ExpandInitializerRefactoring.Register(context, initializer);
        }

        private static async Task<Document> FormatInitializerOnSingleLineAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            InitializerExpressionSyntax newInitializer = initializer
                .WithExpressions(
                    SeparatedList(
                        initializer.Expressions.Select(expression => expression.WithoutTrivia())))
                .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(initializer.Parent, GetNewExpression(newInitializer, (ExpressionSyntax)initializer.Parent));

            return document.WithSyntaxRoot(newRoot);
        }

        private static ExpressionSyntax GetNewExpression(InitializerExpressionSyntax initializer, ExpressionSyntax parent)
        {
            switch (parent.Kind())
            {
                case SyntaxKind.ObjectCreationExpression:
                    {
                        var expression = (ObjectCreationExpressionSyntax)parent;

                        ObjectCreationExpressionSyntax newNode = expression
                            .WithInitializer(initializer);

                        if (newNode.ArgumentList != null)
                            return newNode.WithArgumentList(newNode.ArgumentList.WithoutTrailingTrivia());
                        else
                            return newNode.WithType(newNode.Type.WithoutTrailingTrivia());
                    }
                case SyntaxKind.ArrayCreationExpression:
                    {
                        var expression = (ArrayCreationExpressionSyntax)parent;

                        return expression
                            .WithInitializer(initializer)
                            .WithType(expression.Type.WithoutTrailingTrivia());
                    }
                case SyntaxKind.ImplicitArrayCreationExpression:
                    {
                        var expression = (ImplicitArrayCreationExpressionSyntax)parent;

                        return expression
                            .WithInitializer(initializer)
                            .WithCloseBracketToken(expression.CloseBracketToken.WithoutTrailingTrivia());
                    }
            }

            return null;
        }

        private static async Task<Document> FormatInitializerOnMultipleLinesAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            InitializerExpressionSyntax newInitializer = GetMultilineInitializer(initializer)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(initializer, newInitializer);

            return document.WithSyntaxRoot(newRoot);
        }

        private static InitializerExpressionSyntax GetMultilineInitializer(InitializerExpressionSyntax initializer)
        {
            SyntaxNode parent = initializer.Parent;

            if (parent.IsKind(SyntaxKind.ObjectCreationExpression)
                && !initializer.IsKind(SyntaxKind.CollectionInitializerExpression))
            {
                return initializer
                    .WithExpressions(
                        SeparatedList(
                            initializer.Expressions.Select(expression => expression.WithLeadingTrivia(SyntaxHelper.NewLine))))
                    .WithAdditionalAnnotations(Formatter.Annotation);
            }

            SyntaxTriviaList indent = initializer.GetIndentTrivia();
            SyntaxTriviaList indent2 = indent.Add(SyntaxHelper.DefaultIndent);

            indent = indent.Insert(0, SyntaxHelper.NewLine);
            indent2 = indent2.Insert(0, SyntaxHelper.NewLine);

            return initializer
                .WithExpressions(
                    SeparatedList(
                        initializer.Expressions.Select(expression => expression.WithLeadingTrivia(indent2))))
                .WithOpenBraceToken(initializer.OpenBraceToken.WithLeadingTrivia(indent))
                .WithCloseBraceToken(initializer.CloseBraceToken.WithLeadingTrivia(indent))
                .WithAdditionalAnnotations(Formatter.Annotation);
        }
    }
}
