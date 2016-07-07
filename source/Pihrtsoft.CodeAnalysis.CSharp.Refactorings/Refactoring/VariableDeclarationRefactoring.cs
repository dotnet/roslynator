// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class VariableDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, VariableDeclarationSyntax variableDeclaration)
        {
            if (context.SupportsSemanticModel)
            {
                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RenameIdentifierAccordingToTypeName))
                    await RenameVariableAccordingToTypeNameAsync(context, variableDeclaration);

                await ChangeVariableDeclarationTypeRefactoring.ComputeRefactoringsAsync(context, variableDeclaration);

                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddCastExpression))
                    await AddCastExpressionAsync(context, variableDeclaration);
            }
        }

        private static async Task RenameVariableAccordingToTypeNameAsync(
            RefactoringContext context,
            VariableDeclarationSyntax variableDeclaration)
        {
            if (variableDeclaration.Type != null
                && variableDeclaration.Parent?.IsKind(SyntaxKind.EventFieldDeclaration) == false
                && variableDeclaration.Variables.Count == 1
                && variableDeclaration.Variables[0].Identifier.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                ISymbol symbol = semanticModel.GetDeclaredSymbol(variableDeclaration.Variables[0], context.CancellationToken);

                if (symbol != null)
                {
                    string newName = IdentifierHelper.CreateIdentifierName(
                        variableDeclaration.Type,
                        semanticModel,
                        FirstCharToLower(symbol));

                    if (!string.IsNullOrEmpty(newName))
                    {
                        if (context.Settings.PrefixFieldIdentifierWithUnderscore
                            && symbol.IsKind(SymbolKind.Field)
                            && symbol.DeclaredAccessibility == Accessibility.Private
                            && !((IFieldSymbol)symbol).IsConst)
                        {
                            newName = IdentifierHelper.ToCamelCaseWithUnderscore(newName);
                        }

                        if (!string.Equals(variableDeclaration.Variables[0].Identifier.ValueText, newName, StringComparison.Ordinal))
                        {
                            context.RegisterRefactoring(
                                $"Rename {GetName(symbol)} to '{newName}'",
                                cancellationToken => symbol.RenameAsync(newName, context.Document, cancellationToken));
                        }
                    }
                }
            }
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

        private static async Task AddCastExpressionAsync(
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
                            AddCastExpressionRefactoring.RegisterRefactoring(context, declarator.Initializer.Value, declarationType);
                    }
                }
            }
        }
    }
}