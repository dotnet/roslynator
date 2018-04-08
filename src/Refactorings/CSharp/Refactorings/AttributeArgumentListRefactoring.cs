// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AttributeArgumentListRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, AttributeArgumentListSyntax argumentList)
        {
            if (!argumentList.Arguments.Any())
                return;

            await AttributeArgumentParameterNameRefactoring.ComputeRefactoringsAsync(context, argumentList).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.DuplicateArgument))
                DuplicateAttributeArgumentRefactoring.ComputeRefactoring(context, argumentList);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.FormatArgumentList)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(argumentList))
            {
                if (argumentList.IsSingleLine())
                {
                    if (argumentList.Arguments.Count > 1)
                    {
                        context.RegisterRefactoring(
                            "Format arguments on separate lines",
                            ct => SyntaxFormatter.ToMultiLineAsync(context.Document, argumentList, ct));
                    }
                }
                else
                {
                    context.RegisterRefactoring(
                        "Format arguments on a single line",
                        ct => SyntaxFormatter.ToSingleLineAsync(context.Document, argumentList, ct));
                }
            }
        }
    }
}