// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                    CompilerDiagnosticIdentifiers.AsyncMethodsCannotHaveRefOrOutParameters,
                    CompilerDiagnosticIdentifiers.IteratorsCannotHaveRefOrOutParameters,
                    CompilerDiagnosticIdentifiers.CannotHaveInstancePropertyOrFieldInitializersInStruct,
                    CompilerDiagnosticIdentifiers.ReadOnlyFieldCannotBeUsedAsRefOrOutValue);
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

            if (!node.Kind().SupportsModifiers())
                node = node.FirstAncestor(f => f.Kind().SupportsModifiers());

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
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, token);
                                break;
                            }
                            else if (IsInterfaceMemberOrExplicitInterfaceImplementation(node))
                            {
                                ModifiersCodeFixes.RemoveModifiers(context, diagnostic, node, modifiers, f =>
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
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.StaticKeyword);
                            }
                            else if (node.IsKind(SyntaxKind.PropertyDeclaration))
                            {
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.AsyncKeyword);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MoreThanOneProtectionModifier:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, token);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.AccessibilityModifiersMayNotBeUsedOnAccessorsInInterface:
                    case CompilerDiagnosticIdentifiers.AccessModifiersAreNotAllowedOnStaticConstructors:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                ModifiersCodeFixes.RemoveAccessModifiers(context, diagnostic, node);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ModifiersCannotBePlacedOnEventAccessorDeclarations:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                ModifiersCodeFixes.RemoveModifiers(context, diagnostic, node);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.OnlyMethodsClassesStructsOrInterfacesMayBePartial:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.PartialKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ClassCannotBeBothStaticAndSealed:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                break;

                            SyntaxTokenList modifiers = node.GetModifiers();

                            ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.StaticKeyword, additionalKey: nameof(SyntaxKind.StaticKeyword));
                            ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.SealedKeyword, additionalKey: nameof(SyntaxKind.SealedKeyword));
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.FieldCanNotBeBothVolatileAndReadOnly:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                break;

                            var fieldDeclaration = (FieldDeclarationSyntax)node;

                            SyntaxTokenList modifiers = fieldDeclaration.Modifiers;

                            ModifiersCodeFixes.RemoveModifier(context, diagnostic, fieldDeclaration, SyntaxKind.VolatileKeyword, additionalKey: nameof(SyntaxKind.VolatileKeyword));
                            ModifiersCodeFixes.RemoveModifier(context, diagnostic, fieldDeclaration, SyntaxKind.ReadOnlyKeyword, additionalKey: nameof(SyntaxKind.ReadOnlyKeyword));
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NewProtectedMemberDeclaredInSealedClass:
                    case CompilerDiagnosticIdentifiers.StaticClassesCannotContainProtectedMembers:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeAccessibility))
                                ModifiersCodeFixes.ChangeAccessibility(context, diagnostic, node, _publicOrInternalOrPrivate);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.VirtualOrAbstractmembersCannotBePrivate:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeAccessibility))
                                ModifiersCodeFixes.ChangeAccessibility(context, diagnostic, node, _publicOrInternalOrProtected);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.AbstractPropertiesCannotHavePrivateAccessors:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                ModifiersCodeFixes.RemoveAccessModifiers(context, diagnostic, node, additionalKey: CodeFixIdentifiers.RemoveInvalidModifier);

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeAccessibility))
                                ModifiersCodeFixes.ChangeAccessibility(context, diagnostic, node, _publicOrInternalOrProtected);

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
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.StaticKeyword, additionalKey: nameof(SyntaxKind.StaticKeyword));
                            }

                            ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.OverrideKeyword, additionalKey: nameof(SyntaxKind.OverrideKeyword));
                            ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.VirtualKeyword, additionalKey: nameof(SyntaxKind.VirtualKeyword));
                            ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.AbstractKeyword, additionalKey: nameof(SyntaxKind.AbstractKeyword));
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.AsyncModifierCanOnlyBeUsedInMethodsThatHaveBody:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.AsyncKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.PartialMethodCannotHaveAccessModifiersOrVirtualAbstractOverrideNewSealedOrExternModifiers:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                break;

                            ModifiersCodeFixes.RemoveModifiers(context, diagnostic, node, f =>
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
                            {
                                var methodDeclaration = (MethodDeclarationSyntax)node;

                                ParameterSyntax parameter = methodDeclaration.ParameterList.Parameters.First();

                                SyntaxToken modifier = parameter.Modifiers.Find(SyntaxKind.ThisKeyword);

                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, parameter, modifier, additionalKey: CodeFixIdentifiers.RemoveThisModifier);
                            }

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
                                IEnumerable<ParameterSyntax> thisParameters = classDeclaration.Members
                                    .Where(f => f.IsKind(SyntaxKind.MethodDeclaration))
                                    .Cast<MethodDeclarationSyntax>()
                                    .Select(f => f.ParameterList?.Parameters.FirstOrDefault())
                                    .Where(f => f?.Modifiers.Contains(SyntaxKind.ThisKeyword) == true);

                                ModifiersCodeFixes.RemoveModifier(
                                    context,
                                    diagnostic,
                                    thisParameters,
                                    SyntaxKind.ThisKeyword,
                                    title: "Remove 'this' modifier from extension methods",
                                    additionalKey: CodeFixIdentifiers.RemoveThisModifier);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NoDefiningDeclarationFoundForImplementingDeclarationOfPartialMethod:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.PartialKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MethodHasParameterModifierThisWhichIsNotOnFirstParameter:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveThisModifier))
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, token.Parent, token);

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

                                ModifiersCodeFixes.RemoveModifier(
                                    context,
                                    diagnostic,
                                    classDeclaration,
                                    classDeclaration.Modifiers.Find(SyntaxKind.StaticKeyword),
                                    title: "Make containing class non-static",
                                    additionalKey: CodeFixIdentifiers.MakeContainingClassNonStatic);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ElementsDefinedInNamespaceCannotBeExplicitlyDeclaredAsPrivateProtectedOrProtectedInternal:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeAccessibility))
                                ModifiersCodeFixes.ChangeAccessibility(context, diagnostic, node, _publicOrInternal);

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

                            if (syntaxReferences.Length <= 1)
                                break;

                            ModifiersCodeFixes.AddModifier(
                                context,
                                diagnostic,
                                ImmutableArray.CreateRange(syntaxReferences, f => f.GetSyntax(context.CancellationToken)),
                                SyntaxKind.PartialKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NoSuitableMethodFoundToOverride:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveInvalidModifier))
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.OverrideKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.AsyncMethodsCannotHaveRefOrOutParameters:
                    case CompilerDiagnosticIdentifiers.IteratorsCannotHaveRefOrOutParameters:
                    case CompilerDiagnosticIdentifiers.ReadOnlyFieldCannotBeUsedAsRefOrOutValue:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveRefModifier))
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.RefKeyword, additionalKey: nameof(SyntaxKind.RefKeyword));

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveOutModifier))
                                ModifiersCodeFixes.RemoveModifier(context, diagnostic, node, SyntaxKind.OutKeyword, additionalKey: nameof(SyntaxKind.OutKeyword));

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotHaveInstancePropertyOrFieldInitializersInStruct:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddStaticModifier))
                                AddStaticModifier(context, diagnostic, node);

                            break;
                        }
                }
            }
        }

        private void AddStaticModifier(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node, string additionalKey = null)
        {
            if (node.IsKind(SyntaxKind.ConstructorDeclaration)
                && ((ConstructorDeclarationSyntax)node).ParameterList?.Parameters.Any() == true)
            {
                return;
            }

            ModifiersCodeFixes.AddModifier(context, diagnostic, node, SyntaxKind.StaticKeyword, additionalKey: additionalKey);
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
