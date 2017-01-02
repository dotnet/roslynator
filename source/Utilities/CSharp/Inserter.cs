// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    public static class Inserter
    {
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

        public static int GetModifierInsertIndex(MemberDeclarationSyntax memberDeclaration, SyntaxToken modifier)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return GetModifierInsertIndex(memberDeclaration.GetModifiers(), modifier);
        }

        public static int GetModifierInsertIndex(MemberDeclarationSyntax memberDeclaration, SyntaxKind modifierKind)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return GetModifierInsertIndex(memberDeclaration.GetModifiers(), modifierKind);
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

        public static int GetModifierInsertIndex(ClassDeclarationSyntax classDeclaration, SyntaxToken modifier)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return GetModifierInsertIndex(classDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(ClassDeclarationSyntax classDeclaration, SyntaxKind modifierKind)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return GetModifierInsertIndex(classDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(ConstructorDeclarationSyntax constructorDeclaration, SyntaxToken modifier)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return GetModifierInsertIndex(constructorDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(ConstructorDeclarationSyntax constructorDeclaration, SyntaxKind modifierKind)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return GetModifierInsertIndex(constructorDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxToken modifier)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            return GetModifierInsertIndex(conversionOperatorDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxKind modifierKind)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            return GetModifierInsertIndex(conversionOperatorDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(DelegateDeclarationSyntax delegateDeclaration, SyntaxToken modifier)
        {
            if (delegateDeclaration == null)
                throw new ArgumentNullException(nameof(delegateDeclaration));

            return GetModifierInsertIndex(delegateDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(DelegateDeclarationSyntax delegateDeclaration, SyntaxKind modifierKind)
        {
            if (delegateDeclaration == null)
                throw new ArgumentNullException(nameof(delegateDeclaration));

            return GetModifierInsertIndex(delegateDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(DestructorDeclarationSyntax destructorDeclaration, SyntaxToken modifier)
        {
            if (destructorDeclaration == null)
                throw new ArgumentNullException(nameof(destructorDeclaration));

            return GetModifierInsertIndex(destructorDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(DestructorDeclarationSyntax destructorDeclaration, SyntaxKind modifierKind)
        {
            if (destructorDeclaration == null)
                throw new ArgumentNullException(nameof(destructorDeclaration));

            return GetModifierInsertIndex(destructorDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(EnumDeclarationSyntax enumDeclaration, SyntaxToken modifier)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return GetModifierInsertIndex(enumDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(EnumDeclarationSyntax enumDeclaration, SyntaxKind modifierKind)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return GetModifierInsertIndex(enumDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(EventDeclarationSyntax eventDeclaration, SyntaxToken modifier)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return GetModifierInsertIndex(eventDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(EventDeclarationSyntax eventDeclaration, SyntaxKind modifierKind)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return GetModifierInsertIndex(eventDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxToken modifier)
        {
            if (eventFieldDeclaration == null)
                throw new ArgumentNullException(nameof(eventFieldDeclaration));

            return GetModifierInsertIndex(eventFieldDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxKind modifierKind)
        {
            if (eventFieldDeclaration == null)
                throw new ArgumentNullException(nameof(eventFieldDeclaration));

            return GetModifierInsertIndex(eventFieldDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(FieldDeclarationSyntax fieldDeclaration, SyntaxToken modifier)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            return GetModifierInsertIndex(fieldDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(FieldDeclarationSyntax fieldDeclaration, SyntaxKind modifierKind)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            return GetModifierInsertIndex(fieldDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(IndexerDeclarationSyntax indexerDeclaration, SyntaxToken modifier)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return GetModifierInsertIndex(indexerDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(IndexerDeclarationSyntax indexerDeclaration, SyntaxKind modifierKind)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return GetModifierInsertIndex(indexerDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxToken modifier)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return GetModifierInsertIndex(interfaceDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxKind modifierKind)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return GetModifierInsertIndex(interfaceDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(MethodDeclarationSyntax methodDeclaration, SyntaxToken modifier)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return GetModifierInsertIndex(methodDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(MethodDeclarationSyntax methodDeclaration, SyntaxKind modifierKind)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return GetModifierInsertIndex(methodDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(OperatorDeclarationSyntax operatorDeclaration, SyntaxToken modifier)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return GetModifierInsertIndex(operatorDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(OperatorDeclarationSyntax operatorDeclaration, SyntaxKind modifierKind)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return GetModifierInsertIndex(operatorDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(PropertyDeclarationSyntax propertyDeclaration, SyntaxToken modifier)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return GetModifierInsertIndex(propertyDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(PropertyDeclarationSyntax propertyDeclaration, SyntaxKind modifierKind)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return GetModifierInsertIndex(propertyDeclaration.Modifiers, modifierKind);
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

        public static int GetModifierInsertIndex(StructDeclarationSyntax structDeclaration, SyntaxToken modifier)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return GetModifierInsertIndex(structDeclaration.Modifiers, modifier);
        }

        public static int GetModifierInsertIndex(StructDeclarationSyntax structDeclaration, SyntaxKind modifierKind)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return GetModifierInsertIndex(structDeclaration.Modifiers, modifierKind);
        }

        public static SyntaxTokenList InsertModifier(SyntaxTokenList modifiers, SyntaxKind modifierKind)
        {
            return InsertModifier(modifiers, Token(modifierKind));
        }

        public static SyntaxTokenList InsertModifier(SyntaxTokenList modifiers, SyntaxToken modifier)
        {
            if (modifiers.Any())
            {
                int index = GetModifierInsertIndex(modifiers, modifier);

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
                                .WithTrailingSpace());
                }
            }
            else
            {
                return modifiers.Add(modifier);
            }
        }

        public static int GetModifierInsertIndex(SyntaxTokenList modifiers, SyntaxToken modifier)
        {
            return GetModifierInsertIndex(modifiers, ModifierComparer.GetOrderIndex(modifier));
        }

        public static int GetModifierInsertIndex(SyntaxTokenList modifiers, SyntaxKind kind)
        {
            return GetModifierInsertIndex(modifiers, ModifierComparer.GetOrderIndex(kind));
        }

        private static int GetModifierInsertIndex(SyntaxTokenList modifiers, int orderIndex)
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

        public static CompilationUnitSyntax InsertMember(CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return compilationUnit.WithMembers(InsertMember(compilationUnit.Members, member));
        }

        public static int GetMemberInsertIndex(CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return GetMemberInsertIndex(compilationUnit.Members, member);
        }

        public static int GetMemberInsertIndex(CompilationUnitSyntax compilationUnit, SyntaxKind memberKind)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return GetMemberInsertIndex(compilationUnit.Members, memberKind);
        }

        public static int GetFieldInsertIndex(CompilationUnitSyntax compilationUnit, bool isConst = false)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return GetFieldInsertIndex(compilationUnit.Members, isConst);
        }

        public static NamespaceDeclarationSyntax InsertMember(NamespaceDeclarationSyntax namespaceDeclaration, MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return namespaceDeclaration.WithMembers(InsertMember(namespaceDeclaration.Members, member));
        }

        public static int GetMemberInsertIndex(NamespaceDeclarationSyntax namespaceDeclaration, MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return GetMemberInsertIndex(namespaceDeclaration.Members, member);
        }

        public static int GetMemberInsertIndex(NamespaceDeclarationSyntax namespaceDeclaration, SyntaxKind memberKind)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return GetMemberInsertIndex(namespaceDeclaration.Members, memberKind);
        }

        public static int GetFieldInsertIndex(NamespaceDeclarationSyntax namespaceDeclaration, bool isConst = false)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return GetFieldInsertIndex(namespaceDeclaration.Members, isConst);
        }

        public static ClassDeclarationSyntax InsertMember(ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax member)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithMembers(InsertMember(classDeclaration.Members, member));
        }

        public static int GetMemberInsertIndex(ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax member)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return GetMemberInsertIndex(classDeclaration.Members, member);
        }

        public static int GetMemberInsertIndex(ClassDeclarationSyntax classDeclaration, SyntaxKind memberKind)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return GetMemberInsertIndex(classDeclaration.Members, memberKind);
        }

        public static int GetFieldInsertIndex(ClassDeclarationSyntax classDeclaration, bool isConst = false)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return GetFieldInsertIndex(classDeclaration.Members, isConst);
        }

        public static StructDeclarationSyntax InsertMember(StructDeclarationSyntax structDeclaration, MemberDeclarationSyntax member)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return structDeclaration.WithMembers(InsertMember(structDeclaration.Members, member));
        }

        public static int GetMemberInsertIndex(StructDeclarationSyntax structDeclaration, MemberDeclarationSyntax member)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return GetMemberInsertIndex(structDeclaration.Members, member);
        }

        public static int GetMemberInsertIndex(StructDeclarationSyntax structDeclaration, SyntaxKind memberKind)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return GetMemberInsertIndex(structDeclaration.Members, memberKind);
        }

        public static int GetFieldInsertIndex(StructDeclarationSyntax structDeclaration, bool isConst = false)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return GetFieldInsertIndex(structDeclaration.Members, isConst);
        }

        public static InterfaceDeclarationSyntax InsertMember(InterfaceDeclarationSyntax interfaceDeclaration, MemberDeclarationSyntax member)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return interfaceDeclaration.WithMembers(InsertMember(interfaceDeclaration.Members, member));
        }

        public static int GetMemberInsertIndex(InterfaceDeclarationSyntax interfaceDeclaration, MemberDeclarationSyntax member)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return GetMemberInsertIndex(interfaceDeclaration.Members, member);
        }

        public static int GetMemberInsertIndex(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxKind memberKind)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return GetMemberInsertIndex(interfaceDeclaration.Members, memberKind);
        }

        public static int GetFieldInsertIndex(InterfaceDeclarationSyntax interfaceDeclaration, bool isConst = false)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return GetFieldInsertIndex(interfaceDeclaration.Members, isConst);
        }

        public static SyntaxList<MemberDeclarationSyntax> InsertMember(SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax member)
        {
            return members.Insert(GetMemberInsertIndex(members, member), member);
        }

        public static int GetMemberInsertIndex(SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return GetMemberInsertIndex(members, MemberDeclarationComparer.GetOrderIndex(member));
        }

        public static int GetMemberInsertIndex(SyntaxList<MemberDeclarationSyntax> members, SyntaxKind kind)
        {
            return GetMemberInsertIndex(members, MemberDeclarationComparer.GetOrderIndex(kind));
        }

        public static int GetFieldInsertIndex(SyntaxList<MemberDeclarationSyntax> members, bool isConst)
        {
            return GetMemberInsertIndex(members, (isConst) ? 0 : 1);
        }

        private static int GetMemberInsertIndex(SyntaxList<MemberDeclarationSyntax> members, int orderIndex)
        {
            if (members.Any())
            {
                for (int i = orderIndex; i >= 0; i--)
                {
                    SyntaxKind kind = MemberDeclarationComparer.GetKind(i);

                    for (int j = members.Count - 1; j >= 0; j--)
                    {
                        if (IsMatch(members[j], kind, i))
                            return j + 1;
                    }
                }
            }

            return 0;
        }

        private static bool IsMatch(MemberDeclarationSyntax memberDeclaration, SyntaxKind kind, int orderIndex)
        {
            switch (orderIndex)
            {
                case 0:
                    {
                        return memberDeclaration.IsKind(SyntaxKind.FieldDeclaration)
                           && ((FieldDeclarationSyntax)memberDeclaration).IsConst();
                    }
                case 1:
                    {
                        return memberDeclaration.IsKind(SyntaxKind.FieldDeclaration)
                           && !((FieldDeclarationSyntax)memberDeclaration).IsConst();
                    }
                default:
                    {
                        return memberDeclaration.IsKind(kind);
                    }
            }
        }
    }
}
