// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;

namespace Roslynator
{
    public static class ModifierUtility
    {
        public static bool IsAccessModifier(SyntaxToken token)
        {
            return token.IsKind(
                SyntaxKind.PublicKeyword,
                SyntaxKind.InternalKeyword,
                SyntaxKind.ProtectedKeyword,
                SyntaxKind.PrivateKeyword);
        }

        public static bool ContainsAccessModifier(SyntaxTokenList tokenList)
        {
            return tokenList.Any(token => IsAccessModifier(token));
        }

        public static AccessModifier GetDefaultAccessModifier(MemberDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    {
                        if (((ConstructorDeclarationSyntax)declaration).Modifiers.Contains(SyntaxKind.StaticKeyword))
                            return AccessModifier.None;

                        return AccessModifier.Private;
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        return AccessModifier.None;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)declaration;

                        if (methodDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword)
                            || methodDeclaration.ExplicitInterfaceSpecifier != null
                            || methodDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return AccessModifier.None;
                        }

                        return AccessModifier.Private;
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)declaration;

                        if (propertyDeclaration.ExplicitInterfaceSpecifier != null
                            || propertyDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return AccessModifier.None;
                        }

                        return AccessModifier.Private;
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)declaration;

                        if (indexerDeclaration.ExplicitInterfaceSpecifier != null
                            || indexerDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return AccessModifier.None;
                        }

                        return AccessModifier.Private;
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)declaration;

                        if (eventDeclaration.ExplicitInterfaceSpecifier != null)
                            return AccessModifier.None;

                        return AccessModifier.Private;
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        if (declaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                            return AccessModifier.None;

                        return AccessModifier.Private;
                    }
                case SyntaxKind.FieldDeclaration:
                    {
                        return AccessModifier.Private;
                    }
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return AccessModifier.Public;
                    }
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.DelegateDeclaration:
                    {
                        if (declaration.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
                            return AccessModifier.Private;

                        return AccessModifier.Internal;
                    }
            }

            return AccessModifier.None;
        }

        public static AccessModifier GetAccessModifier(SyntaxTokenList tokenList)
        {
            int count = tokenList.Count;
            for (int i = 0; i < count; i++)
            {
                switch (tokenList[i].Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        return AccessModifier.Public;
                    case SyntaxKind.PrivateKeyword:
                        return AccessModifier.Private;
                    case SyntaxKind.InternalKeyword:
                        return GetAccessModifier(tokenList, i + 1, count, SyntaxKind.ProtectedKeyword, AccessModifier.Internal);
                    case SyntaxKind.ProtectedKeyword:
                        return GetAccessModifier(tokenList, i + 1, count, SyntaxKind.InternalKeyword, AccessModifier.Protected);
                }
            }

            return AccessModifier.None;
        }

        private static AccessModifier GetAccessModifier(SyntaxTokenList tokenList, int startIndex, int count, SyntaxKind kind, AccessModifier accessModifier)
        {
            for (int i = startIndex; i < count; i++)
            {
                if (tokenList[i].Kind()== kind)
                    return AccessModifier.ProtectedInternal;
            }

            return accessModifier;
        }

        public static bool IsSorted(SyntaxTokenList modifiers)
        {
            for (int i = 0; i < modifiers.Count - 1; i++)
            {
                if (ModifierComparer.Instance.Compare(modifiers[i], modifiers[i + 1]) >= 0)
                    return false;
            }

            return true;
        }
    }
}
