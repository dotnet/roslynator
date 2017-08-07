// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Helpers.ModifierHelpers;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ModifiersCodeFixProvider))]
    [Shared]
    public class ModifiersCodeFixProvider : BaseCodeFixProvider
    {
        private static readonly Accessibility[] _publicOrInternal = new Accessibility[]
        {
            Accessibility.Public,
            Accessibility.Internal
        };

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
                    CompilerDiagnosticIdentifiers.StaticMemberCannotBeMarkedOverrideVirtualOrAbstract,
                    CompilerDiagnosticIdentifiers.AsyncModifierCanOnlyBeUsedInMethodsThatHaveBody,
                    CompilerDiagnosticIdentifiers.PartialMethodCannotHaveAccessModifiersOrVirtualAbstractOverrideNewSealedOrExternModifiers,
                    CompilerDiagnosticIdentifiers.ExtensionMethodMustBeStatic,
                    CompilerDiagnosticIdentifiers.NoDefiningDeclarationFoundForImplementingDeclarationOfPartialMethod,
                    CompilerDiagnosticIdentifiers.MethodHasParameterModifierThisWhichIsNotOnFirstParameter,
                    CompilerDiagnosticIdentifiers.CannotDeclareInstanceMembersInStaticClass,
                    CompilerDiagnosticIdentifiers.StaticClassesCannotHaveInstanceConstructors,
                    CompilerDiagnosticIdentifiers.ElementsDefinedInNamespaceCannotBeExplicitlyDeclaredAsPrivateProtectedOrProtectedInternal,
                    CompilerDiagnosticIdentifiers.NamespaceAlreadyContainsDefinition,
                    CompilerDiagnosticIdentifiers.TypeAlreadyContainsDefinition,
                    CompilerDiagnosticIdentifiers.NoSuitableMethodFoundToOverride,
                    CompilerDiagnosticIdentifiers.ExtensionMethodMustBeDefinedInNonGenericStaticClass,
                    CompilerDiagnosticIdentifiers.AsyncMethodsCannotHaveRefOrOutParameters);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeAccessibility)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddStaticModifier)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveThisModifier)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.MakeContainingClassNonStatic)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddPartialModifier)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveOutModifier)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveRefModifier))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
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
                                RemoveModifiers(context, diagnostic, node, modifiers, f =>
                                {
                                    switch (f.Kind())
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
                                                return true;
                                            }
                                    }

                                    return false;
                                });
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
                                RemoveModifier(context, diagnostic, node, SyntaxKind.PartialKeyword);

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
                    case CompilerDiagnosticIdentifiers.AsyncModifierCanOnlyBeUsedInMethodsThatHaveBody:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                RemoveModifier(context, diagnostic, node, SyntaxKind.AsyncKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.PartialMethodCannotHaveAccessModifiersOrVirtualAbstractOverrideNewSealedOrExternModifiers:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                break;

                            RemoveModifiers(context, diagnostic, node, f =>
                            {
                                switch (f.Kind())
                                {
                                    case SyntaxKind.PublicKeyword:
                                    case SyntaxKind.ProtectedKeyword:
                                    case SyntaxKind.InternalKeyword:
                                    case SyntaxKind.PrivateKeyword:
                                    case SyntaxKind.VirtualKeyword:
                                    case SyntaxKind.AbstractKeyword:
                                    case SyntaxKind.OverrideKeyword:
                                    case SyntaxKind.NewKeyword:
                                    case SyntaxKind.SealedKeyword:
                                    case SyntaxKind.ExternKeyword:
                                        {
                                            return true;
                                        }
                                }

                                return false;
                            });

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ExtensionMethodMustBeStatic:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddStaticModifier))
                                AddStaticModifier(context, diagnostic, node, CodeFixIdentifiers.AddStaticModifier);

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveThisModifier))
                                RemoveThisModifier(context, diagnostic, (MethodDeclarationSyntax)node, CodeFixIdentifiers.RemoveThisModifier);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ExtensionMethodMustBeDefinedInNonGenericStaticClass:
                        {
                            if (!node.IsKind(SyntaxKind.ClassDeclaration))
                                return;

                            var classDeclaration = (ClassDeclarationSyntax)node;

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddStaticModifier)
                                && !classDeclaration.IsStatic())
                            {
                                AddStaticModifier(context, diagnostic, node, CodeFixIdentifiers.AddStaticModifier);
                            }

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveThisModifier))
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Remove 'this' modifier from extension methods",
                                    cancellationToken =>
                                    {
                                        IEnumerable<ParameterSyntax> thisParameters = classDeclaration.Members
                                            .Where(f => f.IsKind(SyntaxKind.MethodDeclaration))
                                            .Cast<MethodDeclarationSyntax>()
                                            .Select(f => f.ParameterList?.Parameters.FirstOrDefault())
                                            .Where(f => f?.Modifiers.Contains(SyntaxKind.ThisKeyword) == true);

                                        return context.Document.ReplaceNodesAsync(
                                            thisParameters,
                                            (f, g) => f.RemoveModifier(f.Modifiers.Find(SyntaxKind.ThisKeyword)),
                                            cancellationToken);
                                    },
                                    GetEquivalenceKey(diagnostic, CodeFixIdentifiers.RemoveThisModifier));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NoDefiningDeclarationFoundForImplementingDeclarationOfPartialMethod:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                RemoveModifier(context, diagnostic, node, SyntaxKind.PartialKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MethodHasParameterModifierThisWhichIsNotOnFirstParameter:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveThisModifier))
                                RemoveModifier(context, diagnostic, token.Parent, token);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotDeclareInstanceMembersInStaticClass:
                    case CompilerDiagnosticIdentifiers.StaticClassesCannotHaveInstanceConstructors:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddStaticModifier))
                                AddStaticModifier(context, diagnostic, node, CodeFixIdentifiers.AddStaticModifier);

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.MakeContainingClassNonStatic))
                            {
                                var classDeclaration = (ClassDeclarationSyntax)node.Parent;

                                SyntaxToken staticModifier = classDeclaration.Modifiers.Find(SyntaxKind.StaticKeyword);

                                RemoveModifier(context, diagnostic, classDeclaration, staticModifier, CodeFixIdentifiers.MakeContainingClassNonStatic, "Make containing class non-static");
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ElementsDefinedInNamespaceCannotBeExplicitlyDeclaredAsPrivateProtectedOrProtectedInternal:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeAccessibility))
                                ChangeAccessibility(context, diagnostic, node, _publicOrInternal);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NamespaceAlreadyContainsDefinition:
                    case CompilerDiagnosticIdentifiers.TypeAlreadyContainsDefinition:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddPartialModifier))
                                break;

                            if (!node.IsKind(
                                SyntaxKind.ClassDeclaration,
                                SyntaxKind.StructDeclaration,
                                SyntaxKind.InterfaceDeclaration,
                                SyntaxKind.MethodDeclaration))
                            {
                                return;
                            }

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ISymbol symbol = semanticModel.GetDeclaredSymbol(node, context.CancellationToken);

                            ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

                            if (syntaxReferences.Length > 1)
                                AddPartialModifier(context, diagnostic, ImmutableArray.CreateRange(syntaxReferences, f => f.GetSyntax(context.CancellationToken)));

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NoSuitableMethodFoundToOverride:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                RemoveModifier(context, diagnostic, node, SyntaxKind.OverrideKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.AsyncMethodsCannotHaveRefOrOutParameters:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveRefModifier))
                                RemoveModifier(context, diagnostic, node, SyntaxKind.RefKeyword);

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveOutModifier))
                                RemoveModifier(context, diagnostic, node, SyntaxKind.OutKeyword);

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

        private void RemoveModifier(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node, SyntaxKind modifierKind)
        {
            RemoveModifier(context, diagnostic, node, node.GetModifiers(), modifierKind);
        }

        private void RemoveModifier(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node, SyntaxTokenList modifiers, SyntaxKind modifierKind)
        {
            int index = modifiers.IndexOf(modifierKind);

            if (index == -1)
                return;

            SyntaxToken modifier = modifiers[index];

            RemoveModifier(context, diagnostic, node, modifier, modifierKind.ToString());
        }

        private void RemoveModifier(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node, SyntaxToken token, string additionalKey = null, string message = null)
        {
            CodeAction codeAction = CodeAction.Create(
                message ?? $"Remove '{token.ToString()}' modifier",
                cancellationToken =>
                {
                    SyntaxNode newNode = node.RemoveModifier(token);

                    return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                },
                GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private void RemoveThisModifier(CodeFixContext context, Diagnostic diagnostic, MethodDeclarationSyntax methodDeclaration, string additionalKey = null)
        {
            ParameterSyntax parameter = methodDeclaration.ParameterList.Parameters.First();

            SyntaxTokenList modifiers = parameter.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.ThisKeyword);

            SyntaxToken modifier = modifiers[index];

            RemoveModifier(context, diagnostic, parameter, modifier, additionalKey);
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

        private void RemoveModifiers(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node, Func<SyntaxToken, bool> predicate)
        {
            RemoveModifiers(context, diagnostic, node, node.GetModifiers(), predicate);
        }

        private void RemoveModifiers(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node, SyntaxTokenList modifiers, Func<SyntaxToken, bool> predicate)
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

        private void AddStaticModifier(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node, string additionalKey = null)
        {
            if (node.IsKind(SyntaxKind.ConstructorDeclaration)
                && ((ConstructorDeclarationSyntax)node).ParameterList?.Parameters.Any() == true)
            {
                return;
            }

            CodeAction codeAction = CodeAction.Create(
                "Add 'static' modifier",
                cancellationToken =>
                {
                    SyntaxNode newNode = node;

                    if (node.IsKind(SyntaxKind.ConstructorDeclaration))
                        newNode = ModifierHelper.RemoveAccessModifiers(newNode);

                    newNode = newNode.InsertModifier(SyntaxKind.StaticKeyword, ModifierComparer.Instance);

                    return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                },
                GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private void AddPartialModifier(CodeFixContext context, Diagnostic diagnostic, ImmutableArray<SyntaxNode> nodes)
        {
            CodeAction codeAction = CodeAction.Create(
                "Add 'partial' modifier",
                cancellationToken =>
                {
                    return context.Solution().ReplaceNodesAsync(
                        nodes,
                        (f, g) => f.InsertModifier(SyntaxKind.PartialKeyword, ModifierComparer.Instance),
                        cancellationToken);
                },
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
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
