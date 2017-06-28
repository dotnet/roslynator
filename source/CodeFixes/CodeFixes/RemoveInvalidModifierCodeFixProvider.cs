// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Helpers;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveInvalidModifierCodeFixProvider))]
    [Shared]
    public class RemoveInvalidModifierCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.ModifierIsNotValidForThisItem,
                    CompilerDiagnosticIdentifiers.MoreThanOneProtectionModifier,
                    CompilerDiagnosticIdentifiers.AccessibilityModifiersMayNotBeUsedOnAccessorsInInterface,
                    CompilerDiagnosticIdentifiers.ModifiersCannotBePlacedOnEventAccessorDeclarations,
                    CompilerDiagnosticIdentifiers.AccessModifiersAreNotAllowedOnStaticConstructors,
                    CompilerDiagnosticIdentifiers.OnlyMethodsClassesStructsOrInterfacesMayBePartial,
                    CompilerDiagnosticIdentifiers.ClassCannotBeBothStaticAndSealed,
                    CompilerDiagnosticIdentifiers.FieldCanNotBeBothVolatileAndReadOnly);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxToken token = root.FindToken(context.Span.Start);

            if (token.IsKind(SyntaxKind.None))
                return;

            SyntaxNode node = token.Parent;

            if (!node.SupportsModifiers())
                node = node.FirstAncestor(f => f.SupportsModifiers());

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.ModifierIsNotValidForThisItem:
                        {
                            SyntaxTokenList modifiers = node.GetModifiers();

                            if (modifiers.Contains(token))
                            {
                                RemoveModifier(context, diagnostic, node, token);
                                break;
                            }

                            if (IsInterfaceMemberOrExplicitInterfaceImplementation(node))
                            {
                                List<int> indexes = null;

                                for (int i = 0; i < modifiers.Count ; i++)
                                {
                                    switch (modifiers[i].Kind())
                                    {
                                        case SyntaxKind.PublicKeyword:
                                        case SyntaxKind.ProtectedKeyword:
                                        case SyntaxKind.InternalKeyword:
                                        case SyntaxKind.PrivateKeyword:
                                        case SyntaxKind.StaticKeyword:
                                        case SyntaxKind.VirtualKeyword:
                                        case SyntaxKind.OverrideKeyword:
                                        case SyntaxKind.AbstractKeyword:
                                            {
                                                (indexes ?? (indexes = new List<int>())).Add(i);
                                                break;
                                            }
                                    }
                                }

                                if (indexes != null)
                                {
                                    if (indexes.Count == 1)
                                    {
                                        RemoveModifier(context, diagnostic, node, modifiers[indexes[0]]);
                                    }
                                    else
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Remove invalid modifiers",
                                            cancellationToken =>
                                            {
                                                SyntaxNode newNode = node;

                                                for (int i = indexes.Count - 1; i >= 0; i--)
                                                    newNode = RemoveModifierHelper.RemoveModifierAt(newNode, indexes[i]);

                                                return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                                            },
                                            GetEquivalenceKey(diagnostic));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }
                            else if (node.IsKind(SyntaxKind.IndexerDeclaration))
                            {
                                RemoveModifier(context, diagnostic, node, modifiers, SyntaxKind.StaticKeyword);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MoreThanOneProtectionModifier:
                        {
                            RemoveModifier(context, diagnostic, node, token);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.AccessibilityModifiersMayNotBeUsedOnAccessorsInInterface:
                    case CompilerDiagnosticIdentifiers.AccessModifiersAreNotAllowedOnStaticConstructors:
                        {
                            RemoveAccessibilityModifiers(context, diagnostic, node);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ModifiersCannotBePlacedOnEventAccessorDeclarations:
                        {
                            RemoveModifiers(context, diagnostic, node);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.OnlyMethodsClassesStructsOrInterfacesMayBePartial:
                        {
                            SyntaxTokenList modifiers = node.GetModifiers();

                            SyntaxToken partialModifier = modifiers[modifiers.IndexOf(SyntaxKind.PartialKeyword)];

                            RemoveModifier(context, diagnostic, node, partialModifier);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ClassCannotBeBothStaticAndSealed:
                        {
                            SyntaxTokenList modifiers = node.GetModifiers();

                            RemoveModifier(context, diagnostic, node, modifiers, SyntaxKind.StaticKeyword);
                            RemoveModifier(context, diagnostic, node, modifiers, SyntaxKind.SealedKeyword);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.FieldCanNotBeBothVolatileAndReadOnly:
                        {
                            if (!node.IsKind(SyntaxKind.FieldDeclaration))
                                break;

                            var fieldDeclaration = (FieldDeclarationSyntax)node;

                            SyntaxTokenList modifiers = fieldDeclaration.Modifiers;

                            RemoveModifier(context, diagnostic, fieldDeclaration, modifiers, SyntaxKind.VolatileKeyword);
                            RemoveModifier(context, diagnostic, fieldDeclaration, modifiers, SyntaxKind.ReadOnlyKeyword);
                            break;
                        }
                }
            }
        }

        private static SyntaxToken GetSingleModifierToRemoveOrDefault(SyntaxTokenList modifiers)
        {
            var modifierToRemove = default(SyntaxToken);

            foreach (SyntaxToken modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.AbstractKeyword:
                        {
                            if (modifierToRemove.IsKind(SyntaxKind.None))
                            {
                                modifierToRemove = modifier;
                            }
                            else
                            {
                                return default(SyntaxToken);
                            }

                            break;
                        }
                }
            }

            return modifierToRemove;
        }

        private void RemoveModifier(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node, SyntaxTokenList modifiers, SyntaxKind modifierKind)
        {
            int index = modifiers.IndexOf(modifierKind);

            if (index == -1)
                return;

            SyntaxToken modifier = modifiers[index];

            RemoveModifier(context, diagnostic, node, modifier, modifierKind.ToString());
        }

        private void RemoveModifier(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node, SyntaxToken token, string additionalKey = null)
        {
            CodeAction codeAction = CodeAction.Create(
                $"Remove '{token.ToString()}' modifier",
                cancellationToken =>
                {
                    SyntaxNode newNode = RemoveModifierHelper.RemoveModifier(node, token);

                    return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                },
                GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private void RemoveAccessibilityModifiers(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
        {
            SyntaxTokenList modifiers = node.GetModifiers();

            var accessModifier = default(SyntaxToken);

            foreach (SyntaxToken modifier in modifiers)
            {
                if (modifier.IsAccessModifier())
                {
                    if (accessModifier.IsAccessModifier())
                    {
                        accessModifier = default(SyntaxToken);
                        break;
                    }
                    else
                    {
                        accessModifier = modifier;
                    }
                }
            }

            if (accessModifier.IsAccessModifier())
            {
                RemoveModifier(context, diagnostic, node, accessModifier);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    "Remove accessibility modifiers",
                    cancellationToken =>
                    {
                        SyntaxNode newNode = node.WithModifiers(node.GetModifiers().RemoveAccessModifiers());

                        return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        private void RemoveModifiers(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
        {
            SyntaxTokenList modifiers = node.GetModifiers();

            if (modifiers.Count == 1)
            {
                RemoveModifier(context, diagnostic, node, modifiers[0]);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    "Remove modifiers",
                    cancellationToken =>
                    {
                        SyntaxNode newNode = RemoveModifiers(node);

                        return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        private static TNode RemoveModifiers<TNode>(TNode node) where TNode : SyntaxNode
        {
            SyntaxTokenList modifiers = node.GetModifiers();

            if (!modifiers.Any())
                return node;

            SyntaxToken firstModifier = modifiers.First();

            if (modifiers.Count == 1)
                return RemoveModifierHelper.RemoveModifier(node, firstModifier);

            SyntaxToken nextToken = modifiers.Last().GetNextToken();

            if (!nextToken.IsKind(SyntaxKind.None))
            {
                SyntaxTriviaList trivia = firstModifier.LeadingTrivia;

                trivia = trivia.AddRange(firstModifier.TrailingTrivia.EmptyIfWhitespace());

                for (int i = 1; i < modifiers.Count; i++)
                    trivia = trivia.AddRange(modifiers[i].GetLeadingAndTrailingTrivia().EmptyIfWhitespace());

                trivia = trivia.AddRange(nextToken.LeadingTrivia.EmptyIfWhitespace());

                node = node.ReplaceToken(nextToken, nextToken.WithLeadingTrivia(trivia));
            }
            else
            {
                SyntaxToken previousToken = firstModifier.GetPreviousToken();

                if (!previousToken.IsKind(SyntaxKind.None))
                {
                    SyntaxTriviaList trivia = firstModifier.GetLeadingAndTrailingTrivia();

                    for (int i = 1; i < modifiers.Count; i++)
                        trivia = trivia.AddRange(modifiers[i].GetLeadingAndTrailingTrivia().EmptyIfWhitespace());

                    node = node.ReplaceToken(nextToken, nextToken.AppendToTrailingTrivia(trivia));
                }
            }

            return (TNode)node.WithModifiers(default(SyntaxTokenList));
        }

        private static bool IsInterfaceMemberOrExplicitInterfaceImplementation(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        return node.IsParentKind(SyntaxKind.InterfaceDeclaration)
                            || ((MethodDeclarationSyntax)node).ExplicitInterfaceSpecifier != null;
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        return node.IsParentKind(SyntaxKind.InterfaceDeclaration)
                            || ((PropertyDeclarationSyntax)node).ExplicitInterfaceSpecifier != null;
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        return node.IsParentKind(SyntaxKind.InterfaceDeclaration)
                            || ((IndexerDeclarationSyntax)node).ExplicitInterfaceSpecifier != null;
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        return node.IsParentKind(SyntaxKind.InterfaceDeclaration);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        return ((EventDeclarationSyntax)node).ExplicitInterfaceSpecifier != null;
                    }
            }

            return false;
        }
    }
}
