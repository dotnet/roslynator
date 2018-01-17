// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers
{
    internal static class AllowedAccessibilityHelper
    {
        public static bool IsAllowedAccessibility(SyntaxNode node, Accessibility accessibility, bool allowOverride = false)
        {
            switch (node.Parent?.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                case SyntaxKind.CompilationUnit:
                    {
                        return accessibility.Is(Accessibility.Public, Accessibility.Internal);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        if (accessibility.ContainsProtected())
                            return false;

                        break;
                    }
            }

            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.EnumDeclaration:
                    {
                        return true;
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)node;

                        ModifiersInfo info = SyntaxInfo.ModifiersInfo(eventDeclaration);

                        return (allowOverride || !info.HasOverride)
                            && (!accessibility.IsPrivate() || !info.HasAbstractOrVirtualOrOverride)
                            && CheckProtectedInStaticOrSealedClass(node, accessibility)
                            && CheckAccessorAccessibility(eventDeclaration.AccessorList, accessibility);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;

                        ModifiersInfo info = SyntaxInfo.ModifiersInfo(indexerDeclaration);

                        return (allowOverride || !info.HasOverride)
                            && (!accessibility.IsPrivate() || !info.HasAbstractOrVirtualOrOverride)
                            && CheckProtectedInStaticOrSealedClass(node, accessibility)
                            && CheckAccessorAccessibility(indexerDeclaration.AccessorList, accessibility);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)node;

                        ModifiersInfo info = SyntaxInfo.ModifiersInfo(propertyDeclaration);

                        return (allowOverride || !info.HasOverride)
                            && (!accessibility.IsPrivate() || !info.HasAbstractOrVirtualOrOverride)
                            && CheckProtectedInStaticOrSealedClass(node, accessibility)
                            && CheckAccessorAccessibility(propertyDeclaration.AccessorList, accessibility);
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;

                        ModifiersInfo info = SyntaxInfo.ModifiersInfo(methodDeclaration);

                        return (allowOverride || !info.HasOverride)
                            && (!accessibility.IsPrivate() || !info.HasAbstractOrVirtualOrOverride)
                            && CheckProtectedInStaticOrSealedClass(node, accessibility);
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        var eventFieldDeclaration = (EventFieldDeclarationSyntax)node;

                        ModifiersInfo info = SyntaxInfo.ModifiersInfo(eventFieldDeclaration);

                        return (allowOverride || !info.HasOverride)
                            && (!accessibility.IsPrivate() || !info.HasAbstractOrVirtualOrOverride)
                            && CheckProtectedInStaticOrSealedClass(node, accessibility);
                    }
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.IncompleteMember:
                    {
                        return CheckProtectedInStaticOrSealedClass(node, accessibility);
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
                        var memberDeclaration = node.Parent?.Parent as MemberDeclarationSyntax;

                        Debug.Assert(memberDeclaration != null, node.ToString());

                        if (memberDeclaration != null)
                        {
                            if (!CheckProtectedInStaticOrSealedClass(memberDeclaration, accessibility))
                                return false;

                            Accessibility declarationAccessibility = memberDeclaration.GetModifiers().GetAccessibility();

                            if (declarationAccessibility == Accessibility.NotApplicable)
                                declarationAccessibility = memberDeclaration.GetDefaultExplicitAccessibility();

                            return accessibility.IsMoreRestrictiveThan(declarationAccessibility);
                        }

                        return false;
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        return false;
                    }
                default:
                    {
                        Debug.Fail(node.Kind().ToString());
                        return false;
                    }
            }
        }

        private static bool CheckProtectedInStaticOrSealedClass(SyntaxNode node, Accessibility accessibility)
        {
            return !accessibility.ContainsProtected()
                || (node.Parent as ClassDeclarationSyntax)?
                    .Modifiers
                    .ContainsAny(SyntaxKind.StaticKeyword, SyntaxKind.SealedKeyword) != true;
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
    }
}
