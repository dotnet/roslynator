// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ParameterListRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ParameterListSyntax parameterList)
        {
            if (parameterList.Parameters.Count == 0)
                return;

            DuplicateParameterRefactoring.Refactor(context, parameterList);

            if (parameterList.IsSingleline())
            {
                if (parameterList.Parameters.Count > 1)
                {
                    context.RegisterRefactoring(
                        "Format each parameter on separate line",
                        cancellationToken => FormatParameterListRefactoring.FormatEachParameterOnSeparateLineAsync(context.Document, parameterList, cancellationToken));
                }
            }
            else
            {
                context.RegisterRefactoring(
                    "Format all parameters on a single line",
                    cancellationToken => FormatParameterListRefactoring.FormatAllParametersOnSingleLineAsync(context.Document, parameterList, cancellationToken));
            }
        }
    }
}