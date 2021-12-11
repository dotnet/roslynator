// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.AddExceptionToDocumentationComment;
using Roslynator.CSharp.Refactorings.AddExceptionToDocumentationComment;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ThrowExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ThrowExpressionSyntax throwExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddExceptionElementToDocumentationComment)
                && context.Span.IsContainedInSpanOrBetweenSpans(throwExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                AddExceptionToDocumentationCommentAnalysisResult analysis = AddExceptionToDocumentationCommentAnalysis.Analyze(
                    throwExpression,
                    semanticModel.GetTypeByMetadataName("System.Exception"),
                    semanticModel,
                    context.CancellationToken);

                if (analysis.Success)
                {
                    context.RegisterRefactoring(
                        "Add 'exception' element to documentation comment",
                        ct => AddExceptionElementToDocumentationCommentRefactoring.RefactorAsync(context.Document, analysis, ct),
                        RefactoringDescriptors.AddExceptionElementToDocumentationComment);
                }
            }
        }
    }
}
