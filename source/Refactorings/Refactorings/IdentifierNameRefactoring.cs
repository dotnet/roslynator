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
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameBackingFieldAccordingToPropertyName))
                await RenameFieldAccordingToPropertyNameAsync(context, identifierName).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddUsingDirective)
                && context.Span.IsEmpty)
            {
                await AddUsingDirectiveRefactoring.ComputeRefactoringsAsync(context, identifierName).ConfigureAwait(false);
            }
        }

        private static async Task RenameFieldAccordingToPropertyNameAsync(
            RefactoringContext context,
            IdentifierNameSyntax identifierName)
        {
            if (!IsQualified(identifierName)
                || IsQualifiedWithThis(identifierName))
            {
                PropertyDeclarationSyntax propertyDeclaration = identifierName.FirstAncestor<PropertyDeclarationSyntax>();

                if (propertyDeclaration != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    var fieldSymbol = semanticModel.GetSymbol(identifierName, context.CancellationToken) as IFieldSymbol;

                    if (fieldSymbol?.IsPrivate() == true)
                    {
                        IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

                        if (propertySymbol != null
                            && fieldSymbol.IsStatic == propertySymbol.IsStatic
                            && fieldSymbol.ContainingType == propertySymbol.ContainingType)
                        {
                            string newName = IdentifierUtility.ToCamelCase(propertySymbol.Name, context.Settings.PrefixFieldIdentifierWithUnderscore);

                            if (!string.Equals(fieldSymbol.Name, newName, StringComparison.Ordinal))
                            {
                                bool isUnique = await NameGenerator.IsUniqueMemberNameAsync(
                                    fieldSymbol,
                                    newName,
                                    context.Solution,
                                    context.CancellationToken).ConfigureAwait(false);

                                if (isUnique)
                                {
                                    context.RegisterRefactoring(
                                        $"Rename field to '{newName}'",
                                        cancellationToken => SymbolRenamer.RenameSymbolAsync(context.Document, fieldSymbol, newName, cancellationToken));
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsQualified(SimpleNameSyntax identifierName)
        {
            return identifierName.IsParentKind(SyntaxKind.SimpleMemberAccessExpression);
        }

        private static bool IsQualifiedWithThis(SimpleNameSyntax identifierName)
        {
            if (IsQualified(identifierName))
            {
                var memberAccess = (MemberAccessExpressionSyntax)identifierName.Parent;

                return memberAccess.Expression?.IsKind(SyntaxKind.ThisExpression) == true;
            }

            return false;
        }
    }
}
