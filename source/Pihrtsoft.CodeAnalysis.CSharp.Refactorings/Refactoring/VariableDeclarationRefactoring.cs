// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class VariableDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, VariableDeclarationSyntax variableDeclaration)
        {
            if (context.SupportsSemanticModel)
            {
                await RenameVariableAccordingToTypeNameAsync(context, variableDeclaration);

                await ChangeTypeAccordingToExpressionAsync(context, variableDeclaration);

                if (variableDeclaration.Type?.Span.Contains(context.Span) == true)
                    await ChangeTypeAsync(context, variableDeclaration);

                await AddCastToExpressionAsync(context, variableDeclaration);
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

            switch (result)
            {
                case TypeAnalysisResult.Explicit:
                case TypeAnalysisResult.ExplicitButShouldBeImplicit:
                    {
                        context.RegisterRefactoring(
                            "Change type to 'var'",
                            cancellationToken => TypeSyntaxRefactoring.ChangeTypeToImplicitAsync(context.Document, variableDeclaration.Type, cancellationToken));

                        break;
                    }
                case TypeAnalysisResult.Implicit:
                case TypeAnalysisResult.ImplicitButShouldBeExplicit:
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(variableDeclaration.Type, context.CancellationToken).Type;

                        context.RegisterRefactoring(
                            $"Change type to '{typeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                            cancellationToken => TypeSyntaxRefactoring.ChangeTypeToExplicitAsync(context.Document, variableDeclaration.Type, typeSymbol, cancellationToken));

                        break;
                    }
            }
        }

        private static async Task RenameVariableAccordingToTypeNameAsync(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration)
        {
            if (variableDeclaration.Type == null)
                return;

            if (variableDeclaration.Parent?.IsKind(SyntaxKind.EventFieldDeclaration) == true)
                return;

            if (variableDeclaration.Variables.Count != 1)
                return;

            VariableDeclaratorSyntax declarator = variableDeclaration.Variables[0];

            if (!declarator.Identifier.Span.Contains(context.Span))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync();

            ISymbol symbol = semanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);

            if (symbol == null)
                return;

            string newName = NamingHelper.CreateIdentifierName(
                variableDeclaration.Type,
                semanticModel,
                FirstCharToLower(symbol));

            if (string.IsNullOrEmpty(newName))
                return;

            if (symbol.IsKind(SymbolKind.Field)
                && symbol.DeclaredAccessibility == Accessibility.Private
                && !((IFieldSymbol)symbol).IsConst)
            {
                newName = NamingHelper.ToCamelCaseWithUnderscore(newName);
            }

            if (string.Equals(declarator.Identifier.ValueText, newName, StringComparison.Ordinal))
                return;

            context.RegisterRefactoring(
                $"Rename {GetName(symbol)} to '{newName}'",
                cancellationToken => symbol.RenameAsync(newName, context.Document, cancellationToken));
        }

        private static string GetName(ISymbol symbol)
        {
            if (symbol.IsKind(SymbolKind.Field))
            {
                if (((IFieldSymbol)symbol).IsConst)
                    return "constant";
                else
                    return "field";
            }

            return "local";
        }

        private static bool FirstCharToLower(ISymbol symbol)
        {
            if (symbol.IsKind(SymbolKind.Field))
            {
                if (((IFieldSymbol)symbol).IsConst)
                    return false;

                if (symbol.DeclaredAccessibility != Accessibility.Private)
                    return false;
            }

            return true;
        }

        private static async Task ChangeTypeAccordingToExpressionAsync(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration)
        {
            if (variableDeclaration.Parent?.IsKind(SyntaxKind.FieldDeclaration) != false)
                return;

            TypeSyntax type = variableDeclaration.Type;

            if (type == null)
                return;

            if (type.IsVar)
                return;

            if (!type.Span.Contains(context.Span))
                return;

            if (variableDeclaration.Variables.Count != 1)
                return;

            EqualsValueClauseSyntax initializer = variableDeclaration.Variables[0].Initializer;

            if (initializer == null)
                return;

            if (initializer.Value == null)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync();

            ITypeSymbol initializerTypeSymbol = semanticModel.GetTypeInfo(initializer.Value).Type;

            if (initializerTypeSymbol == null || initializerTypeSymbol.IsKind(SymbolKind.ErrorType))
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(type).ConvertedType;

            if (!initializerTypeSymbol.Equals(typeSymbol))
            {
                context.RegisterRefactoring(
                    $"Change type to '{initializerTypeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                    cancellationToken => TypeSyntaxRefactoring.ChangeTypeToExplicitAsync(context.Document, variableDeclaration.Type, initializerTypeSymbol, cancellationToken));
            }
        }

        private static async Task AddCastToExpressionAsync(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration)
        {
            if (variableDeclaration.Type?.IsVar == false)
            {
                VariableDeclaratorSyntax declarator = variableDeclaration.Variables
                    .FirstOrDefault(f => f.Initializer?.Value?.Span.Contains(context.Span) == true);

                if (declarator != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync();

                    ITypeSymbol declarationType = semanticModel
                        .GetTypeInfo(variableDeclaration.Type, context.CancellationToken)
                        .Type;

                    if (declarationType != null)
                    {
                        ITypeSymbol expressionType = semanticModel
                            .GetTypeInfo(declarator.Initializer.Value, context.CancellationToken)
                            .Type;

                        if (!declarationType.Equals(expressionType))
                            AddCastRefactoring.Refactor(context, declarator.Initializer.Value, declarationType);
                    }
                }
            }
        }
    }
}