// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InitializerExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InitializerExpressionSyntax initializer)
        {
            if (initializer.IsKind(SyntaxKind.ComplexElementInitializerExpression)
                && initializer.Parent?.IsKind(SyntaxKind.CollectionInitializerExpression) == true)
            {
                initializer = (InitializerExpressionSyntax)initializer.Parent;
            }

            if (context.Span.IsEmpty
                || context.Span.IsBetweenSpans(initializer)
                || context.Span.IsBetweenSpans(initializer.Expressions))
            {
                SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.FormatInitializer)
                    && expressions.Any()
                    && !initializer.IsKind(SyntaxKind.ComplexElementInitializerExpression)
                    && initializer.Parent?.IsKind(
                        SyntaxKind.ArrayCreationExpression,
                        SyntaxKind.ImplicitArrayCreationExpression,
                        SyntaxKind.ObjectCreationExpression,
                        SyntaxKind.CollectionInitializerExpression) == true)
                {
                    if (initializer.IsSingleLine(includeExteriorTrivia: false))
                    {
                        context.RegisterRefactoring(
                            "Format initializer on multiple lines",
                            cancellationToken => FormatInitializerOnMultipleLinesRefactoring.RefactorAsync(
                                context.Document,
                                initializer,
                                cancellationToken));
                    }
                    else if (expressions.All(expression => expression.IsSingleLine()))
                    {
                        context.RegisterRefactoring(
                            "Format initializer on a single line",
                            cancellationToken => FormatInitializerOnSingleLineRefactoring.RefactorAsync(
                                context.Document,
                                initializer,
                                cancellationToken));
                    }
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandInitializer))
                    await ExpandInitializerRefactoring.ComputeRefactoringsAsync(context, initializer).ConfigureAwait(false);
            }
        }
    }
}
