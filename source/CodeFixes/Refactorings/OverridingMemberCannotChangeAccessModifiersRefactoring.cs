// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class OverridingMemberCannotChangeAccessModifiersRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            OverrideInfo overrideInfo,
            CancellationToken cancellationToken)
        {
            SyntaxTokenList modifiers = memberDeclaration.GetModifiers();

            Accessibility accessibility = overrideInfo.Symbol.DeclaredAccessibility;

            Accessibility newAccessibility = overrideInfo.OverriddenSymbol.DeclaredAccessibility;

            int index = IndexOfFirstAccessModifier(modifiers);

            SyntaxTokenList newModifiers = modifiers;

            if (index == -1)
            {
                foreach (SyntaxToken modifier in Modifiers.FromAccessibility(newAccessibility))
                    newModifiers = newModifiers.InsertModifier(modifier, ModifierComparer.Instance);
            }
            else
            {
                SyntaxToken modifier = modifiers[index];

                if (accessibility == Accessibility.ProtectedOrInternal)
                {
                    SyntaxToken newModifier = CreateAccessModifier(newAccessibility)
                        .WithLeadingTrivia(modifier.LeadingTrivia)
                        .WithTrailingTrivia(modifiers[index + 1].TrailingTrivia);

                    newModifiers = newModifiers
                        .ReplaceAt(index, newModifier)
                        .RemoveAt(index + 1);
                }
                else if (newAccessibility == Accessibility.ProtectedOrInternal)
                {
                    newModifiers = newModifiers
                        .ReplaceAt(index, ProtectedKeyword().WithLeadingTrivia(modifier.LeadingTrivia))
                        .Insert(index + 1, InternalKeyword().WithTrailingTrivia(modifier.TrailingTrivia));
                }
                else
                {
                    SyntaxToken newModifier = CreateAccessModifier(newAccessibility).WithTriviaFrom(modifier);

                    newModifiers = newModifiers.ReplaceAt(index, newModifier);
                }
            }

            return document.ReplaceNodeAsync(memberDeclaration, memberDeclaration.WithModifiers(newModifiers), cancellationToken);
        }

        private static SyntaxToken CreateAccessModifier(Accessibility accessibility)
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
                default:
                    return default(SyntaxToken);
            }
        }

        private static int IndexOfFirstAccessModifier(SyntaxTokenList modifiers)
        {
            for (int i = 0; i < modifiers.Count; i++)
            {
                if (modifiers[i].IsAccessModifier())
                    return i;
            }

            return -1;
        }

        internal static OverrideInfo GetOverrideInfo(MemberDeclarationSyntax memberDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, cancellationToken);

                        return new OverrideInfo(methodSymbol, methodSymbol.OverriddenMethod);
                    }
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.IndexerDeclaration:
                    {
                        var propertySymbol = (IPropertySymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, cancellationToken);

                        return new OverrideInfo(propertySymbol, propertySymbol.OverriddenProperty);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventSymbol = (IEventSymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, cancellationToken);

                        return new OverrideInfo(eventSymbol, eventSymbol.OverriddenEvent);
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        VariableDeclaratorSyntax declarator = ((EventFieldDeclarationSyntax)memberDeclaration).Declaration.Variables.First();

                        var eventSymbol = (IEventSymbol)semanticModel.GetDeclaredSymbol(declarator, cancellationToken);

                        return new OverrideInfo(eventSymbol, eventSymbol.OverriddenEvent);
                    }
                default:
                    {
                        Debug.Fail(memberDeclaration.Kind().ToString());

                        return null;
                    }
            }
        }
    }
}
