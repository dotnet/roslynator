// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.MakeMemberAbstract
{
    internal static class MakeIndexerAbstractRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            SyntaxTokenList modifiers = indexerDeclaration.Modifiers;

            if (modifiers.ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword))
                return;

            if ((indexerDeclaration.Parent as ClassDeclarationSyntax)?.Modifiers.Contains(SyntaxKind.AbstractKeyword) != true)
                return;

            context.RegisterRefactoring(
                "Make indexer abstract",
                cancellationToken => RefactorAsync(context.Document, indexerDeclaration, cancellationToken));
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
                .InsertModifier(SyntaxKind.AbstractKeyword)
                .RemoveModifier(SyntaxKind.VirtualKeyword)
                .WithTriviaFrom(indexerDeclaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken);
        }
    }
}
