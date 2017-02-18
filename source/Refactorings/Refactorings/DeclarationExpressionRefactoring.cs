// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using Roslynator.Rename;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DeclarationExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, DeclarationExpressionSyntax declarationExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameIdentifierAccordingToTypeName))
                await RenameVariableAccordingToTypeNameAsync(context, declarationExpression).ConfigureAwait(false);

            await ChangeDeclarationExpressionTypeRefactoring.ComputeRefactoringsAsync(context, declarationExpression).ConfigureAwait(false);
        }

        private static async Task RenameVariableAccordingToTypeNameAsync(
            RefactoringContext context,
            DeclarationExpressionSyntax declarationExpression)
        {
            TypeSyntax type = declarationExpression.Type;

            if (type != null)
            {
                VariableDesignationSyntax designation = declarationExpression.Designation;

                if (designation?.IsKind(SyntaxKind.SingleVariableDesignation) == true)
                {
                    var singleVariableDesignation = (SingleVariableDesignationSyntax)designation;

                    SyntaxToken identifier = singleVariableDesignation.Identifier;

                    if (identifier.Span.Contains(context.Span))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        var localSymbol = semanticModel.GetDeclaredSymbol(singleVariableDesignation, context.CancellationToken) as ILocalSymbol;

                        if (localSymbol?.IsErrorType() == false)
                        {
                            ITypeSymbol typeSymbol = localSymbol.Type;

                            if (typeSymbol != null)
                            {
                                string newName = Identifier.CreateName(typeSymbol, firstCharToLower: true);

                                if (!string.IsNullOrEmpty(newName)
                                    && !string.Equals(identifier.ValueText, newName, StringComparison.Ordinal))
                                {
                                    newName = Identifier.EnsureUniqueLocalName(newName, singleVariableDesignation, semanticModel, context.CancellationToken);

                                    context.RegisterRefactoring(
                                        $"Rename local to '{newName}'",
                                        cancellationToken => Renamer.RenameSymbolAsync(context.Document, localSymbol, newName, cancellationToken));
                                }
                            }
                        }
                    }
                }
            }
        }
   }
}