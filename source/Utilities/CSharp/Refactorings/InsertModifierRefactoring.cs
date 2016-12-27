// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class InsertModifierRefactoring
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

            SyntaxTokenList modifiers = classDeclaration.Modifiers;

            Debug.Assert(!modifiers.Contains(modifierKind), modifierKind.ToString());

            if (!modifiers.Contains(modifierKind))
            {
                SyntaxToken modifier = Token(modifierKind);

                ClassDeclarationSyntax newClassDeclaration = ModifierUtility.InsertModifier(classDeclaration, modifier);

                return await document.ReplaceNodeAsync(classDeclaration, newClassDeclaration, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return document;
            }
        }
    }
}