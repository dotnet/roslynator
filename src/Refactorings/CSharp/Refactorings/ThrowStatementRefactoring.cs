// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.AddExceptionToDocumentationComment;
using Roslynator.CSharp.Refactorings.AddExceptionToDocumentationComment;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ThrowStatementRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ThrowStatementSyntax throwStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddExceptionToDocumentationComment)
                && context.Span.IsContainedInSpanOrBetweenSpans(throwStatement))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                INamedTypeSymbol exceptionSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Exception);

                AddExceptionToDocumentationCommentAnalysisResult analysis = AddExceptionToDocumentationCommentAnalysis.Analyze(throwStatement, exceptionSymbol, semanticModel, context.CancellationToken);

                if (analysis.Success)
                {
                    context.RegisterRefactoring(
                        "Add exception to documentation comment",
                        cancellationToken => AddExceptionToDocumentationCommentRefactoring.RefactorAsync(context.Document, analysis, cancellationToken));
                }
            }
        }
    }
}