// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
                    RefactoringIdentifiers.ChangeExplicitTypeToVar,
                    RefactoringIdentifiers.ChangeVarToExplicitType,
                    RefactoringIdentifiers.ChangeTypeAccordingToExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                TypeAnalysis analysis = CSharpTypeAnalysis.AnalyzeType(variableDeclaration, semanticModel, context.CancellationToken);

                if (analysis.IsExplicit)
                {
                    if (analysis.SupportsImplicit
                        && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeExplicitTypeToVar))
                    {
                        context.RegisterRefactoring(CodeActionFactory.ChangeTypeToVar(context.Document, type, equivalenceKey: RefactoringIdentifiers.ChangeExplicitTypeToVar));
                    }

                    if (!variableDeclaration.ContainsDiagnostics
                        && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeTypeAccordingToExpression))
                    {
                        ChangeTypeAccordingToExpression(context, variableDeclaration, analysis.Symbol, semanticModel);
                    }
                }
                else if (analysis.SupportsExplicit
                    && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeVarToExplicitType))
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                    VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.SingleOrDefault(shouldThrow: false);

                    if (variableDeclarator?.Initializer?.Value != null
                        && typeSymbol.OriginalDefinition.EqualsOrInheritsFromTaskOfT())
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
                            ITypeSymbol typeArgument = ((INamedTypeSymbol)typeSymbol).TypeArguments[0];

                            context.RegisterRefactoring(
                                $"Change type to '{SymbolDisplay.ToMinimalDisplayString(typeArgument, semanticModel, type.SpanStart)}' and add 'await'",
                                createChangedDocument,
                                EquivalenceKey.Join(RefactoringIdentifiers.ChangeVarToExplicitType, "AddAwait"));
                        }
                    }

                    context.RegisterRefactoring(CodeActionFactory.ChangeType(context.Document, type, typeSymbol, semanticModel, equivalenceKey: RefactoringIdentifiers.ChangeVarToExplicitType));
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

            context.RegisterRefactoring(CodeActionFactory.ChangeType(context.Document, variableDeclaration.Type, newTypeSymbol, semanticModel, equivalenceKey: RefactoringIdentifiers.ChangeTypeAccordingToExpression));
        }
    }
}