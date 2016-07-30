// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.Comparers;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SpecifyImplicitTypeCodeFixProvider))]
    [Shared]
    public class ReorderModifiersCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.ReorderModifiers);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            SyntaxNode declaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(
                    SyntaxKind.ClassDeclaration,
                    SyntaxKind.ConstructorDeclaration,
                    SyntaxKind.ConversionOperatorDeclaration,
                    SyntaxKind.DelegateDeclaration,
                    SyntaxKind.DestructorDeclaration,
                    SyntaxKind.EnumDeclaration,
                    SyntaxKind.EventDeclaration,
                    SyntaxKind.EventFieldDeclaration,
                    SyntaxKind.FieldDeclaration,
                    SyntaxKind.IndexerDeclaration,
                    SyntaxKind.InterfaceDeclaration,
                    SyntaxKind.MethodDeclaration,
                    SyntaxKind.OperatorDeclaration,
                    SyntaxKind.PropertyDeclaration,
                    SyntaxKind.StructDeclaration);

            if (declaration == null)
                return;

            SyntaxTokenList modifiers = declaration.GetDeclarationModifiers();

            if (!CheckTriviaBetweenModifiers(modifiers))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Reorder modifiers",
                cancellationToken => ReorderModifiersAsync(context.Document, declaration, modifiers, cancellationToken),
                DiagnosticIdentifiers.ReorderModifiers + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static bool CheckTriviaBetweenModifiers(SyntaxTokenList modifiers)
        {
            for (int i = 0; i < modifiers.Count; i++)
            {
                if (i > 0
                    && modifiers[i].LeadingTrivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    return false;
                }

                if (i < (modifiers.Count - 1)
                    && modifiers[i].TrailingTrivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    return false;
                }
            }

            return true;
        }

        internal static async Task<Document> ReorderModifiersAsync(
            Document document,
            SyntaxNode declaration,
            SyntaxTokenList modifiers,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxTokenList newModifiers = SyntaxFactory.TokenList(
                modifiers.OrderBy(f => f, ModifierSorter.Instance));

            newModifiers = SetModifiersTrivia(newModifiers, modifiers);

            SyntaxNode newDeclaration = GetNewNode(declaration, newModifiers);

            SyntaxNode newRoot = oldRoot.ReplaceNode(declaration, newDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxTokenList SetModifiersTrivia(SyntaxTokenList modifiers, SyntaxTokenList oldModifiers)
        {
            for (int i = 0; i < modifiers.Count; i++)
            {
                if (i == 0)
                {
                    modifiers = modifiers.Replace(
                        modifiers[i],
                        modifiers[i].WithLeadingTrivia(oldModifiers[i].LeadingTrivia));
                }
                else
                {
                    modifiers = modifiers.Replace(
                        modifiers[i],
                        modifiers[i].WithoutLeadingTrivia());
                }

                if (i == (modifiers.Count - 1))
                {
                    modifiers = modifiers.Replace(
                        modifiers[i],
                        modifiers[i].WithTrailingTrivia(oldModifiers[i].TrailingTrivia));
                }
                else
                {
                    modifiers = modifiers.Replace(
                        modifiers[i],
                        modifiers[i].WithTrailingTrivia(SyntaxFactory.Space));
                }
            }

            return modifiers;
        }

        private static SyntaxNode GetNewNode(SyntaxNode declaration, SyntaxTokenList modifiers)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).WithModifiers(modifiers);
            }

            Debug.Assert(false, declaration.Kind().ToString());

            return null;
        }
    }
}