// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.IntroduceAndInitialize;
using Roslynator.CSharp.Refactorings.NodeInList;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ParameterListRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ParameterListSyntax parameterList)
        {
            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (parameters.Any())
            {
                if (context.IsRefactoringEnabled(RefactoringDescriptors.CopyParameter))
                {
                    var refactoring = new CopyParameterRefactoring(parameterList);
                    refactoring.ComputeRefactoring(context, RefactoringDescriptors.CopyParameter);
                }

                if (context.IsRefactoringEnabled(RefactoringDescriptors.CheckParameterForNull)
                    && SeparatedSyntaxListSelection<ParameterSyntax>.TryCreate(parameterList.Parameters, context.Span, out SeparatedSyntaxListSelection<ParameterSyntax> selectedParameters))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                    CheckParameterForNullRefactoring.ComputeRefactoring(context, selectedParameters, semanticModel);
                }

                if (context.IsAnyRefactoringEnabled(
                    RefactoringDescriptors.IntroduceAndInitializeField,
                    RefactoringDescriptors.IntroduceAndInitializeProperty))
                {
                    IntroduceAndInitializeRefactoring.ComputeRefactoring(context, parameterList);
                }

                if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapParameters)
                    && (context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(parameterList)))
                {
                    if (parameterList.IsSingleLine())
                    {
                        if (parameters.Count > 1)
                        {
                            context.RegisterRefactoring(
                                "Wrap parameters",
                                ct => SyntaxFormatter.WrapParametersAsync(context.Document, parameterList, ct),
                                RefactoringDescriptors.WrapParameters);
                        }
                    }
                    else if (parameterList.DescendantTrivia(parameterList.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.RegisterRefactoring(
                            "Unwrap parameters",
                            ct => SyntaxFormatter.UnwrapExpressionAsync(context.Document, parameterList, ct),
                            RefactoringDescriptors.WrapParameters);
                    }
                }
            }
        }
    }
}
