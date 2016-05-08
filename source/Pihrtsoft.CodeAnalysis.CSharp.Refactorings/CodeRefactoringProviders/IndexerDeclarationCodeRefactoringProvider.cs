// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(IndexerDeclarationCodeRefactoringProvider))]
    public class IndexerDeclarationCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            IndexerDeclarationSyntax indexerDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<IndexerDeclarationSyntax>();

            if (indexerDeclaration == null)
                return;

            MemberDeclarationRefactoring.Remove(context, indexerDeclaration);
            MemberDeclarationRefactoring.Duplicate(context, indexerDeclaration);

            if (MakeMemberAbstractRefactoring.CanRefactor(context, indexerDeclaration))
            {
                context.RegisterRefactoring(
                    $"Make indexer abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, indexerDeclaration, cancellationToken));
            }
        }
    }
}