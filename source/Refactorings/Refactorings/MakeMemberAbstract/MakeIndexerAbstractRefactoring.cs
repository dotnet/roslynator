// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.MakeMemberAbstract
{
    internal static class MakeIndexerAbstractRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (!CanRefactor(indexerDeclaration))
                return;

            context.RegisterRefactoring(
                "Make indexer abstract",
                cancellationToken => RefactorAsync(context.Document, indexerDeclaration, cancellationToken));
        }

        public static bool CanRefactor(IndexerDeclarationSyntax indexerDeclaration)
        {
            SyntaxTokenList modifiers = indexerDeclaration.Modifiers;

            return !modifiers.ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword)
                && (indexerDeclaration.Parent as ClassDeclarationSyntax)?.Modifiers.Contains(SyntaxKind.AbstractKeyword) == true;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            IndexerDeclarationSyntax indexerDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            AccessorListSyntax accessorList = AccessorList();

            if (indexerDeclaration.ExpressionBody != null)
            {
                accessorList = accessorList
                    .AddAccessors(
                        AutoGetAccessorDeclaration());
            }
            else
            {
                AccessorDeclarationSyntax getter = indexerDeclaration.Getter();
                if (getter != null)
                {
                    accessorList = accessorList.AddAccessors(getter
                       .WithBody(null)
                       .WithSemicolonToken(SemicolonToken()));
                }

                AccessorDeclarationSyntax setter = indexerDeclaration.Setter();
                if (setter != null)
                {
                    accessorList = accessorList.AddAccessors(setter
                       .WithBody(null)
                       .WithSemicolonToken(SemicolonToken()));
                }
            }

            IndexerDeclarationSyntax newNode = indexerDeclaration
                .WithExpressionBody(null)
                .WithSemicolonToken(default(SyntaxToken))
                .WithAccessorList(accessorList)
                .InsertModifier(SyntaxKind.AbstractKeyword, ModifierComparer.Instance)
                .RemoveModifier(SyntaxKind.VirtualKeyword)
                .WithTriviaFrom(indexerDeclaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken);
        }
    }
}
