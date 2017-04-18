// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IndexerDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MarkContainingClassAsAbstract)
                && indexerDeclaration.HeaderSpan().Contains(context.Span)
                && MarkContainingClassAsAbstractRefactoring.CanRefactor(indexerDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark containing class as abstract",
                    cancellationToken => MarkContainingClassAsAbstractRefactoring.RefactorAsync(context.Document, indexerDeclaration, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MakeMemberAbstract)
                && indexerDeclaration.HeaderSpan().Contains(context.Span)
                && MakeMemberAbstractRefactoring.CanRefactor(indexerDeclaration))
            {
                context.RegisterRefactoring(
                    "Make indexer abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, indexerDeclaration, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MakeMemberVirtual)
                && indexerDeclaration.HeaderSpan().Contains(context.Span))
            {
                MakeMemberVirtualRefactoring.ComputeRefactoring(context, indexerDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CopyDocumentationCommentFromBaseMember)
                && indexerDeclaration.HeaderSpan().Contains(context.Span))
            {
                await CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoringAsync(context, indexerDeclaration).ConfigureAwait(false);
            }
        }
    }
}