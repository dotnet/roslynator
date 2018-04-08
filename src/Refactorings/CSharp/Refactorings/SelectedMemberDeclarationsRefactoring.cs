// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SelectedMemberDeclarationsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, NamespaceDeclarationSyntax namespaceDeclaration)
        {
            if (MemberDeclarationListSelection.TryCreate(namespaceDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
                ComputeRefactoring(context, selectedMembers);
        }

        public static void ComputeRefactoring(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
        {
            if (MemberDeclarationListSelection.TryCreate(classDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
                ComputeRefactoring(context, selectedMembers);
        }

        public static void ComputeRefactoring(RefactoringContext context, StructDeclarationSyntax structDeclaration)
        {
            if (MemberDeclarationListSelection.TryCreate(structDeclaration, context.Span, out MemberDeclarationListSelection selectedMembers))
                ComputeRefactoring(context, selectedMembers);
        }

        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationListSelection selectedMembers)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeAccessibility))
            {
                Accessibilities validAccessibilities = ChangeAccessibilityAnalysis.GetValidAccessibilities(selectedMembers, allowOverride: true);

                if (validAccessibilities != Accessibilities.None)
                {
                    bool canHaveMultipleDeclarations = CanHaveMultipleDeclarations();

                    TryRegisterRefactoring(validAccessibilities, Accessibility.Public, canHaveMultipleDeclarations);
                    TryRegisterRefactoring(validAccessibilities, Accessibility.Internal, canHaveMultipleDeclarations);
                    TryRegisterRefactoring(validAccessibilities, Accessibility.Protected, canHaveMultipleDeclarations);
                    TryRegisterRefactoring(validAccessibilities, Accessibility.Private, canHaveMultipleDeclarations);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InitializeFieldFromConstructor))
                InitializeFieldFromConstructorRefactoring.ComputeRefactoring(context, selectedMembers);

            void TryRegisterRefactoring(Accessibilities accessibilities, Accessibility accessibility, bool canHaveMultipleDeclarations)
            {
                if ((accessibilities & accessibility.GetAccessibilities()) != 0)
                {
                    if (canHaveMultipleDeclarations)
                    {
                        context.RegisterRefactoring(
                            ChangeAccessibilityRefactoring.GetTitle(accessibility),
                            async cancellationToken =>
                            {
                                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
                                return await ChangeAccessibilityRefactoring.RefactorAsync(context.Document.Solution(), selectedMembers, accessibility, semanticModel, cancellationToken).ConfigureAwait(false);
                            });
                    }
                    else
                    {
                        context.RegisterRefactoring(
                            ChangeAccessibilityRefactoring.GetTitle(accessibility),
                            cancellationToken => ChangeAccessibilityRefactoring.RefactorAsync(context.Document, selectedMembers, accessibility, cancellationToken));
                    }
                }
            }

            bool CanHaveMultipleDeclarations()
            {
                foreach (MemberDeclarationSyntax member in selectedMembers)
                {
                    switch (member.Kind())
                    {
                        case SyntaxKind.ClassDeclaration:
                            {
                                if (((ClassDeclarationSyntax)member).Modifiers.Contains(SyntaxKind.PartialKeyword))
                                    return true;

                                break;
                            }
                        case SyntaxKind.InterfaceDeclaration:
                            {
                                if (((InterfaceDeclarationSyntax)member).Modifiers.Contains(SyntaxKind.PartialKeyword))
                                    return true;

                                break;
                            }
                        case SyntaxKind.StructDeclaration:
                            {
                                if (((StructDeclarationSyntax)member).Modifiers.Contains(SyntaxKind.PartialKeyword))
                                    return true;

                                break;
                            }
                        case SyntaxKind.MethodDeclaration:
                            {
                                if (((MethodDeclarationSyntax)member).Modifiers.ContainsAny(SyntaxKind.PartialKeyword, SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword))
                                    return true;

                                break;
                            }
                        case SyntaxKind.PropertyDeclaration:
                            {
                                if (((PropertyDeclarationSyntax)member).Modifiers.ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword))
                                    return true;

                                break;
                            }
                        case SyntaxKind.IndexerDeclaration:
                            {
                                if (((IndexerDeclarationSyntax)member).Modifiers.ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword))
                                    return true;

                                break;
                            }
                        case SyntaxKind.EventDeclaration:
                            {
                                if (((EventDeclarationSyntax)member).Modifiers.ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword))
                                    return true;

                                break;
                            }
                        case SyntaxKind.EventFieldDeclaration:
                            {
                                if (((EventFieldDeclarationSyntax)member).Modifiers.ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword))
                                    return true;

                                break;
                            }
                    }
                }

                return false;
            }
        }
    }
}
