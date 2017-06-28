// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Helpers.ModifierHelpers;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    internal static class AccessibilityHelper
    {
        public static bool IsAllowedAccessibility(SyntaxNode node, Accessibility accessibility)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.EnumDeclaration:
                    {
                        if (node.IsParentKind(SyntaxKind.NamespaceDeclaration, SyntaxKind.CompilationUnit))
                        {
                            return accessibility == Accessibility.Public
                                || accessibility == Accessibility.Internal;
                        }

                        return true;
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        return CheckProtectedOrProtectedInternalInStaticOrSealedClass(node, accessibility)
                            && CheckAccessorAccessibility(((EventDeclarationSyntax)node).AccessorList, accessibility);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        return CheckProtectedOrProtectedInternalInStaticOrSealedClass(node, accessibility)
                            && CheckAccessorAccessibility(((IndexerDeclarationSyntax)node).AccessorList, accessibility);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        return CheckProtectedOrProtectedInternalInStaticOrSealedClass(node, accessibility)
                            && CheckAccessorAccessibility(((PropertyDeclarationSyntax)node).AccessorList, accessibility);
                    }
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.IncompleteMember:
                    {
                        return CheckProtectedOrProtectedInternalInStaticOrSealedClass(node, accessibility);
                    }
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return accessibility == Accessibility.Public;
                    }
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    {
                        var memberDeclaration = node.Parent.Parent as MemberDeclarationSyntax;

                        Debug.Assert(memberDeclaration != null, node.ToString());

                        if (memberDeclaration != null)
                        {
                            if (accessibility == Accessibility.Protected
                                || accessibility == Accessibility.ProtectedOrInternal)
                            {
                                if (IsContainedInStaticOrSealedClass(memberDeclaration))
                                    return false;
                            }

                            Accessibility declarationAccessibility = memberDeclaration.GetModifiers().GetAccessibility();

                            if (declarationAccessibility == Accessibility.NotApplicable)
                                declarationAccessibility = memberDeclaration.GetDefaultExplicitAccessibility();

                            return accessibility.IsMoreRestrictiveThan(declarationAccessibility);
                        }

                        return false;
                    }
                default:
                    {
                        Debug.Fail(node.Kind().ToString());
                        return false;
                    }
            }
        }

        private static bool CheckProtectedOrProtectedInternalInStaticOrSealedClass(SyntaxNode node, Accessibility accessibility)
        {
            if (accessibility == Accessibility.Protected
                || accessibility == Accessibility.ProtectedOrInternal)
            {
                if (IsContainedInStaticOrSealedClass(node))
                    return false;
            }

            return true;
        }

        private static bool CheckAccessorAccessibility(AccessorListSyntax accessorList, Accessibility accessibility)
        {
            if (accessorList != null)
            {
                foreach (AccessorDeclarationSyntax accessor in accessorList.Accessors)
                {
                    Accessibility accessorAccessibility = accessor.Modifiers.GetAccessibility();

                    if (accessorAccessibility != Accessibility.NotApplicable)
                        return accessorAccessibility.IsMoreRestrictiveThan(accessibility);
                }
            }

            return true;
        }

        private static bool IsContainedInStaticOrSealedClass(SyntaxNode node)
        {
            return node.IsParentKind(SyntaxKind.ClassDeclaration)
                && ((ClassDeclarationSyntax)node.Parent).Modifiers.ContainsAny(SyntaxKind.StaticKeyword, SyntaxKind.SealedKeyword);
        }

        public static TNode ChangeAccessibility<TNode>(
            TNode node,
            Accessibility newAccessibility,
            IModifierComparer comparer = null) where TNode : SyntaxNode
        {
            return ChangeAccessibility(node, AccessibilityInfo.Create(node.GetModifiers()), newAccessibility, comparer);
        }

        internal static TNode ChangeAccessibility<TNode>(
            TNode node,
            AccessibilityInfo info,
            Accessibility newAccessibility,
            IModifierComparer comparer = null) where TNode : SyntaxNode
        {
            Accessibility accessibility = info.Accessibility;

            if (accessibility == newAccessibility)
                return node;

            comparer = comparer ?? ModifierComparer.Instance;

            if (IsSingleTokenAccessibility(accessibility)
                && IsSingleTokenAccessibility(newAccessibility))
            {
                int insertIndex = comparer.GetInsertIndex(info.Modifiers, GetKind(newAccessibility));

                if (info.Index == insertIndex
                    || info.Index == insertIndex - 1)
                {
                    SyntaxToken newToken = CreateToken(newAccessibility).WithTriviaFrom(info.Token);

                    SyntaxTokenList newModifiers = info.Modifiers.Replace(info.Token, newToken);

                    return node.WithModifiers(newModifiers);
                }
            }

            if (accessibility != Accessibility.NotApplicable)
            {
                node = ModifierHelper.RemoveModifierAt(node, Math.Max(info.Index, info.AdditionalIndex));

                if (accessibility == Accessibility.ProtectedOrInternal)
                    node = ModifierHelper.RemoveModifierAt(node, Math.Min(info.Index, info.AdditionalIndex));
            }

            if (newAccessibility != Accessibility.NotApplicable)
            {
                node = (TNode)InsertModifier(node, newAccessibility, comparer);
            }

            return node;
        }

        private static SyntaxNode InsertModifier(SyntaxNode node, Accessibility accessibility, IModifierComparer comparer)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    {
                        return node.InsertModifier(SyntaxKind.PrivateKeyword, comparer);
                    }
                case Accessibility.Protected:
                    {
                        return node.InsertModifier(SyntaxKind.ProtectedKeyword, comparer);
                    }
                case Accessibility.Internal:
                    {
                        return node.InsertModifier(SyntaxKind.InternalKeyword, comparer);
                    }
                case Accessibility.Public:
                    {
                        return node.InsertModifier(SyntaxKind.PublicKeyword, comparer);
                    }
                case Accessibility.ProtectedOrInternal:
                    {
                        node = node.InsertModifier(SyntaxKind.ProtectedKeyword, comparer);
                        node = node.InsertModifier(SyntaxKind.InternalKeyword, comparer);

                        return node;
                    }
            }

            return node;
        }

        private static bool IsSingleTokenAccessibility(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                case Accessibility.Protected:
                case Accessibility.Internal:
                case Accessibility.Public:
                    return true;
                default:
                    return false;
            }
        }

        private static SyntaxToken CreateToken(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return PrivateKeyword();
                case Accessibility.Protected:
                    return ProtectedKeyword();
                case Accessibility.Internal:
                    return InternalKeyword();
                case Accessibility.Public:
                    return PublicKeyword();
                case Accessibility.NotApplicable:
                    return default(SyntaxToken);
                default:
                    throw new ArgumentException("", nameof(accessibility));
            }
        }

        private static SyntaxKind GetKind(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return SyntaxKind.PrivateKeyword;
                case Accessibility.Protected:
                    return SyntaxKind.ProtectedKeyword;
                case Accessibility.Internal:
                    return SyntaxKind.InternalKeyword;
                case Accessibility.Public:
                    return SyntaxKind.PublicKeyword;
                case Accessibility.NotApplicable:
                    return SyntaxKind.None;
                default:
                    throw new ArgumentException("", nameof(accessibility));
            }
        }

        public static string GetAccessibilityName(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return "private";
                case Accessibility.Protected:
                    return "protected";
                case Accessibility.Internal:
                    return "internal";
                case Accessibility.ProtectedOrInternal:
                    return "protected internal";
                case Accessibility.Public:
                    return "public";
            }

            Debug.Fail(accessibility.ToString());

            return "";
        }
    }
}
