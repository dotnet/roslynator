// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.MakeMemberAbstract;
using Roslynator.CSharp.Refactorings.MakeMemberVirtual;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IndexerDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertBlockBodyToExpressionBody)
                && context.SupportsCSharp6
                && ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(indexerDeclaration, context.Span))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    ct => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, indexerDeclaration, ct),
                    RefactoringDescriptors.ConvertBlockBodyToExpressionBody);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.MakeMemberAbstract)
                && context.Span.IsEmptyAndContainedInSpan(indexerDeclaration.ThisKeyword))
            {
                MakeIndexerAbstractRefactoring.ComputeRefactoring(context, indexerDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.MakeMemberVirtual)
                && indexerDeclaration.HeaderSpan().Contains(context.Span))
            {
                MakeIndexerVirtualRefactoring.ComputeRefactoring(context, indexerDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.CopyDocumentationCommentFromBaseMember)
                && indexerDeclaration.HeaderSpan().Contains(context.Span)
                && !indexerDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoring(context, indexerDeclaration, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddParameterToInterfaceMember)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(indexerDeclaration.ThisKeyword))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                foreach (CodeAction codeAction in AddParameterToInterfaceMemberRefactoring.ComputeRefactoringForImplicitImplementation(
                    new CommonFixContext(context.Document, EquivalenceKey.Create(RefactoringDescriptors.AddParameterToInterfaceMember), semanticModel, context.CancellationToken),
                    indexerDeclaration))
                {
                    context.RegisterRefactoring(codeAction);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddMemberToInterface)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(indexerDeclaration.ThisKeyword))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                AddMemberToInterfaceRefactoring.ComputeRefactoring(context, indexerDeclaration, semanticModel);
            }
        }
    }
}
