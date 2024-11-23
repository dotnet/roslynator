// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings;

internal static class SelectedMemberDeclarationsRefactoring
{
    public static async Task ComputeRefactoringAsync(RefactoringContext context, MemberDeclarationListSelection selectedMembers)
    {
        if (context.IsRefactoringEnabled(RefactoringDescriptors.ChangeAccessibility)
            && !selectedMembers.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            AccessibilityFilter validAccessibilities = ChangeAccessibilityAnalysis.GetValidAccessibilityFilter(selectedMembers, semanticModel, context.CancellationToken);

            if (validAccessibilities != AccessibilityFilter.None)
            {
                bool canHaveMultipleDeclarations = CanHaveMultipleDeclarations();

                ImmutableArray<CodeAction>.Builder codeActions = ImmutableArray.CreateBuilder<CodeAction>();

                TryAddCodeAction(validAccessibilities, Accessibility.Public, canHaveMultipleDeclarations, codeActions);
                TryAddCodeAction(validAccessibilities, Accessibility.Internal, canHaveMultipleDeclarations, codeActions);
                TryAddCodeAction(validAccessibilities, Accessibility.Protected, canHaveMultipleDeclarations, codeActions);
                TryAddCodeAction(validAccessibilities, Accessibility.Private, canHaveMultipleDeclarations, codeActions);

                context.RegisterRefactoring(
                    "Change accessibility to",
                    codeActions.ToImmutable());
            }
        }

        if (context.IsAnyRefactoringEnabled(
            RefactoringDescriptors.ConvertBlockBodyToExpressionBody,
            RefactoringDescriptors.ConvertExpressionBodyToBlockBody))
        {
            ConvertBodyAndExpressionBodyRefactoring.ComputeRefactoring(context, selectedMembers);
        }

        if (context.IsRefactoringEnabled(RefactoringDescriptors.InitializeFieldFromConstructor)
            && !selectedMembers.Parent.IsKind(SyntaxKind.InterfaceDeclaration))
        {
            InitializeFieldFromConstructorRefactoring.ComputeRefactoring(context, selectedMembers);
        }

        if (context.IsRefactoringEnabled(RefactoringDescriptors.AddEmptyLineBetweenDeclarations))
        {
            AddEmptyLineBetweenDeclarationsRefactoring.ComputeRefactoring(context, selectedMembers);
        }

        void TryAddCodeAction(AccessibilityFilter accessibilities, Accessibility accessibility, bool canHaveMultipleDeclarations, ImmutableArray<CodeAction>.Builder codeActions)
        {
            CodeAction codeAction = TryCreateCodeAction(accessibilities, accessibility, canHaveMultipleDeclarations);

            if (codeAction is not null)
                codeActions.Add(codeAction);
        }

        CodeAction TryCreateCodeAction(AccessibilityFilter accessibilities, Accessibility accessibility, bool canHaveMultipleDeclarations)
        {
            if ((accessibilities & accessibility.GetAccessibilityFilter()) != 0)
            {
                if (canHaveMultipleDeclarations)
                {
                    return CodeActionFactory.Create(
                        SyntaxFacts.GetText(accessibility),
                        async ct =>
                        {
                            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(ct).ConfigureAwait(false);
                            return await ChangeAccessibilityRefactoring.RefactorAsync(context.Document.Solution(), selectedMembers, accessibility, semanticModel, ct).ConfigureAwait(false);
                        },
                        RefactoringDescriptors.ChangeAccessibility,
                        accessibility.ToString());
                }
                else
                {
                    return CodeActionFactory.Create(
                        SyntaxFacts.GetText(accessibility),
                        ct => ChangeAccessibilityRefactoring.RefactorAsync(context.Document, selectedMembers, accessibility, ct),
                        RefactoringDescriptors.ChangeAccessibility,
                        accessibility.ToString());
                }
            }

            return null;
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
                    case SyntaxKind.RecordDeclaration:
#if ROSLYN_4_0
                    case SyntaxKind.RecordStructDeclaration:
#endif
                    {
                        if (((RecordDeclarationSyntax)member).Modifiers.Contains(SyntaxKind.PartialKeyword))
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
