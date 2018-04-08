// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

            if (arguments.Count == 0)
                return;

            await AddOrRemoveParameterNameRefactoring.ComputeRefactoringsAsync(context, argumentList).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.DuplicateArgument))
            {
                var refactoring = new DuplicateArgumentRefactoring(argumentList);
                refactoring.ComputeRefactoring(context);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.FormatArgumentList)
                && (context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(argumentList)))
            {
                if (argumentList.IsSingleLine())
                {
                    if (arguments.Count > 1)
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