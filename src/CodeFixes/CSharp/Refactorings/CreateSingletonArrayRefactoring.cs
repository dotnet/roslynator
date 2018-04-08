// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CreateSingletonArrayRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol elementType,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ArrayCreationExpressionSyntax newNode = ArrayCreationExpression(
                ArrayType(elementType.ToMinimalTypeSyntax(semanticModel, expression.SpanStart), SingletonList(ArrayRankSpecifier())),
                ArrayInitializerExpression(SingletonSeparatedList(expression.WithoutTrivia())));

            newNode = newNode
                .WithTriviaFrom(expression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(expression, newNode, cancellationToken);
        }
    }
}