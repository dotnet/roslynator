// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class FormatInitializerOnMultipleLinesRefactoring
    {
        public static async Task<Document> RefactorAsync(
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
