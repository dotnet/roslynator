// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeDeclarationExpressionTypeRefactoring
    {
        public static async Task ComputeRefactoringsAsync(
            RefactoringContext context,
            DeclarationExpressionSyntax declarationExpression)
        {
            if (declarationExpression.Type?.Span.Contains(context.Span) == true
                && context.IsAnyRefactoringEnabled(
                    RefactoringDescriptors.UseImplicitType,
                    RefactoringDescriptors.UseExplicitType))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                TypeAnalysis analysis = CSharpTypeAnalysis.AnalyzeType(declarationExpression, semanticModel, context.CancellationToken);

                if (analysis.IsExplicit)
                {
                    if (analysis.SupportsImplicit
                        && context.IsRefactoringEnabled(RefactoringDescriptors.UseImplicitType))
                    {
                        context.RegisterRefactoring(CodeActionFactory.ChangeTypeToVar(context.Document, declarationExpression.Type, equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.UseImplicitType)));
                    }
                }
                else if (analysis.SupportsExplicit
                    && context.IsRefactoringEnabled(RefactoringDescriptors.UseExplicitType))
                {
                    TypeSyntax type = declarationExpression.Type;

                    var localSymbol = (ILocalSymbol)semanticModel.GetDeclaredSymbol(declarationExpression.Designation, context.CancellationToken);

                    ITypeSymbol typeSymbol = localSymbol.Type;

                    context.RegisterRefactoring(CodeActionFactory.ChangeType(context.Document, type, typeSymbol, semanticModel, equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.UseExplicitType)));
                }
            }
        }
    }
}
