// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class VariableDeclaratorRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, VariableDeclaratorSyntax variableDeclarator)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CheckExpressionForNull)
                && context.Span.IsContainedInSpanOrBetweenSpans(variableDeclarator.Identifier))
            {
                await CheckExpressionForNullRefactoring.ComputeRefactoringAsync(context, variableDeclarator).ConfigureAwait(false);
            }
        }
    }
}