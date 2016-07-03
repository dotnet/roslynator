// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ExpandEventRefactoring
    {
        public static bool CanRefactor(EventFieldDeclarationSyntax eventDeclaration)
        {
            return eventDeclaration.Parent != null
                && eventDeclaration.Parent.IsAnyKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration)
                && eventDeclaration.Declaration?.Variables.Count == 1;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            EventFieldDeclarationSyntax eventDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            EventDeclarationSyntax newNode = ExpandEvent(eventDeclaration)
                .WithTriviaFrom(eventDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(eventDeclaration, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static EventDeclarationSyntax ExpandEvent(EventFieldDeclarationSyntax eventDeclaration)
        {
            AccessorListSyntax accessorList = AccessorList(
                List(new AccessorDeclarationSyntax[]
                {
                    AccessorDeclaration(SyntaxKind.AddAccessorDeclaration, Block()),
                    AccessorDeclaration(SyntaxKind.RemoveAccessorDeclaration, Block()),
                }));

            accessorList = WhitespaceOrEndOfLineRemover.RemoveFrom(accessorList)
                .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine));

            VariableDeclaratorSyntax declarator = eventDeclaration.Declaration.Variables[0];

            return EventDeclaration(
                eventDeclaration.AttributeLists,
                eventDeclaration.Modifiers,
                eventDeclaration.Declaration.Type,
                null,
                declarator.Identifier,
                accessorList);
        }
    }
}
