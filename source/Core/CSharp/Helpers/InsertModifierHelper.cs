// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Helpers
{
    internal static class InsertModifierHelper
    {
        public static SyntaxNode InsertModifier(SyntaxNode node, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(node, Token(modifierKind), comparer);
        }

        public static SyntaxNode InsertModifier(SyntaxNode node, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTokenList modifiers = node.GetModifiers();

            if (!modifiers.Any())
            {
                int position = modifiers.FullSpan.End;

                if (node.FullSpan.Contains(position))
                {
                    SyntaxToken nextToken = node.FindToken(position);

                    if (!nextToken.IsKind(SyntaxKind.None))
                    {
                        SyntaxTriviaList trivia = nextToken.LeadingTrivia;

                        if (trivia.Any())
                        {
                            SyntaxTriviaList leadingTrivia = modifier.LeadingTrivia;

                            if (!leadingTrivia.IsSingleElasticMarker())
                                trivia = trivia.AddRange(leadingTrivia);

                            return node
                                .ReplaceToken(nextToken, nextToken.WithoutLeadingTrivia())
                                .WithModifiers(TokenList(modifier.WithLeadingTrivia(trivia)));
                        }
                    }
                }
            }

            return node.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static ClassDeclarationSyntax InsertModifier(ClassDeclarationSyntax classDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(classDeclaration, Token(modifierKind), comparer);
        }

        public static ClassDeclarationSyntax InsertModifier(ClassDeclarationSyntax classDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            SyntaxTokenList modifiers = classDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                SyntaxToken classKeyword = classDeclaration.Keyword;

                return classDeclaration
                    .WithKeyword(classKeyword.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(classKeyword.LeadingTrivia)));
            }

            return classDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static ConstructorDeclarationSyntax InsertModifier(ConstructorDeclarationSyntax constructorDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(constructorDeclaration, Token(modifierKind), comparer);
        }

        public static ConstructorDeclarationSyntax InsertModifier(ConstructorDeclarationSyntax constructorDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            SyntaxTokenList modifiers = constructorDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                SyntaxToken identifier = constructorDeclaration.Identifier;

                return constructorDeclaration
                    .WithIdentifier(identifier.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(identifier.LeadingTrivia)));
            }

            return constructorDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static ConversionOperatorDeclarationSyntax InsertModifier(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(conversionOperatorDeclaration, Token(modifierKind), comparer);
        }

        public static ConversionOperatorDeclarationSyntax InsertModifier(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            SyntaxTokenList modifiers = conversionOperatorDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                SyntaxToken implicitOrExplicitKeyword = conversionOperatorDeclaration.ImplicitOrExplicitKeyword;

                return conversionOperatorDeclaration
                    .WithImplicitOrExplicitKeyword(implicitOrExplicitKeyword.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(implicitOrExplicitKeyword.LeadingTrivia)));
            }

            return conversionOperatorDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static DelegateDeclarationSyntax InsertModifier(DelegateDeclarationSyntax delegateDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(delegateDeclaration, Token(modifierKind), comparer);
        }

        public static DelegateDeclarationSyntax InsertModifier(DelegateDeclarationSyntax delegateDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (delegateDeclaration == null)
                throw new ArgumentNullException(nameof(delegateDeclaration));

            SyntaxTokenList modifiers = delegateDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                SyntaxToken delegateKeyword = delegateDeclaration.DelegateKeyword;

                return delegateDeclaration
                    .WithDelegateKeyword(delegateKeyword.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(delegateKeyword.LeadingTrivia)));
            }

            return delegateDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static DestructorDeclarationSyntax InsertModifier(DestructorDeclarationSyntax destructorDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(destructorDeclaration, Token(modifierKind), comparer);
        }

        public static DestructorDeclarationSyntax InsertModifier(DestructorDeclarationSyntax destructorDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (destructorDeclaration == null)
                throw new ArgumentNullException(nameof(destructorDeclaration));

            SyntaxTokenList modifiers = destructorDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                SyntaxToken identifier = destructorDeclaration.Identifier;

                return destructorDeclaration
                    .WithIdentifier(identifier.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(identifier.LeadingTrivia)));
            }

            return destructorDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static EnumDeclarationSyntax InsertModifier(EnumDeclarationSyntax enumDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(enumDeclaration, Token(modifierKind), comparer);
        }

        public static EnumDeclarationSyntax InsertModifier(EnumDeclarationSyntax enumDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            SyntaxTokenList modifiers = enumDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                SyntaxToken enumKeyword = enumDeclaration.EnumKeyword;

                return enumDeclaration
                    .WithEnumKeyword(enumKeyword.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(enumKeyword.LeadingTrivia)));
            }

            return enumDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static EventDeclarationSyntax InsertModifier(EventDeclarationSyntax eventDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(eventDeclaration, Token(modifierKind), comparer);
        }

        public static EventDeclarationSyntax InsertModifier(EventDeclarationSyntax eventDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            SyntaxTokenList modifiers = eventDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                SyntaxToken eventKeyword = eventDeclaration.EventKeyword;

                return eventDeclaration
                    .WithEventKeyword(eventKeyword.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(eventKeyword.LeadingTrivia)));
            }

            return eventDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static EventFieldDeclarationSyntax InsertModifier(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(eventFieldDeclaration, Token(modifierKind), comparer);
        }

        public static EventFieldDeclarationSyntax InsertModifier(EventFieldDeclarationSyntax eventFieldDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (eventFieldDeclaration == null)
                throw new ArgumentNullException(nameof(eventFieldDeclaration));

            SyntaxTokenList modifiers = eventFieldDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                SyntaxToken eventKeyword = eventFieldDeclaration.EventKeyword;

                return eventFieldDeclaration
                    .WithEventKeyword(eventKeyword.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(eventKeyword.LeadingTrivia)));
            }

            return eventFieldDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static FieldDeclarationSyntax InsertModifier(FieldDeclarationSyntax fieldDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(fieldDeclaration, Token(modifierKind), comparer);
        }

        public static FieldDeclarationSyntax InsertModifier(FieldDeclarationSyntax fieldDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            SyntaxTokenList modifiers = fieldDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                TypeSyntax type = fieldDeclaration.Declaration?.Type;

                return fieldDeclaration
                    .ReplaceNode(type, type.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(type.GetLeadingTrivia())));
            }

            return fieldDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static IndexerDeclarationSyntax InsertModifier(IndexerDeclarationSyntax indexerDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(indexerDeclaration, Token(modifierKind), comparer);
        }

        public static IndexerDeclarationSyntax InsertModifier(IndexerDeclarationSyntax indexerDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            SyntaxTokenList modifiers = indexerDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                TypeSyntax type = indexerDeclaration.Type;

                return indexerDeclaration
                    .WithType(type.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(type.GetLeadingTrivia())));
            }

            return indexerDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static InterfaceDeclarationSyntax InsertModifier(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(interfaceDeclaration, Token(modifierKind), comparer);
        }

        public static InterfaceDeclarationSyntax InsertModifier(InterfaceDeclarationSyntax interfaceDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            SyntaxTokenList modifiers = interfaceDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                SyntaxToken interfaceKeyword = interfaceDeclaration.Keyword;

                return interfaceDeclaration
                    .WithKeyword(interfaceKeyword.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(interfaceKeyword.LeadingTrivia)));
            }

            return interfaceDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static MethodDeclarationSyntax InsertModifier(MethodDeclarationSyntax methodDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(methodDeclaration, Token(modifierKind), comparer);
        }

        public static MethodDeclarationSyntax InsertModifier(MethodDeclarationSyntax methodDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            SyntaxTokenList modifiers = methodDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                TypeSyntax returnType = methodDeclaration.ReturnType;

                return methodDeclaration
                    .WithReturnType(returnType.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(returnType.GetLeadingTrivia())));
            }

            return methodDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static OperatorDeclarationSyntax InsertModifier(OperatorDeclarationSyntax operatorDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(operatorDeclaration, Token(modifierKind), comparer);
        }

        public static OperatorDeclarationSyntax InsertModifier(OperatorDeclarationSyntax operatorDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            SyntaxTokenList modifiers = operatorDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                TypeSyntax returnType = operatorDeclaration.ReturnType;

                return operatorDeclaration
                    .WithReturnType(returnType.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(returnType.GetLeadingTrivia())));
            }

            return operatorDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static PropertyDeclarationSyntax InsertModifier(PropertyDeclarationSyntax propertyDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(propertyDeclaration, Token(modifierKind), comparer);
        }

        public static PropertyDeclarationSyntax InsertModifier(PropertyDeclarationSyntax propertyDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            SyntaxTokenList modifiers = propertyDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                TypeSyntax type = propertyDeclaration.Type;

                return propertyDeclaration
                    .WithType(type.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(type.GetLeadingTrivia())));
            }

            return propertyDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }

        public static StructDeclarationSyntax InsertModifier(StructDeclarationSyntax structDeclaration, SyntaxKind modifierKind, IModifierComparer comparer)
        {
            return InsertModifier(structDeclaration, Token(modifierKind), comparer);
        }

        public static StructDeclarationSyntax InsertModifier(StructDeclarationSyntax structDeclaration, SyntaxToken modifier, IModifierComparer comparer)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            SyntaxTokenList modifiers = structDeclaration.Modifiers;

            if (!modifiers.Any())
            {
                SyntaxToken structKeyword = structDeclaration.Keyword;

                return structDeclaration
                    .WithKeyword(structKeyword.WithoutLeadingTrivia())
                    .WithModifiers(TokenList(modifier.WithLeadingTrivia(structKeyword.LeadingTrivia)));
            }

            return structDeclaration.WithModifiers(modifiers.InsertModifier(modifier, comparer));
        }
    }
}
