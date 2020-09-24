// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertBlockBodyToExpressionBody)
                && context.SupportsCSharp6
                && ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(indexerDeclaration, context.Span))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    ct => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, indexerDeclaration, ct),
                    RefactoringIdentifiers.ConvertBlockBodyToExpressionBody);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MakeMemberAbstract)
                && context.Span.IsEmptyAndContainedInSpan(indexerDeclaration.ThisKeyword))
            {
                MakeIndexerAbstractRefactoring.ComputeRefactoring(context, indexerDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MakeMemberVirtual)
                && indexerDeclaration.HeaderSpan().Contains(context.Span))
            {
                MakeIndexerVirtualRefactoring.ComputeRefactoring(context, indexerDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CopyDocumentationCommentFromBaseMember)
                && indexerDeclaration.HeaderSpan().Contains(context.Span)
                && !indexerDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoring(context, indexerDeclaration, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddParameterToInterfaceMember)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(indexerDeclaration.ThisKeyword))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                foreach (CodeAction codeAction in AddParameterToInterfaceMemberRefactoring.ComputeRefactoringForImplicitImplementation(
                    new CommonFixContext(context.Document, RefactoringIdentifiers.AddParameterToInterfaceMember, semanticModel, context.CancellationToken),
                    indexerDeclaration))
                {
                    context.RegisterRefactoring(codeAction);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddMemberToInterface)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(indexerDeclaration.ThisKeyword))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                AddMemberToInterfaceRefactoring.ComputeRefactoring(context, indexerDeclaration, semanticModel);
            }
        }
    }
}