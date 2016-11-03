// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveModifierRefactoring
    {
        public static async Task<Document> RemovePartialModifierAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newNode = RemoveModifier(node, SyntaxKind.PartialKeyword)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(node, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> RemoveSealedModifierAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newNode = RemoveModifier(node, SyntaxKind.SealedKeyword)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(node, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode RemoveModifier(SyntaxNode node, SyntaxKind modifierKind)
        {
            switch (node.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    {
                        var declaration = (PropertyDeclarationSyntax)node;

                        return declaration
                            .WithModifiers(RemoveModifier(declaration.Modifiers, modifierKind));
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var declaration = (MethodDeclarationSyntax)node;

                        return declaration
                            .WithModifiers(RemoveModifier(declaration.Modifiers, modifierKind));
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var declaration = (ClassDeclarationSyntax)node;

                        return declaration
                            .WithModifiers(RemoveModifier(declaration.Modifiers, modifierKind));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var declaration = (StructDeclarationSyntax)node;

                        return declaration
                            .WithModifiers(RemoveModifier(declaration.Modifiers, modifierKind));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var declaration = (InterfaceDeclarationSyntax)node;

                        return declaration
                            .WithModifiers(RemoveModifier(declaration.Modifiers, modifierKind));
                    }
            }

            Debug.Assert(false, node.Kind().ToString());

            return node;
        }

        private static SyntaxTokenList RemoveModifier(SyntaxTokenList modifiers, SyntaxKind modifierKind)
        {
            int i = modifiers.IndexOf(modifierKind);

            if (i == -1)
                return modifiers;

            SyntaxToken modifier = modifiers[i];

            SyntaxTriviaList leading = modifier.LeadingTrivia;
            SyntaxTriviaList trailing = modifier.TrailingTrivia;

            if (i > 0
                && modifiers[i].LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && modifiers[i - 1].TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                modifiers = modifiers.Replace(modifiers[i - 1], modifiers[i - 1].WithoutTrailingTrivia());
                leading = TriviaList();
            }

            if (i < (modifiers.Count - 1)
                && modifiers[i].TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && modifiers[i + 1].LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                modifiers = modifiers.Replace(modifiers[i + 1], modifiers[i + 1].WithoutLeadingTrivia());
                trailing = TriviaList();
            }

            SyntaxToken newToken = MissingToken(leading, SyntaxKind.None, trailing);

            return modifiers.Replace(modifiers[i], newToken);
        }
    }
}
