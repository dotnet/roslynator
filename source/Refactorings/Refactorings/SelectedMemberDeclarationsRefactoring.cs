// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SelectedMemberDeclarationsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, NamespaceDeclarationSyntax namespaceDeclaration)
        {
            if (!MemberDeclarationSelection.TryCreate(namespaceDeclaration, context.Span, out MemberDeclarationSelection selectedMembers))
                return;

            ComputeRefactoring(context, selectedMembers);
        }

        public static void ComputeRefactoring(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
        {
            if (!MemberDeclarationSelection.TryCreate(classDeclaration, context.Span, out MemberDeclarationSelection selectedMembers))
                return;

            ComputeRefactoring(context, selectedMembers);
        }

        public static void ComputeRefactoring(RefactoringContext context, StructDeclarationSyntax structDeclaration)
        {
            if (!MemberDeclarationSelection.TryCreate(structDeclaration, context.Span, out MemberDeclarationSelection selectedMembers))
                return;

            ComputeRefactoring(context, selectedMembers);
        }

        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSelection selectedMembers)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeAccessibility))
            {
                AccessibilityFlags accessibilityFlags = ChangeAccessibilityRefactoring.GetAccessibilityFlags(selectedMembers);

                if (accessibilityFlags != AccessibilityFlags.None)
                {
                    bool isAnyPartial = IsAnyPartial();

                    TryRegisterRefactoring(accessibilityFlags, Accessibility.Public, isAnyPartial);
                    TryRegisterRefactoring(accessibilityFlags, Accessibility.Internal, isAnyPartial);
                    TryRegisterRefactoring(accessibilityFlags, Accessibility.Protected, isAnyPartial);
                    TryRegisterRefactoring(accessibilityFlags, Accessibility.Private, isAnyPartial);
                }
            }

            void TryRegisterRefactoring(AccessibilityFlags accessibilityFlags, Accessibility accessibility, bool isAnyPartial)
            {
                if ((accessibilityFlags & accessibility.GetAccessibilityFlag()) != 0)
                {
                    if (isAnyPartial)
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

            bool IsAnyPartial()
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
                                if (((MethodDeclarationSyntax)member).Modifiers.Contains(SyntaxKind.PartialKeyword))
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
