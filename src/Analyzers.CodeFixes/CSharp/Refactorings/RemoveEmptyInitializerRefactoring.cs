// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEmptyInitializerRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreationExpression,
            CancellationToken cancellationToken)
        {
            ArgumentListSyntax argumentList = objectCreationExpression.ArgumentList;
            InitializerExpressionSyntax initializer = objectCreationExpression.Initializer;

            ObjectCreationExpressionSyntax newNode = objectCreationExpression.WithInitializer(null);
            if (argumentList == null)
            {
                TypeSyntax type = objectCreationExpression.Type;

                ArgumentListSyntax newArgumentList = SyntaxFactory.ArgumentList();

                IEnumerable<SyntaxTrivia> trivia = objectCreationExpression.DescendantTrivia(TextSpan.FromBounds(type.Span.End, initializer.Span.End));

                if (trivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                    newArgumentList = newArgumentList.WithTrailingTrivia(trivia);

                newNode = newNode
                    .WithType(type.WithoutTrailingTrivia())
                    .WithArgumentList(newArgumentList);
            }
            else
            {
                IEnumerable<SyntaxTrivia> trivia = objectCreationExpression.DescendantTrivia(TextSpan.FromBounds(argumentList.Span.End, initializer.Span.End));

                if (trivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    newNode = newNode.WithTrailingTrivia(trivia);
                }
                else
                {
                    newNode = newNode.WithoutTrailingTrivia();
                }
            }

            newNode = newNode
                .AppendToTrailingTrivia(initializer.GetTrailingTrivia())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(objectCreationExpression, newNode, cancellationToken);
        }
    }
}
