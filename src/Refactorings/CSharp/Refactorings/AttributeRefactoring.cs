// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AttributeRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, AttributeSyntax attribute)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.GeneratePropertyForDebuggerDisplayAttribute)
                && (context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(attribute)))
            {
                await GeneratePropertyForDebuggerDisplayAttributeRefactoring.ComputeRefactoringAsync(context, attribute).ConfigureAwait(false);
            }
        }
    }
}