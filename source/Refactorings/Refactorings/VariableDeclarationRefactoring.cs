// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using Roslynator.Rename;

namespace Roslynator.CSharp.Refactorings
{
    internal static class VariableDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, VariableDeclarationSyntax variableDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameIdentifierAccordingToTypeName))
                await RenameVariableAccordingToTypeNameAsync(context, variableDeclaration).ConfigureAwait(false);

            await ChangeVariableDeclarationTypeRefactoring.ComputeRefactoringsAsync(context, variableDeclaration).ConfigureAwait(false);

            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.CallToMethod))
                await AddCastExpressionAsync(context, variableDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CheckExpressionForNull))
                await CheckExpressionForNullRefactoring.ComputeRefactoringAsync(context, variableDeclaration).ConfigureAwait(false);

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
            TypeSyntax type = variableDeclaration.Type;

            if (type != null
                && !variableDeclaration.IsParentKind(SyntaxKind.EventFieldDeclaration))
            {
                VariableDeclaratorSyntax variable = variableDeclaration.SingleVariableOrDefault();

                if (variable != null)
                {
                    SyntaxToken identifier = variable.Identifier;

                    if (identifier.Span.Contains(context.Span))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ISymbol symbol = semanticModel.GetDeclaredSymbol(variable, context.CancellationToken);

                        if (symbol != null)
                        {
                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                            if (typeSymbol?.IsErrorType() == false)
                            {
                                string newName = Identifier.CreateName(
                                    typeSymbol,
                                    FirstCharToLower(symbol));

                                if (!string.IsNullOrEmpty(newName))
                                {
                                    if (context.Settings.PrefixFieldIdentifierWithUnderscore
                                        && symbol.IsPrivate()
                                        && symbol.IsField()
                                        && !((IFieldSymbol)symbol).IsConst)
                                    {
                                        newName = Identifier.ToCamelCase(newName, prefixWithUnderscore: true);
                                    }

                                    string oldName = identifier.ValueText;

                                    if (!string.Equals(oldName, newName, StringComparison.Ordinal))
                                    {
                                        newName = Identifier.EnsureUniqueLocalName(newName, variable.SpanStart, semanticModel, context.CancellationToken);

                                        context.RegisterRefactoring(
                                            $"Rename '{oldName}' to '{newName}'",
                                            cancellationToken => Renamer.RenameSymbolAsync(context.Document, symbol, newName, cancellationToken));
                                    }
                                }
                            }
                        }
                    }
                }
            }
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

                    ITypeSymbol declarationType = semanticModel.GetTypeSymbol(variableDeclaration.Type, context.CancellationToken);

                    if (declarationType?.IsErrorType() == false)
                    {
                        ITypeSymbol expressionType = semanticModel.GetTypeSymbol(declarator.Initializer.Value, context.CancellationToken);

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