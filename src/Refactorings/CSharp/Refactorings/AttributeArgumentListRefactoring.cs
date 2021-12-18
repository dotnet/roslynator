// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
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

            if (context.IsRefactoringEnabled(RefactoringDescriptors.CopyArgument))
                CopyAttributeArgumentRefactoring.ComputeRefactoring(context, argumentList);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapArguments)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(argumentList))
            {
                if (argumentList.IsSingleLine())
                {
                    if (argumentList.Arguments.Count > 1)
                    {
                        context.RegisterRefactoring(
                            "Wrap arguments",
                            ct => SyntaxFormatter.WrapArgumentsAsync(context.Document, argumentList, ct),
                            RefactoringDescriptors.WrapArguments);
                    }
                }
                else if (argumentList.DescendantTrivia(argumentList.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    context.RegisterRefactoring(
                        "Unwrap arguments",
                        ct => SyntaxFormatter.UnwrapExpressionAsync(context.Document, argumentList, ct),
                        RefactoringDescriptors.WrapArguments);
                }
            }
        }
    }
}
