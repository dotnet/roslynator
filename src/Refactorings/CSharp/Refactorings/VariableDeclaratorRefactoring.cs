// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class VariableDeclaratorRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, VariableDeclaratorSyntax variableDeclarator)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InitializeFieldFromConstructor)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(variableDeclarator.Identifier))
            {
                InitializeFieldFromConstructorRefactoring.ComputeRefactoring(context, variableDeclarator);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceMethodGroupWithLambda))
                await ReplaceMethodGroupWithLambdaRefactoring.ComputeRefactoringAsync(context, variableDeclarator).ConfigureAwait(false);
        }
    }
}