// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceNullLiteralExpressionWithDefaultExpressionRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (expression?.Kind() != SyntaxKind.NullLiteralExpression)
                return;

            if (!context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(expression))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expression, context.CancellationToken).ConvertedType;

            if (typeSymbol?.SupportsExplicitDeclaration() != true)
                return;

            context.RegisterRefactoring(
                $"Replace 'null' with 'default({SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, expression.SpanStart, SymbolDisplayFormats.Default)})'",
                cancellationToken => RefactorAsync(context.Document, expression, typeSymbol, cancellationToken));
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            TypeSyntax type = typeSymbol.ToMinimalTypeSyntax(semanticModel, expression.SpanStart);

            DefaultExpressionSyntax defaultExpression = DefaultExpression(type).WithTriviaFrom(expression);

            return await document.ReplaceNodeAsync(expression, defaultExpression, cancellationToken).ConfigureAwait(false);
        }
    }
}

