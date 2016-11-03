// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpandEventRefactoring
    {
        public static bool CanRefactor(EventFieldDeclarationSyntax eventDeclaration)
        {
            return eventDeclaration.Parent != null
                && eventDeclaration.Parent.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration)
                && eventDeclaration.Declaration?.Variables.Count == 1;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            EventFieldDeclarationSyntax eventDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            EventDeclarationSyntax newNode = ExpandEvent(eventDeclaration)
                .WithTriviaFrom(eventDeclaration)
                .WithFormatterAnnotation();

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

            accessorList = SyntaxRemover.RemoveWhitespaceOrEndOfLine(accessorList)
                .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLineTrivia()));

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
