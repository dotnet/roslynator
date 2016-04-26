// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(InitializerExpressionCodeRefactoringProvider))]
    public class InitializerExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            InitializerExpressionSyntax initializerExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InitializerExpressionSyntax>();

            if (initializerExpression == null)
                return;

            if (initializerExpression.Expressions.Count == 0)
                return;

            if (initializerExpression.Parent == null)
                return;

            if (!initializerExpression.Parent.IsAnyKind(
                    SyntaxKind.ArrayCreationExpression,
                    SyntaxKind.ImplicitArrayCreationExpression,
                    SyntaxKind.ObjectCreationExpression))
            {
                Debug.Assert(false, initializerExpression.Parent.Kind().ToString());

                return;
            }

            if (initializerExpression.IsSingleline(includeExteriorTrivia: false))
            {
                context.RegisterRefactoring(
                    "Format initializer on multiple lines",
                    cancellationToken => FormatInitializerOnMultipleLinesAsync(
                        context.Document,
                        initializerExpression,
                        cancellationToken));
            }
            else if (initializerExpression.Expressions.All(expression => expression.IsSingleline()))
            {
                context.RegisterRefactoring(
                    "Format initializer on a single line",
                    cancellationToken => FormatInitializerOnSingleLineAsync(
                        context.Document,
                        initializerExpression,
                        cancellationToken));
            }
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

            InitializerExpressionSyntax newInitializer = GetSinglelineInitializer(initializer)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(initializer, newInitializer);

            return document.WithSyntaxRoot(newRoot);
        }

        private static InitializerExpressionSyntax GetSinglelineInitializer(InitializerExpressionSyntax initializer)
        {
            SyntaxNode parent = initializer.Parent;

            if (parent.IsKind(SyntaxKind.ObjectCreationExpression))
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
