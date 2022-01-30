// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using Roslynator.CodeFixes;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ModifiersCodeFixProvider))]
    [Shared]
    public sealed class ModifiersCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        private static readonly Accessibility[] _publicOrInternal = new[]
        {
            Accessibility.Public,
            Accessibility.Internal
        };

        private static readonly Accessibility[] _publicOrInternalOrProtected = new[]
        {
            Accessibility.Public,
            Accessibility.Internal,
            Accessibility.Protected
        };

        private static readonly Accessibility[] _publicOrInternalOrPrivate = new[]
        {
            Accessibility.Public,
            Accessibility.Internal,
            Accessibility.Private
        };

        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0106_ModifierIsNotValidForThisItem,
                    CompilerDiagnosticIdentifiers.CS0107_MoreThanOneProtectionModifier,
                    CompilerDiagnosticIdentifiers.CS0275_AccessibilityModifiersMayNotBeUsedOnAccessorsInInterface,
                    CompilerDiagnosticIdentifiers.CS1609_ModifiersCannotBePlacedOnEventAccessorDeclarations,
                    CompilerDiagnosticIdentifiers.CS0515_AccessModifiersAreNotAllowedOnStaticConstructors,
                    CompilerDiagnosticIdentifiers.CS0753_OnlyMethodsClassesStructsOrInterfacesMayBePartial,
                    CompilerDiagnosticIdentifiers.CS0441_ClassCannotBeBothStaticAndSealed,
                    CompilerDiagnosticIdentifiers.CS0678_FieldCanNotBeBothVolatileAndReadOnly,
                    CompilerDiagnosticIdentifiers.CS0628_NewProtectedMemberDeclaredInSealedClass,
                    CompilerDiagnosticIdentifiers.CS1057_StaticClassesCannotContainProtectedMembers,
                    CompilerDiagnosticIdentifiers.CS0621_VirtualOrAbstractMembersCannotBePrivate,
                    CompilerDiagnosticIdentifiers.CS0442_AbstractPropertiesCannotHavePrivateAccessors,
                    CompilerDiagnosticIdentifiers.CS0112_StaticMemberCannotBeMarkedOverrideVirtualOrAbstract,
                    CompilerDiagnosticIdentifiers.CS1994_AsyncModifierCanOnlyBeUsedInMethodsThatHaveBody,
                    CompilerDiagnosticIdentifiers.CS0750_PartialMethodCannotHaveAccessModifiersOrVirtualAbstractOverrideNewSealedOrExternModifiers,
                    CompilerDiagnosticIdentifiers.CS1105_ExtensionMethodMustBeStatic,
                    CompilerDiagnosticIdentifiers.CS0759_NoDefiningDeclarationFoundForImplementingDeclarationOfPartialMethod,
                    CompilerDiagnosticIdentifiers.CS1100_MethodHasParameterModifierThisWhichIsNotOnFirstParameter,
                    CompilerDiagnosticIdentifiers.CS0708_CannotDeclareInstanceMembersInStaticClass,
                    CompilerDiagnosticIdentifiers.CS0710_StaticClassesCannotHaveInstanceConstructors,
                    CompilerDiagnosticIdentifiers.CS1527_ElementsDefinedInNamespaceCannotBeExplicitlyDeclaredAsPrivateProtectedOrProtectedInternal,
                    CompilerDiagnosticIdentifiers.CS0101_NamespaceAlreadyContainsDefinition,
                    CompilerDiagnosticIdentifiers.CS0102_TypeAlreadyContainsDefinition,
                    CompilerDiagnosticIdentifiers.CS0115_NoSuitableMethodFoundToOverride,
                    CompilerDiagnosticIdentifiers.CS1106_ExtensionMethodMustBeDefinedInNonGenericStaticClass,
                    CompilerDiagnosticIdentifiers.CS1988_AsyncMethodsCannotHaveRefOrOutParameters,
                    CompilerDiagnosticIdentifiers.CS1623_IteratorsCannotHaveRefOrOutParameters,
                    CompilerDiagnosticIdentifiers.CS0573_CannotHaveInstancePropertyOrFieldInitializersInStruct,
                    CompilerDiagnosticIdentifiers.CS0192_ReadOnlyFieldCannotBePassedAsRefOrOutValue,
                    CompilerDiagnosticIdentifiers.CS0501_MemberMustDeclareBodyBecauseItIsNotMarkedAbstractExternOrPartial,
                    CompilerDiagnosticIdentifiers.CS0549_NewVirtualMemberInSealedClass,
                    CompilerDiagnosticIdentifiers.CS8340_InstanceFieldsOfReadOnlyStructsMustBeReadOnly,
                    CompilerDiagnosticIdentifiers.CS0238_MemberCannotBeSealedBecauseItIsNotOverride);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
                return;

            SyntaxNode node = token.Parent;

            if (!CSharpFacts.CanHaveModifiers(node.Kind()))
                node = node.FirstAncestor(f => CSharpFacts.CanHaveModifiers(f.Kind()));

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS0106_ModifierIsNotValidForThisItem:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                break;

                            SyntaxTokenList modifiers = SyntaxInfo.ModifierListInfo(node).Modifiers;

                            if (modifiers.Contains(token))
                            {
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, token);
                                break;
                            }
                            else if (IsInterfaceMemberOrExplicitInterfaceImplementation(node))
                            {
                                ModifiersCodeFixRegistrator.RemoveModifiers(
                                    context,
                                    diagnostic,
                                    node,
                                    modifiers,
                                    f =>
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
                            else if (node.IsKind(SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.IndexerDeclaration, SyntaxKind.EventDeclaration, SyntaxKind.EventFieldDeclaration)
                                && node.IsParentKind(SyntaxKind.StructDeclaration, SyntaxKind.RecordStructDeclaration)
                                && modifiers.Contains(SyntaxKind.VirtualKeyword))
                            {
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.VirtualKeyword);
                            }
                            else if (node.IsKind(SyntaxKind.IndexerDeclaration)
                                && modifiers.Contains(SyntaxKind.StaticKeyword))
                            {
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.StaticKeyword);
                            }
                            else if (node.IsKind(SyntaxKind.PropertyDeclaration, SyntaxKind.IndexerDeclaration, SyntaxKind.EventDeclaration, SyntaxKind.EventFieldDeclaration)
                                && modifiers.Contains(SyntaxKind.AsyncKeyword))
                            {
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.AsyncKeyword);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0107_MoreThanOneProtectionModifier:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, token);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0275_AccessibilityModifiersMayNotBeUsedOnAccessorsInInterface:
                    case CompilerDiagnosticIdentifiers.CS0515_AccessModifiersAreNotAllowedOnStaticConstructors:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.RemoveAccessibility(context, diagnostic, node);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS1609_ModifiersCannotBePlacedOnEventAccessorDeclarations:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.RemoveModifiers(context, diagnostic, node);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0753_OnlyMethodsClassesStructsOrInterfacesMayBePartial:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.PartialKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0441_ClassCannotBeBothStaticAndSealed:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                break;

                            ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.StaticKeyword, additionalKey: nameof(SyntaxKind.StaticKeyword));
                            ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.SealedKeyword, additionalKey: nameof(SyntaxKind.SealedKeyword));
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0678_FieldCanNotBeBothVolatileAndReadOnly:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                break;

                            var fieldDeclaration = (FieldDeclarationSyntax)node;

                            ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, fieldDeclaration, SyntaxKind.VolatileKeyword, additionalKey: nameof(SyntaxKind.VolatileKeyword));
                            ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, fieldDeclaration, SyntaxKind.ReadOnlyKeyword, additionalKey: nameof(SyntaxKind.ReadOnlyKeyword));
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0628_NewProtectedMemberDeclaredInSealedClass:
                    case CompilerDiagnosticIdentifiers.CS1057_StaticClassesCannotContainProtectedMembers:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeAccessibility, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.ChangeAccessibility(context, diagnostic, node, _publicOrInternalOrPrivate);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0621_VirtualOrAbstractMembersCannotBePrivate:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveVirtualModifier, context.Document, root.SyntaxTree))
                            {
                                ModifierListInfo modifierInfo = SyntaxInfo.ModifierListInfo(node);

                                if (modifierInfo.IsVirtual)
                                    ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.VirtualKeyword, additionalKey: nameof(SyntaxKind.VirtualKeyword));
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeAccessibility, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.ChangeAccessibility(context, diagnostic, node, _publicOrInternalOrProtected);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0442_AbstractPropertiesCannotHavePrivateAccessors:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.RemoveAccessibility(context, diagnostic, node, additionalKey: CodeFixIdentifiers.RemoveInvalidModifier);

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeAccessibility, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.ChangeAccessibility(context, diagnostic, node, _publicOrInternalOrProtected);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0112_StaticMemberCannotBeMarkedOverrideVirtualOrAbstract:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                break;

                            if (!node.IsParentKind(SyntaxKind.ClassDeclaration)
                                || !((ClassDeclarationSyntax)node.Parent).Modifiers.Contains(SyntaxKind.StaticKeyword))
                            {
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.StaticKeyword, additionalKey: nameof(SyntaxKind.StaticKeyword));
                            }

                            ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.OverrideKeyword, additionalKey: nameof(SyntaxKind.OverrideKeyword));
                            ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.VirtualKeyword, additionalKey: nameof(SyntaxKind.VirtualKeyword));
                            ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.AbstractKeyword, additionalKey: nameof(SyntaxKind.AbstractKeyword));
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS1994_AsyncModifierCanOnlyBeUsedInMethodsThatHaveBody:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.AsyncKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0750_PartialMethodCannotHaveAccessModifiersOrVirtualAbstractOverrideNewSealedOrExternModifiers:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                break;

                            ModifiersCodeFixRegistrator.RemoveModifiers(
                                context,
                                diagnostic,
                                node,
                                f =>
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
                    case CompilerDiagnosticIdentifiers.CS1105_ExtensionMethodMustBeStatic:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddStaticModifier, context.Document, root.SyntaxTree))
                                AddStaticModifier(context, diagnostic, node, CodeFixIdentifiers.AddStaticModifier);

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveThisModifier, context.Document, root.SyntaxTree))
                            {
                                var methodDeclaration = (MethodDeclarationSyntax)node;

                                ParameterSyntax parameter = methodDeclaration.ParameterList.Parameters[0];

                                SyntaxToken modifier = parameter.Modifiers.Find(SyntaxKind.ThisKeyword);

                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, parameter, modifier, additionalKey: CodeFixIdentifiers.RemoveThisModifier);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS1106_ExtensionMethodMustBeDefinedInNonGenericStaticClass:
                        {
                            if (node is not ClassDeclarationSyntax classDeclaration)
                                return;

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddStaticModifier, context.Document, root.SyntaxTree)
                                && !classDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
                            {
                                AddStaticModifier(context, diagnostic, node, CodeFixIdentifiers.AddStaticModifier);
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveThisModifier, context.Document, root.SyntaxTree))
                            {
                                IEnumerable<ParameterSyntax> thisParameters = classDeclaration.Members
                                    .Where(f => f.IsKind(SyntaxKind.MethodDeclaration))
                                    .Cast<MethodDeclarationSyntax>()
                                    .Select(f => f.ParameterList?.Parameters.FirstOrDefault())
                                    .Where(f => f?.Modifiers.Contains(SyntaxKind.ThisKeyword) == true);

                                ModifiersCodeFixRegistrator.RemoveModifier(
                                    context,
                                    diagnostic,
                                    thisParameters,
                                    SyntaxKind.ThisKeyword,
                                    title: "Remove 'this' modifier from extension methods",
                                    additionalKey: CodeFixIdentifiers.RemoveThisModifier);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0759_NoDefiningDeclarationFoundForImplementingDeclarationOfPartialMethod:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.PartialKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS1100_MethodHasParameterModifierThisWhichIsNotOnFirstParameter:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveThisModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, token.Parent, token);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0708_CannotDeclareInstanceMembersInStaticClass:
                    case CompilerDiagnosticIdentifiers.CS0710_StaticClassesCannotHaveInstanceConstructors:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddStaticModifier, context.Document, root.SyntaxTree))
                                AddStaticModifier(context, diagnostic, node, CodeFixIdentifiers.AddStaticModifier);

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.MakeContainingClassNonStatic, context.Document, root.SyntaxTree))
                            {
                                var classDeclaration = (ClassDeclarationSyntax)node.Parent;

                                ModifiersCodeFixRegistrator.RemoveModifier(
                                    context,
                                    diagnostic,
                                    classDeclaration,
                                    classDeclaration.Modifiers.Find(SyntaxKind.StaticKeyword),
                                    title: "Make containing class non-static",
                                    additionalKey: CodeFixIdentifiers.MakeContainingClassNonStatic);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS1527_ElementsDefinedInNamespaceCannotBeExplicitlyDeclaredAsPrivateProtectedOrProtectedInternal:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeAccessibility, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.ChangeAccessibility(context, diagnostic, node, _publicOrInternal);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0101_NamespaceAlreadyContainsDefinition:
                    case CompilerDiagnosticIdentifiers.CS0102_TypeAlreadyContainsDefinition:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddPartialModifier, context.Document, root.SyntaxTree))
                                break;

                            if (!node.IsKind(
                                SyntaxKind.ClassDeclaration,
                                SyntaxKind.StructDeclaration,
                                SyntaxKind.RecordStructDeclaration,
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

                            ModifiersCodeFixRegistrator.AddModifier(
                                context,
                                diagnostic,
                                ImmutableArray.CreateRange(syntaxReferences, f => f.GetSyntax(context.CancellationToken)),
                                SyntaxKind.PartialKeyword,
                                title: $"Make {CSharpFacts.GetTitle(node)} partial");

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0115_NoSuitableMethodFoundToOverride:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveInvalidModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.OverrideKeyword);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS1988_AsyncMethodsCannotHaveRefOrOutParameters:
                    case CompilerDiagnosticIdentifiers.CS1623_IteratorsCannotHaveRefOrOutParameters:
                    case CompilerDiagnosticIdentifiers.CS0192_ReadOnlyFieldCannotBePassedAsRefOrOutValue:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveRefModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.RefKeyword, additionalKey: nameof(SyntaxKind.RefKeyword));

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveOutModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.OutKeyword, additionalKey: nameof(SyntaxKind.OutKeyword));

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0573_CannotHaveInstancePropertyOrFieldInitializersInStruct:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddStaticModifier, context.Document, root.SyntaxTree))
                                AddStaticModifier(context, diagnostic, node);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0501_MemberMustDeclareBodyBecauseItIsNotMarkedAbstractExternOrPartial:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddModifierAbstract, context.Document, root.SyntaxTree)
                                && node.IsKind(SyntaxKind.MethodDeclaration)
                                && (node.Parent as ClassDeclarationSyntax)?.Modifiers.Contains(SyntaxKind.AbstractKeyword) == true)
                            {
                                ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, node, SyntaxKind.AbstractKeyword);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0549_NewVirtualMemberInSealedClass:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveVirtualModifier, context.Document, root.SyntaxTree))
                            {
                                if (node is AccessorDeclarationSyntax
                                    && SyntaxInfo.ModifierListInfo(node.Parent.Parent).IsVirtual)
                                {
                                    node = node.Parent.Parent;
                                }

                                ModifiersCodeFixRegistrator.RemoveModifier(
                                    context,
                                    diagnostic,
                                    node,
                                    SyntaxKind.VirtualKeyword,
                                    additionalKey: CodeFixIdentifiers.RemoveVirtualModifier);
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.MakeContainingClassUnsealed, context.Document, root.SyntaxTree)
                                && node.Parent is ClassDeclarationSyntax classDeclaration)
                            {
                                ModifiersCodeFixRegistrator.RemoveModifier(
                                    context,
                                    diagnostic,
                                    classDeclaration,
                                    SyntaxKind.SealedKeyword,
                                    title: "Make containing class unsealed",
                                    additionalKey: CodeFixIdentifiers.MakeContainingClassUnsealed);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS8340_InstanceFieldsOfReadOnlyStructsMustBeReadOnly:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.MakeMemberReadOnly, context.Document, root.SyntaxTree))
                                break;

                            ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, node, SyntaxKind.ReadOnlyKeyword);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0238_MemberCannotBeSealedBecauseItIsNotOverride:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveSealedModifier, context.Document, root.SyntaxTree))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            if (semanticModel.GetDiagnostic(
                                CompilerDiagnosticIdentifiers.CS0114_MemberHidesInheritedMemberToMakeCurrentMethodOverrideThatImplementationAddOverrideKeyword,
                                CSharpUtility.GetIdentifier(node).Span,
                                context.CancellationToken) != null)
                            {
                                break;
                            }

                            ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, node, SyntaxKind.SealedKeyword);
                            break;
                        }
                }
            }
        }

        private static void AddStaticModifier(CodeFixContext context, Diagnostic diagnostic, SyntaxNode node, string additionalKey = null)
        {
            if (node.IsKind(SyntaxKind.ConstructorDeclaration)
                && ((ConstructorDeclarationSyntax)node).ParameterList?.Parameters.Any() == true)
            {
                return;
            }

            ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, node, SyntaxKind.StaticKeyword, additionalKey: additionalKey);
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
