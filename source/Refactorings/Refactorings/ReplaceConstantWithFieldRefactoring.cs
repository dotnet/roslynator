// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceConstantWithFieldRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            FieldDeclarationSyntax field,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            FieldDeclarationSyntax newField = field
                .WithModifiers(GetModifiers(field, semanticModel, cancellationToken))
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(field, newField, cancellationToken).ConfigureAwait(false);
        }

        private static SyntaxTokenList GetModifiers(
            FieldDeclarationSyntax field,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxToken constModifier = field.Modifiers.FirstOrDefault(f => f.IsKind(SyntaxKind.ConstKeyword));

            var parentMember = (MemberDeclarationSyntax)field.Parent;

            if (parentMember != null
                && semanticModel.GetDeclaredSymbol(parentMember, cancellationToken)?.IsStatic == true)
            {
                return field.Modifiers.ReplaceRange(
                    constModifier,
                    new SyntaxToken[]
                    {
                        StaticKeyword().WithLeadingTrivia(constModifier.LeadingTrivia),
                        ReadOnlyKeyword().WithTrailingTrivia(constModifier.TrailingTrivia)
                    });
            }
            else
            {
                return field.Modifiers.Replace(
                    constModifier,
                    ReadOnlyKeyword().WithTriviaFrom(constModifier));
            }
        }
    }
}
