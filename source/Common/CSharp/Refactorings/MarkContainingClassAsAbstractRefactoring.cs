// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Helpers;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkContainingClassAsAbstractRefactoring
    {
        public static bool CanRefactor(MethodDeclarationSyntax methodDeclaration)
        {
            return ComputeRefactoring(methodDeclaration, methodDeclaration.Modifiers);
        }

        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            return ComputeRefactoring(propertyDeclaration, propertyDeclaration.Modifiers);
        }

        public static bool CanRefactor(IndexerDeclarationSyntax indexerDeclaration)
        {
            return ComputeRefactoring(indexerDeclaration, indexerDeclaration.Modifiers);
        }

        public static bool CanRefactor(EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            return ComputeRefactoring(eventFieldDeclaration, eventFieldDeclaration.Modifiers);
        }

        private static bool ComputeRefactoring(MemberDeclarationSyntax memberDeclaration, SyntaxTokenList modifiers)
        {
            if (modifiers.Contains(SyntaxKind.AbstractKeyword)
                && !modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                SyntaxNode parent = memberDeclaration.Parent;

                Debug.Assert(parent?.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.StructDeclaration) == true, parent?.Kind().ToString());

                if (parent?.IsKind(SyntaxKind.ClassDeclaration) == true)
                {
                    var classDeclaration = (ClassDeclarationSyntax)parent;

                    SyntaxTokenList classModifiers = classDeclaration.Modifiers;

                    return !classModifiers.Contains(SyntaxKind.AbstractKeyword)
                        && !classModifiers.Contains(SyntaxKind.StaticKeyword)
                        && !classModifiers.Contains(SyntaxKind.SealedKeyword);
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            return document.InsertModifierAsync(
                memberDeclaration.Parent,
                SyntaxKind.AbstractKeyword,
                ModifierComparer.Instance,
                cancellationToken);
        }
    }
}