// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddCastExpressionRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax type = destinationType.ToMinimalTypeSyntax(semanticModel, expression.SpanStart);

            return RefactorAsync(document, expression, type, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            TypeSyntax destinationType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newExpression = expression
                .WithoutTrivia()
                .Parenthesize();

            ExpressionSyntax newNode = CastExpression(destinationType, newExpression)
                .WithTriviaFrom(expression)
                .Parenthesize();

            return document.ReplaceNodeAsync(expression, newNode, cancellationToken);
        }
    }
}

