// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeVariableDeclarationTypeRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, VariableDeclarationSyntax variableDeclaration)
        {
            TypeSyntax type = variableDeclaration.Type;

            if (type?.Span.Contains(context.Span) == true
                && context.IsAnyRefactoringEnabled(
                    RefactoringDescriptors.UseImplicitType,
                    RefactoringDescriptors.UseExplicitType,
                    RefactoringDescriptors.ChangeTypeAccordingToExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                TypeAnalysis analysis = CSharpTypeAnalysis.AnalyzeType(variableDeclaration, semanticModel, context.CancellationToken);

                if (analysis.IsExplicit)
                {
                    if (analysis.SupportsImplicit
                        && context.IsRefactoringEnabled(RefactoringDescriptors.UseImplicitType))
                    {
                        context.RegisterRefactoring(CodeActionFactory.ChangeTypeToVar(context.Document, type, equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.UseImplicitType)));
                    }

                    if (!variableDeclaration.ContainsDiagnostics
                        && context.IsRefactoringEnabled(RefactoringDescriptors.ChangeTypeAccordingToExpression))
                    {
                        ChangeTypeAccordingToExpression(context, variableDeclaration, analysis.Symbol, semanticModel);
                    }
                }
                else if (analysis.SupportsExplicit
                    && context.IsRefactoringEnabled(RefactoringDescriptors.UseExplicitType))
                {
                    ITypeSymbol typeSymbol = analysis.Symbol;

                    VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.SingleOrDefault(shouldThrow: false);

                    if (variableDeclarator?.Initializer?.Value != null)
                    {
                        if (typeSymbol.OriginalDefinition.EqualsOrInheritsFromTaskOfT())
                        {
                            Func<CancellationToken, Task<Document>> createChangedDocument = DocumentRefactoringFactory.ChangeTypeAndAddAwait(
                                context.Document,
                                variableDeclaration,
                                variableDeclarator,
                                typeSymbol,
                                semanticModel,
                                context.CancellationToken);

                            if (createChangedDocument != null)
                            {
                                context.RegisterRefactoring(
                                    "Use explicit type (and add 'await')",
                                    createChangedDocument,
                                    RefactoringDescriptors.UseExplicitType,
                                    "AddAwait");
                            }
                        }

                        typeSymbol = semanticModel.GetTypeSymbol(variableDeclarator.Initializer.Value, context.CancellationToken);

                        if (typeSymbol != null)
                        {
                            context.RegisterRefactoring(CodeActionFactory.UseExplicitType(context.Document, type, typeSymbol, semanticModel, equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.UseExplicitType)));
                        }
                    }
                }
            }
        }

        private static void ChangeTypeAccordingToExpression(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel)
        {
            foreach (VariableDeclaratorSyntax variableDeclarator in variableDeclaration.Variables)
            {
                ExpressionSyntax value = variableDeclarator.Initializer?.Value;

                if (value == null)
                    return;

                Conversion conversion = semanticModel.ClassifyConversion(value, typeSymbol);

                if (conversion.IsIdentity)
                    return;

                if (!conversion.IsImplicit)
                    return;
            }

            ITypeSymbol newTypeSymbol = semanticModel.GetTypeSymbol(variableDeclaration.Variables[0].Initializer.Value, context.CancellationToken);

            if (newTypeSymbol == null)
                return;

            context.RegisterRefactoring(CodeActionFactory.UseExplicitType(context.Document, variableDeclaration.Type, newTypeSymbol, semanticModel, equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.ChangeTypeAccordingToExpression)));
        }
    }
}
