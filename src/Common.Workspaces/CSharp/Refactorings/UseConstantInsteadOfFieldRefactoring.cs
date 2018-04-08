// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseConstantInsteadOfFieldRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            FieldDeclarationSyntax fieldDeclaration,
            CancellationToken cancellationToken)
        {
            FieldDeclarationSyntax newNode = fieldDeclaration
                .InsertModifier(SyntaxKind.ConstKeyword)
                .RemoveModifiers(SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword);

            return document.ReplaceNodeAsync(fieldDeclaration, newNode, cancellationToken);
        }
    }
}
