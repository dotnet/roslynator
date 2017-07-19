// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

            ExpressionSyntax newExpression = expression
                .WithoutTrivia()
                .Parenthesize()
                .WithSimplifierAnnotation();

            CastExpressionSyntax castExpression = SyntaxFactory.CastExpression(type, newExpression)
                .WithTriviaFrom(expression);

            return document.ReplaceNodeAsync(expression, castExpression, cancellationToken);
        }
    }
}

