// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
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

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.FormatInitializer)
                && initializer.Expressions.Count > 0
                && !initializer.IsKind(SyntaxKind.ComplexElementInitializerExpression)
                && initializer.Parent?.IsAnyKind(
                    SyntaxKind.ArrayCreationExpression,
                    SyntaxKind.ImplicitArrayCreationExpression,
                    SyntaxKind.ObjectCreationExpression,
                    SyntaxKind.CollectionInitializerExpression) == true)
            {
                if (initializer.IsSingleline(includeExteriorTrivia: false))
                {
                    context.RegisterRefactoring(
                        "Format initializer on multiple lines",
                        cancellationToken => FormatInitializerOnMultipleLinesRefactoring.RefactorAsync(
                            context.Document,
                            initializer,
                            cancellationToken));
                }
                else if (initializer.Expressions.All(expression => expression.IsSingleline()))
                {
                    context.RegisterRefactoring(
                        "Format initializer on a single line",
                        cancellationToken => FormatInitializerOnSingleLineRefactoring.RefactorAsync(
                            context.Document,
                            initializer,
                            cancellationToken));
                }
            }

            await ExpandInitializerRefactoring.ComputeRefactoringsAsync(context, initializer);
        }
    }
}
