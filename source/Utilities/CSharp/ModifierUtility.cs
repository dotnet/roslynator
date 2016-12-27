// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    public static class ModifierUtility
    {
        public static bool IsAccessModifier(SyntaxToken token)
        {
            return IsAccessModifier(token.Kind());
        }

        public static bool IsAccessModifier(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.PrivateKeyword:
                    return true;
                default:
                    return false;
            }
        }

        public static SyntaxTokenList RemoveAccessModifiers(SyntaxTokenList tokenList)
        {
            return SyntaxFactory.TokenList(tokenList.Where(token => !IsAccessModifier(token)));
        }

        public static bool ContainsAccessModifier(SyntaxTokenList tokenList)
        {
            return tokenList.Any(token => IsAccessModifier(token));
        }

        public static Accessibility GetDefaultAccessModifier(MemberDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    {
                        if (((ConstructorDeclarationSyntax)declaration).IsStatic())
                            return Accessibility.NotApplicable;

                        return Accessibility.Private;
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        return Accessibility.NotApplicable;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)declaration;

                        if (methodDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword)
                            || methodDeclaration.ExplicitInterfaceSpecifier != null
                            || methodDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.NotApplicable;
                        }

                        return Accessibility.Private;
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)declaration;

                        if (propertyDeclaration.ExplicitInterfaceSpecifier != null
                            || propertyDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.NotApplicable;
                        }

                        return Accessibility.Private;
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)declaration;

                        if (indexerDeclaration.ExplicitInterfaceSpecifier != null
                            || indexerDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.NotApplicable;
                        }

                        return Accessibility.Private;
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)declaration;

                        if (eventDeclaration.ExplicitInterfaceSpecifier != null)
                            return Accessibility.NotApplicable;

                        return Accessibility.Private;
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        if (declaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                            return Accessibility.NotApplicable;

                        return Accessibility.Private;
                    }
                case SyntaxKind.FieldDeclaration:
                    {
                        return Accessibility.Private;
                    }
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return Accessibility.Public;
                    }
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.DelegateDeclaration:
                    {
                        if (declaration.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
                            return Accessibility.Private;

                        return Accessibility.Internal;
                    }
            }

            return Accessibility.NotApplicable;
        }

        public static Accessibility GetAccessModifier(SyntaxTokenList tokenList)
        {
            int count = tokenList.Count;

            for (int i = 0; i < count; i++)
            {
                switch (tokenList[i].Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        return Accessibility.Public;
                    case SyntaxKind.PrivateKeyword:
                        return Accessibility.Private;
                    case SyntaxKind.InternalKeyword:
                        return GetAccessModifier(tokenList, i + 1, count, SyntaxKind.ProtectedKeyword, Accessibility.Internal);
                    case SyntaxKind.ProtectedKeyword:
                        return GetAccessModifier(tokenList, i + 1, count, SyntaxKind.InternalKeyword, Accessibility.Protected);
                }
            }

            return Accessibility.NotApplicable;
        }

        private static Accessibility GetAccessModifier(
            SyntaxTokenList tokenList,
            int startIndex,
            int count,
            SyntaxKind kind,
            Accessibility accessibility)
        {
            for (int i = startIndex; i < count; i++)
            {
                if (tokenList[i].Kind()== kind)
                    return Accessibility.ProtectedOrInternal;
            }

            return accessibility;
        }

        public static MemberDeclarationSyntax InsertModifier(MemberDeclarationSyntax memberDeclaration, SyntaxToken modifier)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return memberDeclaration.SetModifiers(InsertModifier(memberDeclaration.GetModifiers(), modifier));
        }

        public static MemberDeclarationSyntax InsertModifier(MemberDeclarationSyntax memberDeclaration, SyntaxKind modifierKind)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return memberDeclaration.SetModifiers(InsertModifier(memberDeclaration.GetModifiers(), modifierKind));
        }

        public static int GetInsertIndex(MemberDeclarationSyntax memberDeclaration, SyntaxToken modifier)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return GetInsertIndex(memberDeclaration.GetModifiers(), modifier);
        }

        public static int GetInsertIndex(MemberDeclarationSyntax memberDeclaration, SyntaxKind modifierKind)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return GetInsertIndex(memberDeclaration.GetModifiers(), modifierKind);
        }

        public static ClassDeclarationSyntax InsertModifier(ClassDeclarationSyntax classDeclaration, SyntaxToken modifier)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithModifiers(InsertModifier(classDeclaration.Modifiers, modifier));
        }

        public static ClassDeclarationSyntax InsertModifier(ClassDeclarationSyntax classDeclaration, SyntaxKind modifierKind)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithModifiers(InsertModifier(classDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(ClassDeclarationSyntax classDeclaration, SyntaxToken modifier)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return GetInsertIndex(classDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(ClassDeclarationSyntax classDeclaration, SyntaxKind modifierKind)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return GetInsertIndex(classDeclaration.Modifiers, modifierKind);
        }

        public static ConstructorDeclarationSyntax InsertModifier(ConstructorDeclarationSyntax constructorDeclaration, SyntaxToken modifier)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return constructorDeclaration.WithModifiers(InsertModifier(constructorDeclaration.Modifiers, modifier));
        }

        public static ConstructorDeclarationSyntax InsertModifier(ConstructorDeclarationSyntax constructorDeclaration, SyntaxKind modifierKind)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return constructorDeclaration.WithModifiers(InsertModifier(constructorDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(ConstructorDeclarationSyntax constructorDeclaration, SyntaxToken modifier)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return GetInsertIndex(constructorDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(ConstructorDeclarationSyntax constructorDeclaration, SyntaxKind modifierKind)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return GetInsertIndex(constructorDeclaration.Modifiers, modifierKind);
        }

        public static ConversionOperatorDeclarationSyntax InsertModifier(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxToken modifier)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            return conversionOperatorDeclaration.WithModifiers(InsertModifier(conversionOperatorDeclaration.Modifiers, modifier));
        }

        public static ConversionOperatorDeclarationSyntax InsertModifier(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxKind modifierKind)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            return conversionOperatorDeclaration.WithModifiers(InsertModifier(conversionOperatorDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxToken modifier)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            return GetInsertIndex(conversionOperatorDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxKind modifierKind)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            return GetInsertIndex(conversionOperatorDeclaration.Modifiers, modifierKind);
        }

        public static DelegateDeclarationSyntax InsertModifier(DelegateDeclarationSyntax delegateDeclaration, SyntaxToken modifier)
        {
            if (delegateDeclaration == null)
                throw new ArgumentNullException(nameof(delegateDeclaration));

            return delegateDeclaration.WithModifiers(InsertModifier(delegateDeclaration.Modifiers, modifier));
        }

        public static DelegateDeclarationSyntax InsertModifier(DelegateDeclarationSyntax delegateDeclaration, SyntaxKind modifierKind)
        {
            if (delegateDeclaration == null)
                throw new ArgumentNullException(nameof(delegateDeclaration));

            return delegateDeclaration.WithModifiers(InsertModifier(delegateDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(DelegateDeclarationSyntax delegateDeclaration, SyntaxToken modifier)
        {
            if (delegateDeclaration == null)
                throw new ArgumentNullException(nameof(delegateDeclaration));

            return GetInsertIndex(delegateDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(DelegateDeclarationSyntax delegateDeclaration, SyntaxKind modifierKind)
        {
            if (delegateDeclaration == null)
                throw new ArgumentNullException(nameof(delegateDeclaration));

            return GetInsertIndex(delegateDeclaration.Modifiers, modifierKind);
        }

        public static DestructorDeclarationSyntax InsertModifier(DestructorDeclarationSyntax destructorDeclaration, SyntaxToken modifier)
        {
            if (destructorDeclaration == null)
                throw new ArgumentNullException(nameof(destructorDeclaration));

            return destructorDeclaration.WithModifiers(InsertModifier(destructorDeclaration.Modifiers, modifier));
        }

        public static DestructorDeclarationSyntax InsertModifier(DestructorDeclarationSyntax destructorDeclaration, SyntaxKind modifierKind)
        {
            if (destructorDeclaration == null)
                throw new ArgumentNullException(nameof(destructorDeclaration));

            return destructorDeclaration.WithModifiers(InsertModifier(destructorDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(DestructorDeclarationSyntax destructorDeclaration, SyntaxToken modifier)
        {
            if (destructorDeclaration == null)
                throw new ArgumentNullException(nameof(destructorDeclaration));

            return GetInsertIndex(destructorDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(DestructorDeclarationSyntax destructorDeclaration, SyntaxKind modifierKind)
        {
            if (destructorDeclaration == null)
                throw new ArgumentNullException(nameof(destructorDeclaration));

            return GetInsertIndex(destructorDeclaration.Modifiers, modifierKind);
        }

        public static EnumDeclarationSyntax InsertModifier(EnumDeclarationSyntax enumDeclaration, SyntaxToken modifier)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return enumDeclaration.WithModifiers(InsertModifier(enumDeclaration.Modifiers, modifier));
        }

        public static EnumDeclarationSyntax InsertModifier(EnumDeclarationSyntax enumDeclaration, SyntaxKind modifierKind)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return enumDeclaration.WithModifiers(InsertModifier(enumDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(EnumDeclarationSyntax enumDeclaration, SyntaxToken modifier)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return GetInsertIndex(enumDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(EnumDeclarationSyntax enumDeclaration, SyntaxKind modifierKind)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return GetInsertIndex(enumDeclaration.Modifiers, modifierKind);
        }

        public static EventDeclarationSyntax InsertModifier(EventDeclarationSyntax eventDeclaration, SyntaxToken modifier)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return eventDeclaration.WithModifiers(InsertModifier(eventDeclaration.Modifiers, modifier));
        }

        public static EventDeclarationSyntax InsertModifier(EventDeclarationSyntax eventDeclaration, SyntaxKind modifierKind)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return eventDeclaration.WithModifiers(InsertModifier(eventDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(EventDeclarationSyntax eventDeclaration, SyntaxToken modifier)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return GetInsertIndex(eventDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(EventDeclarationSyntax eventDeclaration, SyntaxKind modifierKind)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return GetInsertIndex(eventDeclaration.Modifiers, modifierKind);
        }

        public static EventFieldDeclarationSyntax InsertModifier(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxToken modifier)
        {
            if (eventFieldDeclaration == null)
                throw new ArgumentNullException(nameof(eventFieldDeclaration));

            return eventFieldDeclaration.WithModifiers(InsertModifier(eventFieldDeclaration.Modifiers, modifier));
        }

        public static EventFieldDeclarationSyntax InsertModifier(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxKind modifierKind)
        {
            if (eventFieldDeclaration == null)
                throw new ArgumentNullException(nameof(eventFieldDeclaration));

            return eventFieldDeclaration.WithModifiers(InsertModifier(eventFieldDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxToken modifier)
        {
            if (eventFieldDeclaration == null)
                throw new ArgumentNullException(nameof(eventFieldDeclaration));

            return GetInsertIndex(eventFieldDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxKind modifierKind)
        {
            if (eventFieldDeclaration == null)
                throw new ArgumentNullException(nameof(eventFieldDeclaration));

            return GetInsertIndex(eventFieldDeclaration.Modifiers, modifierKind);
        }

        public static FieldDeclarationSyntax InsertModifier(FieldDeclarationSyntax fieldDeclaration, SyntaxToken modifier)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            return fieldDeclaration.WithModifiers(InsertModifier(fieldDeclaration.Modifiers, modifier));
        }

        public static FieldDeclarationSyntax InsertModifier(FieldDeclarationSyntax fieldDeclaration, SyntaxKind modifierKind)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            return fieldDeclaration.WithModifiers(InsertModifier(fieldDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(FieldDeclarationSyntax fieldDeclaration, SyntaxToken modifier)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            return GetInsertIndex(fieldDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(FieldDeclarationSyntax fieldDeclaration, SyntaxKind modifierKind)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            return GetInsertIndex(fieldDeclaration.Modifiers, modifierKind);
        }

        public static IndexerDeclarationSyntax InsertModifier(IndexerDeclarationSyntax indexerDeclaration, SyntaxToken modifier)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return indexerDeclaration.WithModifiers(InsertModifier(indexerDeclaration.Modifiers, modifier));
        }

        public static IndexerDeclarationSyntax InsertModifier(IndexerDeclarationSyntax indexerDeclaration, SyntaxKind modifierKind)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return indexerDeclaration.WithModifiers(InsertModifier(indexerDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(IndexerDeclarationSyntax indexerDeclaration, SyntaxToken modifier)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return GetInsertIndex(indexerDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(IndexerDeclarationSyntax indexerDeclaration, SyntaxKind modifierKind)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return GetInsertIndex(indexerDeclaration.Modifiers, modifierKind);
        }

        public static InterfaceDeclarationSyntax InsertModifier(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxToken modifier)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return interfaceDeclaration.WithModifiers(InsertModifier(interfaceDeclaration.Modifiers, modifier));
        }

        public static InterfaceDeclarationSyntax InsertModifier(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxKind modifierKind)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return interfaceDeclaration.WithModifiers(InsertModifier(interfaceDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxToken modifier)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return GetInsertIndex(interfaceDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxKind modifierKind)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return GetInsertIndex(interfaceDeclaration.Modifiers, modifierKind);
        }

        public static MethodDeclarationSyntax InsertModifier(MethodDeclarationSyntax methodDeclaration, SyntaxToken modifier)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.WithModifiers(InsertModifier(methodDeclaration.Modifiers, modifier));
        }

        public static MethodDeclarationSyntax InsertModifier(MethodDeclarationSyntax methodDeclaration, SyntaxKind modifierKind)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.WithModifiers(InsertModifier(methodDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(MethodDeclarationSyntax methodDeclaration, SyntaxToken modifier)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return GetInsertIndex(methodDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(MethodDeclarationSyntax methodDeclaration, SyntaxKind modifierKind)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return GetInsertIndex(methodDeclaration.Modifiers, modifierKind);
        }

        public static OperatorDeclarationSyntax InsertModifier(OperatorDeclarationSyntax operatorDeclaration, SyntaxToken modifier)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return operatorDeclaration.WithModifiers(InsertModifier(operatorDeclaration.Modifiers, modifier));
        }

        public static OperatorDeclarationSyntax InsertModifier(OperatorDeclarationSyntax operatorDeclaration, SyntaxKind modifierKind)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return operatorDeclaration.WithModifiers(InsertModifier(operatorDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(OperatorDeclarationSyntax operatorDeclaration, SyntaxToken modifier)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return GetInsertIndex(operatorDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(OperatorDeclarationSyntax operatorDeclaration, SyntaxKind modifierKind)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return GetInsertIndex(operatorDeclaration.Modifiers, modifierKind);
        }

        public static PropertyDeclarationSyntax InsertModifier(PropertyDeclarationSyntax propertyDeclaration, SyntaxToken modifier)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithModifiers(InsertModifier(propertyDeclaration.Modifiers, modifier));
        }

        public static PropertyDeclarationSyntax InsertModifier(PropertyDeclarationSyntax propertyDeclaration, SyntaxKind modifierKind)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithModifiers(InsertModifier(propertyDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(PropertyDeclarationSyntax propertyDeclaration, SyntaxToken modifier)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return GetInsertIndex(propertyDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(PropertyDeclarationSyntax propertyDeclaration, SyntaxKind modifierKind)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return GetInsertIndex(propertyDeclaration.Modifiers, modifierKind);
        }

        public static StructDeclarationSyntax InsertModifier(StructDeclarationSyntax structDeclaration, SyntaxToken modifier)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return structDeclaration.WithModifiers(InsertModifier(structDeclaration.Modifiers, modifier));
        }

        public static StructDeclarationSyntax InsertModifier(StructDeclarationSyntax structDeclaration, SyntaxKind modifierKind)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return structDeclaration.WithModifiers(InsertModifier(structDeclaration.Modifiers, modifierKind));
        }

        public static int GetInsertIndex(StructDeclarationSyntax structDeclaration, SyntaxToken modifier)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return GetInsertIndex(structDeclaration.Modifiers, modifier);
        }

        public static int GetInsertIndex(StructDeclarationSyntax structDeclaration, SyntaxKind modifierKind)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return GetInsertIndex(structDeclaration.Modifiers, modifierKind);
        }

        public static SyntaxTokenList InsertModifier(SyntaxTokenList modifiers, SyntaxKind modifierKind)
        {
            return InsertModifier(modifiers, Token(modifierKind));
        }

        public static SyntaxTokenList InsertModifier(SyntaxTokenList modifiers, SyntaxToken modifier)
        {
            if (modifiers.Any())
            {
                int index = GetInsertIndex(modifiers, modifier);

                if (index == modifiers.Count)
                {
                    return modifiers.Add(modifier.PrependLeadingTrivia(Space));
                }
                else
                {
                    SyntaxToken nextModifier = modifiers[index];

                    return modifiers
                        .Replace(nextModifier, nextModifier.WithoutLeadingTrivia())
                        .Insert(
                            index,
                            modifier
                                .WithLeadingTrivia(nextModifier.LeadingTrivia)
                                .WithTrailingTrivia(Space));
                }
            }
            else
            {
                return modifiers.Add(modifier);
            }
        }

        public static int GetInsertIndex(SyntaxTokenList modifiers, SyntaxToken modifier)
        {
            return GetInsertIndex(modifiers, ModifierComparer.GetOrderIndex(modifier));
        }

        public static int GetInsertIndex(SyntaxTokenList modifiers, SyntaxKind kind)
        {
            return GetInsertIndex(modifiers, ModifierComparer.GetOrderIndex(kind));
        }

        private static int GetInsertIndex(SyntaxTokenList modifiers, int orderIndex)
        {
            if (modifiers.Any())
            {
                for (int i = orderIndex; i >= 0; i--)
                {
                    SyntaxKind kind = ModifierComparer.GetKind(i);

                    for (int j = modifiers.Count - 1; j >= 0; j--)
                    {
                        if (modifiers[j].IsKind(kind))
                            return j + 1;
                    }
                }
            }

            return 0;
        }
    }
}
