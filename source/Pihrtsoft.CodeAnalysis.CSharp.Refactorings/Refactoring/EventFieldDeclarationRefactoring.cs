// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class EventFieldDeclarationRefactoring
    {
        public static bool CanExpand(
            EventFieldDeclarationSyntax eventDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return eventDeclaration.Parent != null
                && eventDeclaration.Parent.IsAnyKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration)
                && eventDeclaration.Declaration?.Variables.Count == 1;
        }

        public static async Task<Document> ExpandAsync(
            Document document,
            EventFieldDeclarationSyntax eventDeclaration,
            CancellationToken cancellationToken)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            EventDeclarationSyntax newNode = Expand(eventDeclaration)
                .WithTriviaFrom(eventDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(eventDeclaration, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static EventDeclarationSyntax Expand(EventFieldDeclarationSyntax eventDeclaration)
        {
            AccessorListSyntax accessorList = AccessorList(
                List(new AccessorDeclarationSyntax[]
                {
                    AccessorDeclaration(SyntaxKind.AddAccessorDeclaration, Block()),
                    AccessorDeclaration(SyntaxKind.RemoveAccessorDeclaration, Block()),
                }));

            accessorList = RemoveWhitespaceOrEndOfLineSyntaxRewriter.VisitNode(accessorList)
                .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(SyntaxHelper.NewLine));

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
