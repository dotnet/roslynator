// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactorings.IntroduceAndInitialize;
using Pihrtsoft.CodeAnalysis.CSharp.Refactorings.NodeInList;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class ParameterListRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ParameterListSyntax parameterList)
        {
            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (parameters.Count == 0)
                return;

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.DuplicateParameter))
            {
                var refactoring = new DuplicateParameterRefactoring(parameterList);
                refactoring.ComputeRefactoring(context, parameterList);
            }

            if (context.Settings.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.IntroduceAndInitializeField,
                RefactoringIdentifiers.IntroduceAndInitializeProperty))
            {
                IntroduceAndInitializeRefactoring.ComputeRefactoring(context, parameterList);
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.FormatParameterList))
            {
                if (parameterList.IsSingleLine())
                {
                    if (parameters.Count > 1)
                    {
                        context.RegisterRefactoring(
                            "Format each parameter on separate line",
                            cancellationToken => FormatParameterListRefactoring.FormatEachParameterOnSeparateLineAsync(context.Document, parameterList, cancellationToken));
                    }
                }
                else
                {
                    string title = parameters.Count == 1
                        ? "Format parameter on a single line"
                        : "Format all parameters on a single line";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken => FormatParameterListRefactoring.FormatAllParametersOnSingleLineAsync(context.Document, parameterList, cancellationToken));
                }
            }
        }
    }
}