// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class AddModifierRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            SyntaxKind modifierKind,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            ClassDeclarationSyntax newClassDeclaration = AddModifier(classDeclaration, modifierKind);

            return await document.ReplaceNodeAsync(classDeclaration, newClassDeclaration, cancellationToken).ConfigureAwait(false);
        }

        private static ClassDeclarationSyntax AddModifier(ClassDeclarationSyntax classDeclaration, SyntaxKind modifierKind)
        {
            SyntaxTokenList modifiers = classDeclaration.Modifiers;

            if (modifiers.Contains(modifierKind))
                return classDeclaration;

            if (modifiers.Any())
            {
                int partialIndex = modifiers.IndexOf(SyntaxKind.PartialKeyword);

                if (partialIndex != -1)
                {
                    SyntaxToken partialToken = modifiers[partialIndex];

                    modifiers = modifiers
                        .ReplaceAt(partialIndex, partialToken.WithoutLeadingTrivia())
                        .Insert(
                            partialIndex,
                            Token(modifierKind)
                                .WithLeadingTrivia(partialToken.LeadingTrivia)
                                .WithTrailingTrivia(SpaceTrivia()));

                    return classDeclaration.WithModifiers(modifiers);
                }
                else
                {
                    return classDeclaration
                        .AddModifiers(Token(modifierKind).WithLeadingTrivia(SpaceTrivia()));
                }
            }
            else
            {
                return classDeclaration.AddModifiers(Token(modifierKind));
            }
        }
    }
}