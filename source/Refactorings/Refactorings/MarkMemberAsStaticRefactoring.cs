// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkMemberAsStaticRefactoring
    {
        public static bool CanRefactor(FieldDeclarationSyntax fieldDeclaration)
        {
            return !fieldDeclaration.IsStatic()
                && !fieldDeclaration.IsConst()
                && IsStaticClass(fieldDeclaration.Parent);
        }

        public static bool CanRefactor(MethodDeclarationSyntax methodDeclaration)
        {
            return !methodDeclaration.IsStatic()
                && IsStaticClass(methodDeclaration.Parent);
        }

        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            return !propertyDeclaration.IsStatic()
                && IsStaticClass(propertyDeclaration.Parent);
        }

        public static bool CanRefactor(EventDeclarationSyntax eventDeclaration)
        {
            return !eventDeclaration.IsStatic()
                && IsStaticClass(eventDeclaration.Parent);
        }

        public static bool CanRefactor(EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            return !eventFieldDeclaration.IsStatic()
                && IsStaticClass(eventFieldDeclaration.Parent);
        }

        public static bool CanRefactor(ConstructorDeclarationSyntax constructorDeclaration)
        {
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
            FieldDeclarationSyntax fieldDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            FieldDeclarationSyntax newNode = AddStaticModifier(fieldDeclaration);

            return await document.ReplaceNodeAsync(fieldDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MethodDeclarationSyntax newNode = AddStaticModifier(methodDeclaration);

            return await document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            PropertyDeclarationSyntax newNode = AddStaticModifier(propertyDeclaration);

            return await document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            EventDeclarationSyntax eventDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            EventDeclarationSyntax newNode = AddStaticModifier(eventDeclaration);

            return await document.ReplaceNodeAsync(eventDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            EventFieldDeclarationSyntax eventFieldDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            EventFieldDeclarationSyntax newNode = AddStaticModifier(eventFieldDeclaration);

            return await document.ReplaceNodeAsync(eventFieldDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ConstructorDeclarationSyntax constructorDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ConstructorDeclarationSyntax newNode = AddStaticModifier(constructorDeclaration);

            return await document.ReplaceNodeAsync(constructorDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static FieldDeclarationSyntax AddStaticModifier(FieldDeclarationSyntax fieldDeclaration)
        {
            SyntaxTokenList modifiers = fieldDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                return Inserter.InsertModifier(fieldDeclaration, SyntaxKind.StaticKeyword);
            }
            else
            {
                return fieldDeclaration;
            }
        }

        public static MethodDeclarationSyntax AddStaticModifier(MethodDeclarationSyntax methodDeclaration)
        {
            SyntaxTokenList modifiers = methodDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                return Inserter.InsertModifier(methodDeclaration, SyntaxKind.StaticKeyword);
            }
            else
            {
                return methodDeclaration;
            }
        }

        public static PropertyDeclarationSyntax AddStaticModifier(PropertyDeclarationSyntax propertyDeclaration)
        {
            SyntaxTokenList modifiers = propertyDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                return Inserter.InsertModifier(propertyDeclaration, SyntaxKind.StaticKeyword);
            }
            else
            {
                return propertyDeclaration;
            }
        }

        public static EventDeclarationSyntax AddStaticModifier(EventDeclarationSyntax eventDeclaration)
        {
            SyntaxTokenList modifiers = eventDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                return Inserter.InsertModifier(eventDeclaration, SyntaxKind.StaticKeyword);
            }
            else
            {
                return eventDeclaration;
            }
        }

        public static EventFieldDeclarationSyntax AddStaticModifier(EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            SyntaxTokenList modifiers = eventFieldDeclaration.Modifiers;

            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                return Inserter.InsertModifier(eventFieldDeclaration, SyntaxKind.StaticKeyword);
            }
            else
            {
                return eventFieldDeclaration;
            }
        }

        public static ConstructorDeclarationSyntax AddStaticModifier(ConstructorDeclarationSyntax constructorDeclaration)
        {
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
                        newModifiers = Inserter.InsertModifier(newModifiers, SyntaxKind.StaticKeyword);
                    }
                    else
                    {
                        newModifiers = Inserter.InsertModifier(newModifiers, CSharpFactory.StaticKeyword().WithLeadingTrivia(modifiers[0].LeadingTrivia));
                    }
                }
                else
                {
                    newModifiers = Inserter.InsertModifier(newModifiers, SyntaxKind.StaticKeyword);
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
