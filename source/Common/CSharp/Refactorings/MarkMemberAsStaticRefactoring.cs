// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkMemberAsStaticRefactoring
    {
        public static bool CanRefactor(FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            return !fieldDeclaration.IsStatic()
                && !fieldDeclaration.IsConst()
                && IsStaticClass(fieldDeclaration.Parent);
        }

        public static bool CanRefactor(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return !methodDeclaration.IsStatic()
                && IsStaticClass(methodDeclaration.Parent);
        }

        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return !propertyDeclaration.IsStatic()
                && IsStaticClass(propertyDeclaration.Parent);
        }

        public static bool CanRefactor(EventDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return !eventDeclaration.IsStatic()
                && IsStaticClass(eventDeclaration.Parent);
        }

        public static bool CanRefactor(EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (eventFieldDeclaration == null)
                throw new ArgumentNullException(nameof(eventFieldDeclaration));

            return !eventFieldDeclaration.IsStatic()
                && IsStaticClass(eventFieldDeclaration.Parent);
        }

        public static bool CanRefactor(ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return !constructorDeclaration.IsStatic()
                && IsStaticClass(constructorDeclaration.Parent);
        }

        private static bool IsStaticClass(SyntaxNode node)
        {
            return node?.IsKind(SyntaxKind.ClassDeclaration) == true
                && ((ClassDeclarationSyntax)node).IsStatic();
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.FieldDeclaration:
                    return await RefactorAsync(document, (FieldDeclarationSyntax)memberDeclaration, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.MethodDeclaration:
                    return await RefactorAsync(document, (MethodDeclarationSyntax)memberDeclaration, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.PropertyDeclaration:
                    return await RefactorAsync(document, (PropertyDeclarationSyntax)memberDeclaration, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.EventDeclaration:
                    return await RefactorAsync(document, (EventDeclarationSyntax)memberDeclaration, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.EventFieldDeclaration:
                    return await RefactorAsync(document, (EventFieldDeclarationSyntax)memberDeclaration, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.ConstructorDeclaration:
                    return await RefactorAsync(document, (ConstructorDeclarationSyntax)memberDeclaration, cancellationToken).ConfigureAwait(false);
            }

            Debug.Assert(false, "");
            return document;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            FieldDeclarationSyntax fieldDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            FieldDeclarationSyntax newNode = AddStaticModifier(fieldDeclaration);

            return document.ReplaceNodeAsync(fieldDeclaration, newNode, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            MethodDeclarationSyntax newNode = AddStaticModifier(methodDeclaration);

            return document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            PropertyDeclarationSyntax newNode = AddStaticModifier(propertyDeclaration);

            return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            EventDeclarationSyntax eventDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            EventDeclarationSyntax newNode = AddStaticModifier(eventDeclaration);

            return document.ReplaceNodeAsync(eventDeclaration, newNode, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            EventFieldDeclarationSyntax eventFieldDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (eventFieldDeclaration == null)
                throw new ArgumentNullException(nameof(eventFieldDeclaration));

            EventFieldDeclarationSyntax newNode = AddStaticModifier(eventFieldDeclaration);

            return document.ReplaceNodeAsync(eventFieldDeclaration, newNode, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ConstructorDeclarationSyntax constructorDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            ConstructorDeclarationSyntax newNode = AddStaticModifier(constructorDeclaration);

            return document.ReplaceNodeAsync(constructorDeclaration, newNode, cancellationToken);
        }

        public static FieldDeclarationSyntax AddStaticModifier(FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            SyntaxTokenList modifiers = fieldDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                return fieldDeclaration.InsertModifier(SyntaxKind.StaticKeyword, ModifierComparer.Instance);
            }
            else
            {
                return fieldDeclaration;
            }
        }

        public static MethodDeclarationSyntax AddStaticModifier(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            SyntaxTokenList modifiers = methodDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                return methodDeclaration.InsertModifier(SyntaxKind.StaticKeyword, ModifierComparer.Instance);
            }
            else
            {
                return methodDeclaration;
            }
        }

        public static PropertyDeclarationSyntax AddStaticModifier(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            SyntaxTokenList modifiers = propertyDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                return propertyDeclaration.InsertModifier(SyntaxKind.StaticKeyword, ModifierComparer.Instance);
            }
            else
            {
                return propertyDeclaration;
            }
        }

        public static EventDeclarationSyntax AddStaticModifier(EventDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            SyntaxTokenList modifiers = eventDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                return eventDeclaration.InsertModifier(SyntaxKind.StaticKeyword, ModifierComparer.Instance);
            }
            else
            {
                return eventDeclaration;
            }
        }

        public static EventFieldDeclarationSyntax AddStaticModifier(EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (eventFieldDeclaration == null)
                throw new ArgumentNullException(nameof(eventFieldDeclaration));

            SyntaxTokenList modifiers = eventFieldDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                return eventFieldDeclaration.InsertModifier(SyntaxKind.StaticKeyword, ModifierComparer.Instance);
            }
            else
            {
                return eventFieldDeclaration;
            }
        }

        public static ConstructorDeclarationSyntax AddStaticModifier(ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            SyntaxTokenList modifiers = constructorDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                SyntaxTokenList newModifiers = modifiers;

                if (modifiers.ContainsAccessModifier())
                {
                    newModifiers = modifiers.RemoveAccessModifiers();

                    if (newModifiers.Any())
                    {
                        newModifiers = newModifiers.ReplaceAt(0, newModifiers[0].WithLeadingTrivia(modifiers[0].LeadingTrivia));
                        newModifiers = newModifiers.InsertModifier(SyntaxKind.StaticKeyword, ModifierComparer.Instance);
                    }
                    else
                    {
                        newModifiers = newModifiers.InsertModifier(CSharpFactory.StaticKeyword().WithLeadingTrivia(modifiers[0].LeadingTrivia), ModifierComparer.Instance);
                    }
                }
                else
                {
                    newModifiers = newModifiers.InsertModifier(SyntaxKind.StaticKeyword, ModifierComparer.Instance);
                }

                return constructorDeclaration.WithModifiers(newModifiers);
            }
            else
            {
                return constructorDeclaration;
            }
        }
    }
}
