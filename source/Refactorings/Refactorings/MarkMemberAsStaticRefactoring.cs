// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkMemberAsStaticRefactoring
    {
        public static bool CanRefactor(FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration.Parent?.IsKind(SyntaxKind.ClassDeclaration) == true
                && !fieldDeclaration.Modifiers.Contains(SyntaxKind.ConstKeyword)
                && !fieldDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                var classDeclaration = (ClassDeclarationSyntax)fieldDeclaration.Parent;

                return classDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            FieldDeclarationSyntax fieldDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            FieldDeclarationSyntax newNode = AddStaticModifier(fieldDeclaration);

            return await document.ReplaceNodeAsync(fieldDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static bool CanRefactor(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.Parent?.IsKind(SyntaxKind.ClassDeclaration) == true
                && !methodDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                var classDeclaration = (ClassDeclarationSyntax)methodDeclaration.Parent;

                return classDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MethodDeclarationSyntax newNode = AddStaticModifier(methodDeclaration);

            return await document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.Parent?.IsKind(SyntaxKind.ClassDeclaration) == true
                && !propertyDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                var classDeclaration = (ClassDeclarationSyntax)propertyDeclaration.Parent;

                return classDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            PropertyDeclarationSyntax newNode = AddStaticModifier(propertyDeclaration);

            return await document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static bool CanRefactor(EventDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration.Parent?.IsKind(SyntaxKind.ClassDeclaration) == true
                && !eventDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                var classDeclaration = (ClassDeclarationSyntax)eventDeclaration.Parent;

                return classDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            EventDeclarationSyntax eventDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            EventDeclarationSyntax newNode = AddStaticModifier(eventDeclaration);

            return await document.ReplaceNodeAsync(eventDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static bool CanRefactor(EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (eventFieldDeclaration.Parent?.IsKind(SyntaxKind.ClassDeclaration) == true
                && !eventFieldDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                var classDeclaration = (ClassDeclarationSyntax)eventFieldDeclaration.Parent;

                return classDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            EventFieldDeclarationSyntax eventFieldDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            EventFieldDeclarationSyntax newNode = AddStaticModifier(eventFieldDeclaration);

            return await document.ReplaceNodeAsync(eventFieldDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static bool CanRefactor(ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration.Parent?.IsKind(SyntaxKind.ClassDeclaration) == true
                && !constructorDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                var classDeclaration = (ClassDeclarationSyntax)constructorDeclaration.Parent;

                return classDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ConstructorDeclarationSyntax constructorDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ConstructorDeclarationSyntax newNode = AddStaticModifier(constructorDeclaration);

            return await document.ReplaceNodeAsync(constructorDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static FieldDeclarationSyntax AddStaticModifier(FieldDeclarationSyntax node)
        {
            if (!node.Modifiers.Contains(SyntaxKind.StaticKeyword))
                return node.WithModifiers(AddStaticModifier(node.Modifiers));

            return node;
        }

        public static MethodDeclarationSyntax AddStaticModifier(MethodDeclarationSyntax node)
        {
            if (!node.Modifiers.Contains(SyntaxKind.StaticKeyword))
                return node.WithModifiers(AddStaticModifier(node.Modifiers));

            return node;
        }

        public static PropertyDeclarationSyntax AddStaticModifier(PropertyDeclarationSyntax node)
        {
            if (!node.Modifiers.Contains(SyntaxKind.StaticKeyword))
                return node.WithModifiers(AddStaticModifier(node.Modifiers));

            return node;
        }

        public static EventDeclarationSyntax AddStaticModifier(EventDeclarationSyntax node)
        {
            if (!node.Modifiers.Contains(SyntaxKind.StaticKeyword))
                return node.WithModifiers(AddStaticModifier(node.Modifiers));

            return node;
        }

        public static EventFieldDeclarationSyntax AddStaticModifier(EventFieldDeclarationSyntax node)
        {
            if (!node.Modifiers.Contains(SyntaxKind.StaticKeyword))
                return node.WithModifiers(AddStaticModifier(node.Modifiers));

            return node;
        }

        public static ConstructorDeclarationSyntax AddStaticModifier(ConstructorDeclarationSyntax node)
        {
            return node.WithModifiers(Modifiers.Static());
        }

        private static SyntaxTokenList AddStaticModifier(SyntaxTokenList modifiers)
        {
            return TokenList(
                modifiers.Add(CSharpFactory.StaticToken())
                    .OrderBy(f => f, ModifierComparer.Instance));
        }
    }
}
