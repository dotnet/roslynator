// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;

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
            Accessibility newAccessibility = overrideInfo.OverriddenSymbol.DeclaredAccessibility;

            MemberDeclarationSyntax newNode = AccessibilityHelper.ChangeAccessibility(memberDeclaration, newAccessibility, ModifierComparer.Instance);

            return document.ReplaceNodeAsync(memberDeclaration, newNode, cancellationToken);
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
