// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceConstantWithFieldRefactoring
    {
        public const string Title = "Replace constant with field";

        public static async Task<Document> RefactorAsync(
            Document document,
            FieldDeclarationSyntax field,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            FieldDeclarationSyntax newField = field
                .RemoveModifier(SyntaxKind.ConstKeyword)
                .InsertModifier(SyntaxKind.ReadOnlyKeyword);

            var containingDeclaration = (MemberDeclarationSyntax)field.Parent;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (semanticModel.GetDeclaredSymbol(containingDeclaration, cancellationToken)?.IsStatic == true)
            {
                newField = newField.InsertModifier(SyntaxKind.StaticKeyword);
            }

            newField = newField.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(field, newField, cancellationToken).ConfigureAwait(false);
        }
    }
}
