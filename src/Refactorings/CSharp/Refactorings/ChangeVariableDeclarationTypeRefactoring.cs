// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            if (variableDeclaration.Type?.Span.Contains(context.Span) == true
                && context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ChangeExplicitTypeToVar,
                    RefactoringIdentifiers.ChangeVarToExplicitType))
            {
                await ChangeTypeAsync(context, variableDeclaration).ConfigureAwait(false);
            }
        }

        private static async Task ChangeTypeAsync(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            TypeAnalysis analysis = TypeAnalysis.AnalyzeType(variableDeclaration, semanticModel, context.CancellationToken);

            if (analysis.IsExplicit)
            {
                if (analysis.SupportsImplicit
                    && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeExplicitTypeToVar))
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
            else if (analysis.SupportsExplicit
                && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeVarToExplicitType))
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(variableDeclaration.Type, context.CancellationToken);

                ChangeType(context, variableDeclaration, typeSymbol, semanticModel, context.CancellationToken);
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

            if (variableDeclaration.Variables.SingleOrDefault(shouldThrow: false)?.Initializer?.Value != null
                && typeSymbol.OriginalDefinition.EqualsOrInheritsFromTaskOfT(semanticModel))
            {
                ISymbol enclosingSymbol = semanticModel.GetEnclosingSymbol(variableDeclaration.SpanStart, cancellationToken);

                if (enclosingSymbol.IsAsyncMethod())
                {
                    ITypeSymbol typeArgument = ((INamedTypeSymbol)typeSymbol).TypeArguments[0];

                    context.RegisterRefactoring(
                        $"Change type to '{SymbolDisplay.ToMinimalDisplayString(typeArgument, semanticModel, type.SpanStart, SymbolDisplayFormats.Default)}' and insert 'await'",
                        c => ChangeTypeAndAddAwaitAsync(context.Document, variableDeclaration, typeArgument, c));
                }
            }

            context.RegisterRefactoring(
                $"Change type to '{SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, type.SpanStart, SymbolDisplayFormats.Default)}'",
                c => ChangeTypeRefactoring.ChangeTypeAsync(context.Document, type, typeSymbol, c));
        }

        private static async Task<Document> ChangeTypeAndAddAwaitAsync(
            Document document,
            VariableDeclarationSyntax variableDeclaration,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax type = variableDeclaration.Type;

            ExpressionSyntax value = variableDeclaration.Variables[0].Initializer.Value;

            AwaitExpressionSyntax newInitializerValue = SyntaxFactory.AwaitExpression(value)
                .WithTriviaFrom(value);

            VariableDeclarationSyntax newNode = variableDeclaration.ReplaceNode(value, newInitializerValue);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            newNode = newNode.WithType(
                typeSymbol.ToMinimalTypeSyntax(semanticModel, type.SpanStart).WithTriviaFrom(type));

            return await document.ReplaceNodeAsync(variableDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}