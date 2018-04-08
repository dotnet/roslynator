// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;
using Roslynator.CSharp.Refactorings.InlineDefinition;

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

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InlineProperty))
                await InlinePropertyRefactoring.ComputeRefactoringsAsync(context, identifierName).ConfigureAwait(false);
        }

        private static async Task RenameFieldAccordingToPropertyNameAsync(
            RefactoringContext context,
            IdentifierNameSyntax identifierName)
        {
            if (IsQualified(identifierName)
                && !IsQualifiedWithThis(identifierName))
            {
                return;
            }

            PropertyDeclarationSyntax propertyDeclaration = identifierName.FirstAncestor<PropertyDeclarationSyntax>();

            if (propertyDeclaration == null)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var fieldSymbol = semanticModel.GetSymbol(identifierName, context.CancellationToken) as IFieldSymbol;

            if (fieldSymbol?.DeclaredAccessibility != Accessibility.Private)
                return;

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

            if (propertySymbol == null)
                return;

            if (fieldSymbol.IsStatic != propertySymbol.IsStatic)
                return;

            if (fieldSymbol.ContainingType != propertySymbol.ContainingType)
                return;

            string newName = StringUtility.ToCamelCase(propertySymbol.Name, context.Settings.PrefixFieldIdentifierWithUnderscore);

            if (string.Equals(fieldSymbol.Name, newName, StringComparison.Ordinal))
                return;

            if (!await MemberNameGenerator.IsUniqueMemberNameAsync(
                newName,
                fieldSymbol,
                context.Solution,
                cancellationToken: context.CancellationToken).ConfigureAwait(false))
            {
                return;
            }

            context.RegisterRefactoring(
                $"Rename '{fieldSymbol.Name}' to '{newName}'",
                cancellationToken => Renamer.RenameSymbolAsync(context.Solution, fieldSymbol, newName, default(OptionSet), cancellationToken));
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

                return memberAccess.Expression?.Kind() == SyntaxKind.ThisExpression;
            }

            return false;
        }
    }
}
