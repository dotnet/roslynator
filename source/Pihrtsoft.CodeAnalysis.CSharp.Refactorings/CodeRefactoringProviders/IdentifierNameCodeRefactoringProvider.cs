// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(IdentifierNameCodeRefactoringProvider))]
    public class IdentifierNameCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            IdentifierNameSyntax identifierName = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<IdentifierNameSyntax>();

            if (identifierName == null)
                return;

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                RenameFieldAccordingToPropertyName(context, identifierName, semanticModel);
            }
        }

        private static void RenameFieldAccordingToPropertyName(
            CodeRefactoringContext context,
            IdentifierNameSyntax identifierName,
            SemanticModel semanticModel)
        {
            if (!identifierName.IsQualified()
                || identifierName.IsQualifiedWithThis())
            {
                PropertyDeclarationSyntax propertyDeclaration = identifierName.FirstAncestor<PropertyDeclarationSyntax>();

                if (propertyDeclaration != null)
                {
                    var fieldSymbol = semanticModel
                        .GetSymbolInfo(identifierName, context.CancellationToken)
                        .Symbol as IFieldSymbol;

                    if (fieldSymbol?.DeclaredAccessibility == Accessibility.Private)
                    {
                        IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

                        if (propertySymbol != null
                            && fieldSymbol.IsStatic == propertySymbol.IsStatic
                            && object.Equals(fieldSymbol.ContainingType, propertySymbol.ContainingType))
                        {
                            string newName = NamingHelper.ToCamelCaseWithUnderscore(propertySymbol.Name);

                            if (!string.Equals(newName, propertySymbol.Name, StringComparison.Ordinal))
                            {
                                context.RegisterRefactoring(
                                    $"Rename field to '{newName}'",
                                    cancellationToken =>
                                    {
                                        return fieldSymbol.RenameAsync(
                                            newName,
                                            context.Document,
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
