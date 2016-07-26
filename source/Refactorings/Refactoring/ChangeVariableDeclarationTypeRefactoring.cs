// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ChangeVariableDeclarationTypeRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, VariableDeclarationSyntax variableDeclaration)
        {
            if (context.SupportsSemanticModel
                && variableDeclaration.Type?.Span.Contains(context.Span) == true)
            {
                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ChangeTypeAccordingToExpression))
                    await ChangeTypeAccordingToExpressionAsync(context, variableDeclaration);

                if (context.Settings.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ReplaceExplicitTypeWithVar,
                    RefactoringIdentifiers.ReplaceVarWithExplicitType))
                {
                    await ChangeTypeAsync(context, variableDeclaration);
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
                    SemanticModel semanticModel = await context.GetSemanticModelAsync();

                    ITypeSymbol initializerTypeSymbol = semanticModel.GetTypeInfo(initializerValue).Type;

                    if (initializerTypeSymbol?.IsErrorType() == false)
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(variableDeclaration.Type).ConvertedType;

                        if (!initializerTypeSymbol.Equals(typeSymbol))
                        {
                            ChangeType(context, variableDeclaration, initializerTypeSymbol, semanticModel);
                        }
                    }
                }
            }
        }

        private static async Task ChangeTypeAsync(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync();

            TypeAnalysisResult result = VariableDeclarationAnalysis.AnalyzeType(
                variableDeclaration,
                semanticModel,
                context.CancellationToken);

            if (result == TypeAnalysisResult.Explicit || result == TypeAnalysisResult.ExplicitButShouldBeImplicit)
            {
                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceExplicitTypeWithVar))
                {
                    context.RegisterRefactoring(
                        $"Replace '{variableDeclaration.Type}' with 'var'",
                        cancellationToken =>
                        {
                            return TypeSyntaxRefactoring.ChangeTypeToVarAsync(
                                context.Document,
                                variableDeclaration.Type,
                                cancellationToken);
                        });
                }
            }
            else if (result == TypeAnalysisResult.Implicit || result == TypeAnalysisResult.ImplicitButShouldBeExplicit)
            {
                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceVarWithExplicitType))
                {
                    ITypeSymbol typeSymbol = semanticModel
                        .GetTypeInfo(variableDeclaration.Type, context.CancellationToken)
                        .Type;

                    ChangeType(context, variableDeclaration, typeSymbol, semanticModel);
                }
            }
        }

        private static void ChangeType(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration,
            ITypeSymbol type,
            SemanticModel semanticModel)
        {
            if (variableDeclaration.Variables.Count == 1
                && variableDeclaration.Variables[0].Initializer?.Value != null
                && type.IsNamedType())
            {
                INamedTypeSymbol taskOfT = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");

                if (((INamedTypeSymbol)type).ConstructedFrom.Equals(taskOfT))
                {
                    ITypeSymbol typeArgumentType = ((INamedTypeSymbol)type).TypeArguments[0];

                    context.RegisterRefactoring(
                        $"Replace '{variableDeclaration.Type}' with '{typeArgumentType.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                        cancellationToken =>
                        {
                            return ChangeTypeAndAddAwaitAsync(
                                context.Document,
                                variableDeclaration,
                                typeArgumentType,
                                cancellationToken);
                        });
                }
            }

            context.RegisterRefactoring(
                $"Replace '{variableDeclaration.Type}' with '{type.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                cancellationToken =>
                {
                    return TypeSyntaxRefactoring.ChangeTypeAsync(
                        context.Document,
                        variableDeclaration.Type,
                        type,
                        cancellationToken);
                });
        }

        private static async Task<Document> ChangeTypeAndAddAwaitAsync(
            Document document,
            VariableDeclarationSyntax variableDeclaration,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ExpressionSyntax initializerValue = variableDeclaration.Variables[0].Initializer.Value;

            AwaitExpressionSyntax newInitializerValue = SyntaxFactory.AwaitExpression(initializerValue)
                .WithTriviaFrom(initializerValue);

            VariableDeclarationSyntax newNode = variableDeclaration.ReplaceNode(initializerValue, newInitializerValue);

            newNode = newNode
                .WithType(
                    TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol)
                        .WithTriviaFrom(variableDeclaration.Type)
                        .WithSimplifierAnnotation());

            SyntaxNode newRoot = oldRoot.ReplaceNode(variableDeclaration, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}