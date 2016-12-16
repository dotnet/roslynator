// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkContainingClassAsAbstractRefactoring
    {
        internal static void ComputeRefactoring(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            ComputeRefactoring(context, methodDeclaration, methodDeclaration.Modifiers);
        }

        internal static void ComputeRefactoring(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            ComputeRefactoring(context, propertyDeclaration, propertyDeclaration.Modifiers);
        }

        internal static void ComputeRefactoring(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            ComputeRefactoring(context, indexerDeclaration, indexerDeclaration.Modifiers);
        }

        internal static void ComputeRefactoring(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            ComputeRefactoring(context, eventFieldDeclaration, eventFieldDeclaration.Modifiers);
        }

        private static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSyntax memberDeclaration, SyntaxTokenList modifiers)
        {
            if (modifiers.Contains(SyntaxKind.AbstractKeyword)
                && !modifiers.Contains(SyntaxKind.StaticKeyword))
            {
                SyntaxNode parent = memberDeclaration.Parent;

                Debug.Assert(parent?.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.StructDeclaration) == true, parent?.Kind().ToString());

                if (parent?.IsKind(SyntaxKind.ClassDeclaration) == true)
                {
                    var classDeclaration = (ClassDeclarationSyntax)parent;

                    if (CanBeMarkedAsAbstract(classDeclaration))
                    {
                        context.RegisterRefactoring(
                            "Mark containing class as abstract",
                            cancellationToken =>
                            {
                                return AddModifierRefactoring.RefactorAsync(
                                    context.Document,
                                    classDeclaration,
                                    SyntaxKind.AbstractKeyword,
                                    cancellationToken);
                            });
                    }
                }
            }
        }

        private static bool CanBeMarkedAsAbstract(ClassDeclarationSyntax classDeclaration)
        {
            SyntaxTokenList modifiers = classDeclaration.Modifiers;

            return !modifiers.Contains(SyntaxKind.AbstractKeyword)
                && !modifiers.Contains(SyntaxKind.StaticKeyword)
                && !modifiers.Contains(SyntaxKind.SealedKeyword);
        }
    }
}