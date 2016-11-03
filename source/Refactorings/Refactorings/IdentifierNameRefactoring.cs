// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IdentifierNameRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, IdentifierNameSyntax identifierName)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameBackingFieldAccordingToPropertyName)
                && context.SupportsSemanticModel)
            {
                await RenameFieldAccordingToPropertyNameAsync(context, identifierName).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddUsingDirective)
                && context.Span.IsEmpty
                && context.SupportsSemanticModel)
            {
                await AddUsingDirectiveRefactoring.ComputeRefactoringsAsync(context, identifierName).ConfigureAwait(false);
            }
        }

        private static async Task RenameFieldAccordingToPropertyNameAsync(
            RefactoringContext context,
            IdentifierNameSyntax identifierName)
        {
            if (!identifierName.IsQualified()
                || identifierName.IsQualifiedWithThis())
            {
                PropertyDeclarationSyntax propertyDeclaration = identifierName.FirstAncestor<PropertyDeclarationSyntax>();

                if (propertyDeclaration != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    var fieldSymbol = semanticModel
                        .GetSymbolInfo(identifierName, context.CancellationToken)
                        .Symbol as IFieldSymbol;

                    if (fieldSymbol?.IsPrivate() == true)
                    {
                        IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

                        if (propertySymbol != null
                            && fieldSymbol.IsStatic == propertySymbol.IsStatic
                            && object.Equals(fieldSymbol.ContainingType, propertySymbol.ContainingType))
                        {
                            string newName = TextUtility.ToCamelCase(propertySymbol.Name, context.Settings.PrefixFieldIdentifierWithUnderscore);

                            if (!string.Equals(newName, fieldSymbol.Name, StringComparison.Ordinal))
                            {
                                string fieldName = identifierName.Identifier.ValueText;

                                context.RegisterRefactoring(
                                    $"Rename field to '{newName}'",
                                    cancellationToken =>
                                    {
                                        return SymbolRenamer.RenameAsync(
                                            context.Document,
                                            fieldSymbol,
                                            newName,
                                            cancellationToken);
                                    });
                            }
                        }
                    }
                }
            }
        }
    }
}
