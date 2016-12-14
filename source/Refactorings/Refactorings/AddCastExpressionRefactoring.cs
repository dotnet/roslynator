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
        public static void RegisterRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol destinationType)
        {
            context.RegisterRefactoring(
                $"Cast to '{destinationType.ToDisplayString(DefaultSymbolDisplayFormat.Value)}'",
                cancellationToken =>
                {
                    return RefactorAsync(
                        context.Document,
                        expression,
                        destinationType,
                        cancellationToken);
                });
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            TypeSyntax type = CSharpFactory.Type(destinationType, semanticModel, expression.SpanStart);

            CastExpressionSyntax castExpression = SyntaxFactory.CastExpression(type, expression)
                .WithTriviaFrom(expression);

            return await document.ReplaceNodeAsync(expression, castExpression, cancellationToken).ConfigureAwait(false);
        }
    }
}

