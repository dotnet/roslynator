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
using Roslynator.CSharp.Helpers.ModifierHelpers;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ModifiersCodeFixProvider))]
    [Shared]
    public class ModifiersCodeFixProvider : BaseCodeFixProvider
    {
        private static readonly Accessibility[] _publicOrInternalOrProtected = new Accessibility[]
        {
            Accessibility.Public,
            Accessibility.Internal,
            Accessibility.Protected
        };

        private static readonly Accessibility[] _publicOrInternalOrPrivate = new Accessibility[]
        {
            Accessibility.Public,
            Accessibility.Internal,
            Accessibility.Private
        };

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
                    CompilerDiagnosticIdentifiers.FieldCanNotBeBothVolatileAndReadOnly,
                    CompilerDiagnosticIdentifiers.NewProtectedMemberDeclaredInSealedClass,
                    CompilerDiagnosticIdentifiers.StaticClassesCannotContainProtectedMembers,
                    CompilerDiagnosticIdentifiers.VirtualOrAbstractmembersCannotBePrivate,
                    CompilerDiagnosticIdentifiers.AbstractPropertiesCannotHavePrivateAccessors,
                    CompilerDiagnosticIdentifiers.StaticMemberCannotBeMarkedOverrideVirtualOrAbstract);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.RemoveInvalidModifier,
                CodeFixIdentifiers.ChangeAccessibility))
            {
                return;
            }

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
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                break;

                            SyntaxTokenList modifiers = node.GetModifiers();

                            if (modifiers.Contains(token))
                            {
                                RemoveModifier(context, diagnostic, node, token);
                                break;
                            }

                            if (IsInterfaceMemberOrExplicitInterfaceImplementation(node))
                            {
                                List<int> indexes = null;

                                for (int i = 0; i < modifiers.Count; i++)
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
                                                    newNode = ModifierHelper.RemoveModifierAt(newNode, indexes[i]);

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
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                RemoveModifier(context, diagnostic, node, token);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.AccessibilityModifiersMayNotBeUsedOnAccessorsInInterface:
                    case CompilerDiagnosticIdentifiers.AccessModifiersAreNotAllowedOnStaticConstructors:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                RemoveAccessModifiers(context, diagnostic, node);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ModifiersCannotBePlacedOnEventAccessorDeclarations:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                RemoveModifiers(context, diagnostic, node);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.OnlyMethodsClassesStructsOrInterfacesMayBePartial:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                RemoveModifier(context, diagnostic, node, node.GetModifiers(), SyntaxKind.PartialKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ClassCannotBeBothStaticAndSealed:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                break;

                            SyntaxTokenList modifiers = node.GetModifiers();

                            RemoveModifier(context, diagnostic, node, modifiers, SyntaxKind.StaticKeyword);
                            RemoveModifier(context, diagnostic, node, modifiers, SyntaxKind.SealedKeyword);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.FieldCanNotBeBothVolatileAndReadOnly:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                break;

                            var fieldDeclaration = (FieldDeclarationSyntax)node;

                            SyntaxTokenList modifiers = fieldDeclaration.Modifiers;

                            RemoveModifier(context, diagnostic, fieldDeclaration, modifiers, SyntaxKind.VolatileKeyword);
                            RemoveModifier(context, diagnostic, fieldDeclaration, modifiers, SyntaxKind.ReadOnlyKeyword);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NewProtectedMemberDeclaredInSealedClass:
                    case CompilerDiagnosticIdentifiers.StaticClassesCannotContainProtectedMembers:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeAccessibility))
                                ChangeAccessibility(context, diagnostic, node, _publicOrInternalOrPrivate);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.VirtualOrAbstractmembersCannotBePrivate:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeAccessibility))
                                ChangeAccessibility(context, diagnostic, node, _publicOrInternalOrProtected);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.AbstractPropertiesCannotHavePrivateAccessors:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                RemoveAccessModifiers(context, diagnostic, node);

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeAccessibility))
                                ChangeAccessibility(context, diagnostic, node, _publicOrInternalOrProtected);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.StaticMemberCannotBeMarkedOverrideVirtualOrAbstract:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                break;

                            SyntaxTokenList modifiers = node.GetModifiers();

                            if (!node.IsParentKind(SyntaxKind.ClassDeclaration)
                                || !((ClassDeclarationSyntax)node.Parent).Modifiers.Contains(SyntaxKind.StaticKeyword))
                            {
                                RemoveModifier(context, diagnostic, node, modifiers, SyntaxKind.StaticKeyword);
                            }

                            RemoveModifier(context, diagnostic, node, modifiers, SyntaxKind.OverrideKeyword);
                            RemoveModifier(context, diagnostic, node, modifiers, SyntaxKind.VirtualKeyword);
                            RemoveModifier(context, diagnostic, node, modifiers, SyntaxKind.AbstractKeyword);
                            break;
                        }
                }
            }
        }

        private void ChangeAccessibility(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node, Accessibility[] accessibilities)
        {
            foreach (Accessibility accessibility in accessibilities)
            {
                if (AccessibilityHelper.IsAllowedAccessibility(node, accessibility))
                {
                    CodeAction codeAction = CodeAction.Create(
                        $"Change accessibility to '{AccessibilityHelper.GetAccessibilityName(accessibility)}'",
                        cancellationToken => ChangeAccessibilityRefactoring.RefactorAsync(context.Document, node, accessibility, cancellationToken),
                        GetEquivalenceKey(diagnostic.Id, accessibility.ToString()));

                    context.RegisterCodeFix(codeAction, diagnostic);
                }
            }
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
                    SyntaxNode newNode = node.RemoveModifier(token);

                    return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                },
                GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private void RemoveAccessModifiers(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node)
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
                        SyntaxNode newNode = ModifierHelper.RemoveAccessModifiers(node);

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
                        SyntaxNode newNode = ModifierHelper.RemoveModifiers(node);

                        return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
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
