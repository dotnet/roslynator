// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.MakeMemberVirtual
{
    internal static class MakeIndexerVirtualRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (!indexerDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
                return;

            var classDeclaration = indexerDeclaration.Parent as ClassDeclarationSyntax;

            if (classDeclaration == null)
                return;

            if (classDeclaration.Modifiers.Contains(SyntaxKind.SealedKeyword))
                return;

            context.RegisterRefactoring(
                "Make indexer virtual",
                cancellationToken => RefactorAsync(context.Document, indexerDeclaration, cancellationToken));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IndexerDeclarationSyntax indexerDeclaration,
            CancellationToken cancellationToken)
        {
            IndexerDeclarationSyntax newNode = indexerDeclaration
                .WithSemicolonToken(default(SyntaxToken))
                .WithAccessorList(MakeMemberAbstractHelper.ExpandAccessorList(indexerDeclaration.AccessorList))
                .WithModifiers(indexerDeclaration.Modifiers.Replace(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword))
                .WithTriviaFrom(indexerDeclaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken);
        }
    }
}