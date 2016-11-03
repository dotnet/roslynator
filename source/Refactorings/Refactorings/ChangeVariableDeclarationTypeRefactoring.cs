// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeVariableDeclarationTypeRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, VariableDeclarationSyntax variableDeclaration)
        {
            if (context.SupportsSemanticModel
                && variableDeclaration.Type?.Span.Contains(context.Span) == true)
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeTypeAccordingToExpression))
                    await ChangeTypeAccordingToExpressionAsync(context, variableDeclaration).ConfigureAwait(false);

                if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ChangeExplicitTypeToVar,
                    RefactoringIdentifiers.ChangeVarToExplicitType))
                {
                    await ChangeTypeAsync(context, variableDeclaration).ConfigureAwait(false);
                }
            }
        }

        private static async Task ChangeTypeAccordingToExpressionAsync(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration)
        {
            if (variableDeclaration.Type?.IsVar == false
                && variableDeclaration.Variables.Count == 1)
            {
                ExpressionSyntax initializerValue = variableDeclaration.Variables[0].Initializer?.Value;

                if (initializerValue != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ITypeSymbol initializerTypeSymbol = semanticModel.GetTypeInfo(initializerValue).Type;

                    if (initializerTypeSymbol?.IsErrorType() == false)
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(variableDeclaration.Type).ConvertedType;

                        if (!initializerTypeSymbol.Equals(typeSymbol))
                        {
                            ChangeType(context, variableDeclaration, initializerTypeSymbol, semanticModel, context.CancellationToken);
                        }
                    }
                }
            }
        }

        private static async Task ChangeTypeAsync(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            TypeAnalysisResult result = VariableDeclarationAnalysis.AnalyzeType(
                variableDeclaration,
                semanticModel,
                context.CancellationToken);

            if (result == TypeAnalysisResult.Explicit || result == TypeAnalysisResult.ExplicitButShouldBeImplicit)
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeExplicitTypeToVar))
                {
                    context.RegisterRefactoring(
                        "Change type to 'var'",
                        cancellationToken =>
                        {
                            return ChangeTypeRefactoring.ChangeTypeToVarAsync(
                                context.Document,
                                variableDeclaration.Type,
                                cancellationToken);
                        });
                }
            }
            else if (result == TypeAnalysisResult.Implicit || result == TypeAnalysisResult.ImplicitButShouldBeExplicit)
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeVarToExplicitType))
                {
                    ITypeSymbol typeSymbol = semanticModel
                        .GetTypeInfo(variableDeclaration.Type, context.CancellationToken)
                        .Type;

                    ChangeType(context, variableDeclaration, typeSymbol, semanticModel, context.CancellationToken);
                }
            }
        }

        private static void ChangeType(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            TypeSyntax type = variableDeclaration.Type;

            if (variableDeclaration.Variables.Count == 1
                && variableDeclaration.Variables[0].Initializer?.Value != null
                && typeSymbol.IsNamedType())
            {
                INamedTypeSymbol taskOfT = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");

                if (((INamedTypeSymbol)typeSymbol).ConstructedFrom.Equals(taskOfT)
                    && AsyncAnalysis.IsPartOfAsyncBlock(variableDeclaration, semanticModel, cancellationToken))
                {
                    ITypeSymbol typeArgumentType = ((INamedTypeSymbol)typeSymbol).TypeArguments[0];

                    context.RegisterRefactoring(
                        $"Change type to '{typeArgumentType.ToDisplayString(SyntaxUtility.DefaultSymbolDisplayFormat)}' and insert 'await'",
                        c =>
                        {
                            return ChangeTypeAndAddAwaitAsync(
                                context.Document,
                                variableDeclaration,
                                typeArgumentType,
                                c);
                        });
                }
            }

            context.RegisterRefactoring(
                $"Change type to '{typeSymbol.ToDisplayString(SyntaxUtility.DefaultSymbolDisplayFormat)}'",
                c =>
                {
                    return ChangeTypeRefactoring.ChangeTypeAsync(
                        context.Document,
                        type,
                        typeSymbol,
                        c);
                });
        }

        private static async Task<Document> ChangeTypeAndAddAwaitAsync(
            Document document,
            VariableDeclarationSyntax variableDeclaration,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax initializerValue = variableDeclaration.Variables[0].Initializer.Value;

            AwaitExpressionSyntax newInitializerValue = SyntaxFactory.AwaitExpression(initializerValue)
                .WithTriviaFrom(initializerValue);

            VariableDeclarationSyntax newNode = variableDeclaration.ReplaceNode(initializerValue, newInitializerValue);

            newNode = newNode
                .WithType(
                    CSharpFactory.Type(typeSymbol)
                        .WithTriviaFrom(variableDeclaration.Type)
                        .WithSimplifierAnnotation());

            SyntaxNode newRoot = oldRoot.ReplaceNode(variableDeclaration, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}