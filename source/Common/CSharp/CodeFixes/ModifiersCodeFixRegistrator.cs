// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    internal static class ModifiersCodeFixRegistrator
    {
        public static void AddModifier(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            SyntaxKind modifierKind,
            string title = null,
            string additionalKey = null,
            IModifierComparer comparer = null)
        {
            Document document = context.Document;

            AddModifier(context, document, diagnostic, node, modifierKind, title, additionalKey, comparer);
        }

        public static void AddModifier(
            CodeFixContext context,
            Document document,
            Diagnostic diagnostic,
            SyntaxNode node,
            SyntaxKind modifierKind,
            string title = null,
            string additionalKey = null,
            IModifierComparer comparer = null)
        {
            CodeAction codeAction = CodeAction.Create(
                title ?? GetAddModifierTitle(modifierKind, node),
                cancellationToken => AddModifierAsync(document, node, modifierKind, comparer, cancellationToken),
                GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Task<Document> AddModifierAsync<TNode>(
            Document document,
            TNode node,
            SyntaxKind modifierKind,
            IModifierComparer comparer = null,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            TNode newNode = AddModifier(node, modifierKind, comparer);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        private static TNode AddModifier<TNode>(
            TNode node,
            SyntaxKind modifierKind,
            IModifierComparer comparer = null) where TNode : SyntaxNode
        {
            switch (modifierKind)
            {
                case SyntaxKind.AbstractKeyword:
                    {
                        node = node
                            .RemoveModifier(SyntaxKind.VirtualKeyword)
                            .RemoveModifier(SyntaxKind.OverrideKeyword);

                        break;
                    }
                case SyntaxKind.VirtualKeyword:
                    {
                        node = node
                            .RemoveModifier(SyntaxKind.AbstractKeyword)
                            .RemoveModifier(SyntaxKind.OverrideKeyword);

                        break;
                    }
                case SyntaxKind.OverrideKeyword:
                    {
                        node = node
                            .RemoveModifier(SyntaxKind.AbstractKeyword)
                            .RemoveModifier(SyntaxKind.VirtualKeyword);

                        break;
                    }
                case SyntaxKind.StaticKeyword:
                    {
                        if (node.Kind() == SyntaxKind.ConstructorDeclaration)
                            node = Modifier.RemoveAccess(node);

                        node = node.RemoveModifier(SyntaxKind.SealedKeyword);

                        break;
                    }
            }

            return node.InsertModifier(modifierKind, comparer);
        }

        public static void AddModifier<TNode>(
            CodeFixContext context,
            Diagnostic diagnostic,
            IEnumerable<TNode> nodes,
            SyntaxKind modifierKind,
            string title = null,
            string additionalKey = null,
            IModifierComparer comparer = null) where TNode : SyntaxNode
        {
            if (nodes is IList<TNode> list)
            {
                if (list.Count == 0)
                    return;

                if (list.Count == 1)
                {
                    AddModifier(context, diagnostic, list[0], modifierKind, title, additionalKey, comparer);
                    return;
                }
            }

            CodeAction codeAction = CodeAction.Create(
                title ?? GetAddModifierTitle(modifierKind),
                cancellationToken =>
                {
                    return context.Solution().ReplaceNodesAsync(
                        nodes,
                        (f, _) => AddModifier(f, modifierKind, comparer),
                        cancellationToken);
                },
                GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void RemoveModifier(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            SyntaxKind modifierKind,
            string title = null,
            string additionalKey = null)
        {
            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                title ?? GetRemoveModifierTitle(modifierKind),
                cancellationToken => RemoveModifierAsync(document, node, modifierKind, cancellationToken),
                GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void RemoveModifier(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            SyntaxToken modifier,
            string title = null,
            string additionalKey = null)
        {
            SyntaxKind kind = modifier.Kind();

            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                title ?? GetRemoveModifierTitle(kind),
                cancellationToken => RemoveModifierAsync(document, node, modifier, cancellationToken),
                GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Task<Document> RemoveModifierAsync<TNode>(
            Document document,
            TNode node,
            SyntaxKind modifierKind,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            SyntaxNode newNode = Modifier.Remove(node, modifierKind);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        private static Task<Document> RemoveModifierAsync<TNode>(
            Document document,
            TNode node,
            SyntaxToken modifier,
            CancellationToken cancellationToken = default(CancellationToken)) where TNode : SyntaxNode
        {
            SyntaxNode newNode = Modifier.Remove(node, modifier);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        public static void RemoveModifier<TNode>(
            CodeFixContext context,
            Diagnostic diagnostic,
            IEnumerable<TNode> nodes,
            SyntaxKind modifierKind,
            string title = null,
            string additionalKey = null) where TNode : SyntaxNode
        {
            if (nodes is IList<TNode> list)
            {
                if (list.Count == 0)
                    return;

                if (list.Count == 1)
                {
                    RemoveModifier(context, diagnostic, list[0], modifierKind, title, additionalKey);
                    return;
                }
            }

            CodeAction codeAction = CodeAction.Create(
                title ?? GetRemoveModifierTitle(modifierKind),
                cancellationToken =>
                {
                    return context.Solution().ReplaceNodesAsync(
                        nodes,
                        (f, _) => Modifier.Remove(f, modifierKind),
                        cancellationToken);
                },
                GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void RemoveModifiers(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            Func<SyntaxToken, bool> predicate,
            string additionalKey = null)
        {
            RemoveModifiers(context, diagnostic, node, node.GetModifiers(), predicate, additionalKey);
        }

        public static void RemoveModifiers(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            SyntaxTokenList modifiers,
            Func<SyntaxToken, bool> predicate,
            string additionalKey = null)
        {
            List<int> indexes = null;

            for (int i = 0; i < modifiers.Count; i++)
            {
                if (predicate(modifiers[i]))
                    (indexes ?? (indexes = new List<int>())).Add(i);
            }

            if (indexes != null)
            {
                if (indexes.Count == 1)
                {
                    RemoveModifier(context, diagnostic, node, modifiers[indexes[0]], additionalKey: additionalKey);
                }
                else
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Remove modifiers",
                        cancellationToken =>
                        {
                            SyntaxNode newNode = node;

                            for (int i = indexes.Count - 1; i >= 0; i--)
                                newNode = Modifier.RemoveAt(newNode, indexes[i]);

                            return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                        },
                        GetEquivalenceKey(diagnostic, additionalKey));

                    context.RegisterCodeFix(codeAction, diagnostic);
                }
            }
        }

        public static void RemoveModifiers(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            string additionalKey = null)
        {
            SyntaxTokenList modifiers = node.GetModifiers();

            if (modifiers.Count == 1)
            {
                RemoveModifier(context, diagnostic, node, modifiers[0], additionalKey);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    "Remove modifiers",
                    cancellationToken =>
                    {
                        SyntaxNode newNode = Modifier.RemoveAll(node);

                        return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                    },
                    GetEquivalenceKey(diagnostic, additionalKey));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        public static void RemoveAccessModifiers(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            string additionalKey = null)
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
                RemoveModifier(context, diagnostic, node, accessModifier, additionalKey: additionalKey);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    "Remove access modifiers",
                    cancellationToken =>
                    {
                        SyntaxNode newNode = Modifier.RemoveAccess(node);

                        return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                    },
                    GetEquivalenceKey(diagnostic, additionalKey));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        public static void MoveModifier(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            SyntaxToken modifier,
            string title = null,
            string additionalKey = null,
            IModifierComparer comparer = null)
        {
            Document document = context.Document;

            SyntaxKind kind = modifier.Kind();

            CodeAction codeAction = CodeAction.Create(
                title ?? GetRemoveModifierTitle(kind),
                cancellationToken =>
                {
                    SyntaxNode newNode = node
                        .RemoveModifier(modifier)
                        .InsertModifier(kind, comparer);

                    return document.ReplaceNodeAsync(node, newNode, cancellationToken);
                },
                GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void ChangeAccessibility(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            IEnumerable<Accessibility> accessibilities)
        {
            foreach (Accessibility accessibility in accessibilities)
                ChangeAccessibility(context, diagnostic, node, accessibility);
        }

        public static void ChangeAccessibility(
            CodeFixContext context,
            Diagnostic diagnostic,
            SyntaxNode node,
            Accessibility accessibility)
        {
            if (!CSharpUtility.IsAllowedAccessibility(node, accessibility))
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Change accessibility to '{accessibility.GetName()}'",
                cancellationToken => ChangeAccessibilityRefactoring.RefactorAsync(context.Document, node, accessibility, cancellationToken),
                GetEquivalenceKey(diagnostic, accessibility.ToString()));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static string GetEquivalenceKey(Diagnostic diagnostic, string additionalKey)
        {
            return EquivalenceKeyProvider.GetEquivalenceKey(diagnostic, additionalKey);
        }

        private static string GetAddModifierTitle(SyntaxKind modifierKind)
        {
            return $"Add modifier '{Modifier.GetName(modifierKind)}'";
        }

        private static string GetAddModifierTitle(SyntaxKind modifierKind, SyntaxNode node)
        {
            switch (modifierKind)
            {
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.VirtualKeyword:
                case SyntaxKind.AbstractKeyword:
                case SyntaxKind.ReadOnlyKeyword:
                case SyntaxKind.AsyncKeyword:
                    return $"Make {node.GetTitle()} {Modifier.GetTitle(modifierKind)}";
            }

            return GetAddModifierTitle(modifierKind);
        }

        private static string GetRemoveModifierTitle(SyntaxKind modifierKind)
        {
            return $"Remove modifier '{Modifier.GetName(modifierKind)}'";
        }
    }
}