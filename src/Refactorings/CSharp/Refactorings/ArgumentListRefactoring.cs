// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.AddOrRemoveParameterName;
using Roslynator.CSharp.Refactorings.NodeInList;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ArgumentListRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArgumentListSyntax argumentList)
        {
            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (!arguments.Any())
                return;

            await AddOrRemoveParameterNameRefactoring.ComputeRefactoringsAsync(context, argumentList).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.DuplicateArgument))
            {
                var refactoring = new DuplicateArgumentRefactoring(argumentList);
                refactoring.ComputeRefactoring(context, RefactoringIdentifiers.DuplicateArgument);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapArguments)
                && (context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(argumentList)))
            {
                if (argumentList.IsSingleLine())
                {
                    if (arguments.Count > 1)
                    {
                        context.RegisterRefactoring(
                            "Wrap arguments",
                            ct => SyntaxFormatter.WrapArgumentsAsync(context.Document, argumentList, ct),
                            RefactoringIdentifiers.WrapArguments);
                    }
                }
                else if (argumentList.DescendantTrivia(argumentList.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    context.RegisterRefactoring(
                        "Unwrap arguments",
                        ct => SyntaxFormatter.UnwrapExpressionAsync(context.Document, argumentList, ct),
                        RefactoringIdentifiers.WrapArguments);
                }
            }
        }
    }
}