// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.Refactorings.ChangeAccessibilityRefactoring;

namespace Roslynator.CSharp.Refactorings;

internal static class AccessModifierRefactoring
{
    public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxToken modifier)
    {
        SyntaxNode node = modifier.Parent;

        if (node.IsKind(SyntaxKind.DestructorDeclaration))
            return;

        ModifierListInfo modifiersInfo = SyntaxInfo.ModifierListInfo(node);

        if (node.IsKind(
            SyntaxKind.ClassDeclaration,
            SyntaxKind.InterfaceDeclaration,
#if ROSLYN_4_0
            SyntaxKind.RecordStructDeclaration,
#endif
            SyntaxKind.StructDeclaration))
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var symbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(node, context.CancellationToken);

            ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

            if (syntaxReferences.Length > 1)
            {
                ImmutableArray<MemberDeclarationSyntax> memberDeclarations = ImmutableArray.CreateRange(
                    syntaxReferences,
                    f => (MemberDeclarationSyntax)f.GetSyntax(context.CancellationToken));

                ImmutableArray<CodeAction>.Builder typeDeclarationActions = ImmutableArray.CreateBuilder<CodeAction>();

                foreach (Accessibility accessibility in AvailableAccessibilities)
                {
                    if (accessibility != modifiersInfo.ExplicitAccessibility
                        && SyntaxAccessibility.IsValidAccessibility(node, accessibility))
                    {
                        typeDeclarationActions.Add(CodeActionFactory.Create(
                            SyntaxFacts.GetText(accessibility),
                            ct => RefactorAsync(context.Solution, memberDeclarations, accessibility, ct),
                            RefactoringDescriptors.ChangeAccessibility,
                            accessibility.ToString()));
                    }
                }

                context.RegisterRefactoring(
                    "Change accessibility to",
                    typeDeclarationActions.ToImmutable());

                return;
            }
        }

        ImmutableArray<CodeAction>.Builder codeActions = ImmutableArray.CreateBuilder<CodeAction>();

        foreach (Accessibility accessibility in AvailableAccessibilities)
        {
            if (accessibility == modifiersInfo.ExplicitAccessibility)
                continue;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ISymbol symbol = GetBaseSymbolOrDefault(semanticModel, context.CancellationToken);

            if (symbol is not null)
            {
                if (SyntaxAccessibility.IsValidAccessibility(node, accessibility, ignoreOverride: true))
                {
                    codeActions.Add(CodeActionFactory.Create(
                        SyntaxFacts.GetText(accessibility),
                        ct => RefactorAsync(context.Solution, symbol, accessibility, ct),
                        RefactoringDescriptors.ChangeAccessibility,
                        accessibility.ToString()));
                }
            }
            else if (SyntaxAccessibility.IsValidAccessibility(node, accessibility))
            {
                codeActions.Add(CodeActionFactory.Create(
                    SyntaxFacts.GetText(accessibility),
                    ct => RefactorAsync(context.Document, node, accessibility, ct),
                    RefactoringDescriptors.ChangeAccessibility,
                    accessibility.ToString()));
            }
        }

        context.RegisterRefactoring(
            "Change accessibility to",
            codeActions.ToImmutable());

        ISymbol GetBaseSymbolOrDefault(SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (modifiersInfo.GetFilter().HasAnyFlag(ModifierFilter.AbstractVirtualOverride))
                return ChangeAccessibilityRefactoring.GetBaseSymbolOrDefault(node, semanticModel, cancellationToken);

            return null;
        }
    }
}
