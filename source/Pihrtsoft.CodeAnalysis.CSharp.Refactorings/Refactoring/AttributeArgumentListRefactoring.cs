// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AttributeArgumentListRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, AttributeArgumentListSyntax argumentList)
        {
            if (argumentList.Arguments.Count == 0)
                return;

            await AttributeArgumentParameterNameRefactoring.ComputeRefactoringsAsync(context, argumentList);

            DuplicateAttributeArgumentRefactoring.ComputeRefactoring(context, argumentList);

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.FormatArgumentList))
            {
                if (argumentList.IsSingleline())
                {
                    if (argumentList.Arguments.Count > 1)
                    {
                        context.RegisterRefactoring(
                            "Format each argument on separate line",
                            cancellationToken =>
                            {
                                return FormatAttributeArgumentListRefactoring.FormatEachArgumentOnSeparateLineAsync(
                                    context.Document,
                                    argumentList,
                                    cancellationToken);
                            });
                    }
                }
                else
                {
                    string title = (argumentList.Arguments.Count == 1)
                            ? "Format argument on a single line"
                            : "Format all arguments on a single line";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken =>
                        {
                            return FormatAttributeArgumentListRefactoring.FormatAllArgumentsOnSingleLineAsync(
                                context.Document,
                                argumentList,
                                cancellationToken);
                        });
                }
            }
        }
    }
}