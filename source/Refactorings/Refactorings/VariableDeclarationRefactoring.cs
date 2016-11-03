// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class VariableDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, VariableDeclarationSyntax variableDeclaration)
        {
            if (context.SupportsSemanticModel)
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameIdentifierAccordingToTypeName))
                    await RenameVariableAccordingToTypeNameAsync(context, variableDeclaration).ConfigureAwait(false);

                await ChangeVariableDeclarationTypeRefactoring.ComputeRefactoringsAsync(context, variableDeclaration).ConfigureAwait(false);

                if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.AddToMethodInvocation))
                    await AddCastExpressionAsync(context, variableDeclaration).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.SplitVariableDeclaration)
                && SplitVariableDeclarationRefactoring.CanRefactor(variableDeclaration))
            {
                context.RegisterRefactoring(
                    SplitVariableDeclarationRefactoring.GetTitle(variableDeclaration),
                    cancellationToken => SplitVariableDeclarationRefactoring.RefactorAsync(context.Document, variableDeclaration, cancellationToken));
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
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ISymbol symbol = semanticModel.GetDeclaredSymbol(variableDeclaration.Variables[0], context.CancellationToken);

                if (symbol != null)
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(variableDeclaration.Type, context.CancellationToken).Type;

                    if (typeSymbol?.IsErrorType() == false)
                    {
                        string newName = SyntaxUtility.CreateIdentifier(
                            typeSymbol,
                            FirstCharToLower(symbol));

                        if (!string.IsNullOrEmpty(newName))
                        {
                            if (context.Settings.PrefixFieldIdentifierWithUnderscore
                                && symbol.IsField()
                                && symbol.IsPrivate()
                                && !((IFieldSymbol)symbol).IsConst)
                            {
                                newName = TextUtility.ToCamelCaseWithUnderscore(newName);
                            }

                            if (!string.Equals(variableDeclaration.Variables[0].Identifier.ValueText, newName, StringComparison.Ordinal))
                            {
                                context.RegisterRefactoring(
                                    $"Rename {GetName(symbol)} to '{newName}'",
                                    cancellationToken => SymbolRenamer.RenameAsync(context.Document, symbol, newName, cancellationToken));
                            }
                        }
                    }
                }
            }
        }

        private static string GetName(ISymbol symbol)
        {
            if (symbol.IsField())
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
            if (symbol.IsField())
            {
                if (((IFieldSymbol)symbol).IsConst)
                    return false;

                if (!symbol.IsPrivate())
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
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ITypeSymbol declarationType = semanticModel
                        .GetTypeInfo(variableDeclaration.Type, context.CancellationToken)
                        .Type;

                    if (declarationType?.IsErrorType() == false)
                    {
                        ITypeSymbol expressionType = semanticModel
                            .GetTypeInfo(declarator.Initializer.Value, context.CancellationToken)
                            .Type;

                        if (expressionType?.IsErrorType() == false
                            && !declarationType.Equals(expressionType))
                        {
                            ModifyExpressionRefactoring.ComputeRefactoring(context, declarator.Initializer.Value, declarationType, semanticModel);
                        }
                    }
                }
            }
        }
    }
}