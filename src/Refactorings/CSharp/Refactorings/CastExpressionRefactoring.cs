// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CastExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, CastExpressionSyntax castExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceCastWithAs)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(castExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(castExpression.Type, context.CancellationToken);

                if (typeSymbol?.IsErrorType() == false
                    && typeSymbol.IsReferenceTypeOrNullableType())
                {
                    Document document = context.Document;

                    context.RegisterRefactoring(
                        "Replace cast with 'as'",
                        cancellationToken =>
                        {
                            BinaryExpressionSyntax newNode = CSharpFactory.AsExpression(castExpression.Expression.WithTriviaFrom(castExpression.Type), castExpression.Type.WithTriviaFrom(castExpression.Expression))
                                .WithTriviaFrom(castExpression)
                                .WithFormatterAnnotation();

                            return document.ReplaceNodeAsync(castExpression, newNode, cancellationToken);
                        },
                        RefactoringIdentifiers.ReplaceCastWithAs);
                }
            }
        }
    }
}