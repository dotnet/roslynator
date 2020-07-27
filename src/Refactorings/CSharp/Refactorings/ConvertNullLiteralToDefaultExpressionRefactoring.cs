// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertNullLiteralToDefaultExpressionRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (!context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(expression))
                return;

            if (!expression.IsKind(SyntaxKind.NullLiteralExpression))
                return;

            if (expression.IsParentKind(SyntaxKind.EqualsValueClause)
                && expression.Parent.IsParentKind(SyntaxKind.Parameter)
                && object.ReferenceEquals(expression, ((ParameterSyntax)expression.Parent.Parent).Default.Value))
            {
                return;
            }

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expression, context.CancellationToken).ConvertedType;

            if (typeSymbol?.SupportsExplicitDeclaration() != true)
                return;

            context.RegisterRefactoring(
                $"Convert to 'default({SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, expression.SpanStart, SymbolDisplayFormats.DisplayName)})'",
                cancellationToken => RefactorAsync(context.Document, expression, typeSymbol, cancellationToken),
                RefactoringIdentifiers.ConvertNullLiteralToDefaultExpression);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            TypeSyntax type = typeSymbol.ToMinimalTypeSyntax(semanticModel, expression.SpanStart);

            DefaultExpressionSyntax defaultExpression = DefaultExpression(type).WithTriviaFrom(expression);

            return await document.ReplaceNodeAsync(expression, defaultExpression, cancellationToken).ConfigureAwait(false);
        }
    }
}

