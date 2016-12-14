// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatInitializerOnMultipleLinesRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            InitializerExpressionSyntax newInitializer = GetMultilineInitializer(initializer)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(initializer, newInitializer, cancellationToken).ConfigureAwait(false);
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
                            initializer.Expressions.Select(expression => expression.WithLeadingTrivia(CSharpFactory.NewLineTrivia()))))
                    .WithFormatterAnnotation();
            }

            SyntaxTriviaList indent = SyntaxHelper.GetIndentTrivia(initializer);
            SyntaxTriviaList indent2 = indent.Add(CSharpFactory.IndentTrivia());

            indent = indent.Insert(0, CSharpFactory.NewLineTrivia());
            indent2 = indent2.Insert(0, CSharpFactory.NewLineTrivia());

            return initializer
                .WithExpressions(
                    SeparatedList(
                        initializer.Expressions.Select(expression => expression.WithLeadingTrivia(indent2))))
                .WithOpenBraceToken(initializer.OpenBraceToken.WithLeadingTrivia(indent))
                .WithCloseBraceToken(initializer.CloseBraceToken.WithLeadingTrivia(indent))
                .WithFormatterAnnotation();
        }
    }
}
