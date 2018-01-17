// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AccessModifierRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxToken modifier)
        {
            SyntaxNode node = modifier.Parent;

            if (node.IsKind(SyntaxKind.DestructorDeclaration))
                return;

            ModifiersInfo modifiersInfo = SyntaxInfo.ModifiersInfo(node);

            if (node.IsKind(
                SyntaxKind.ClassDeclaration,
                SyntaxKind.InterfaceDeclaration,
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

                    foreach (Accessibility accessibility in ChangeAccessibilityRefactoring.Accessibilities)
                    {
                        if (accessibility != modifiersInfo.Accessibility
                            && CSharpUtility.IsAllowedAccessibility(node, accessibility))
                        {
                            context.RegisterRefactoring(
                                ChangeAccessibilityRefactoring.GetTitle(accessibility),
                                cancellationToken => ChangeAccessibilityRefactoring.RefactorAsync(context.Solution, memberDeclarations, accessibility, cancellationToken));
                        }
                    }

                    return;
                }
            }

            foreach (Accessibility accessibility in ChangeAccessibilityRefactoring.Accessibilities)
            {
                if (accessibility == modifiersInfo.Accessibility)
                    continue;

                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ISymbol symbol = GetBaseSymbolOrDefault(semanticModel, context.CancellationToken);

                if (symbol != null)
                {
                    if (CSharpUtility.IsAllowedAccessibility(node, accessibility, allowOverride: true))
                    {
                        context.RegisterRefactoring(
                            ChangeAccessibilityRefactoring.GetTitle(accessibility),
                            cancellationToken => ChangeAccessibilityRefactoring.RefactorAsync(context.Solution, symbol, accessibility, cancellationToken));
                    }
                }
                else if (CSharpUtility.IsAllowedAccessibility(node, accessibility))
                {
                    context.RegisterRefactoring(
                        ChangeAccessibilityRefactoring.GetTitle(accessibility),
                        cancellationToken => ChangeAccessibilityRefactoring.RefactorAsync(context.Document, node, accessibility, cancellationToken));
                }
            }

            ISymbol GetBaseSymbolOrDefault(SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                if (modifiersInfo.HasAbstractOrVirtualOrOverride)
                    return ChangeAccessibilityRefactoring.GetBaseSymbolOrDefault(node, semanticModel, cancellationToken);

                return null;
            }
        }
    }
}
