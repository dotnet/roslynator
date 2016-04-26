// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(RenameFieldAccordingToPropertyNameCodeRefactoringProvider))]
    public class RenameFieldAccordingToPropertyNameCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            IdentifierNameSyntax identifierName = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<IdentifierNameSyntax>();

            if (identifierName == null)
                return;

            PropertyDeclarationSyntax propertyDeclaration = identifierName.FirstAncestor<PropertyDeclarationSyntax>();
            if (propertyDeclaration == null)
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
            if (semanticModel == null)
                return;

            ISymbol symbol = semanticModel.GetSymbolInfo(identifierName, context.CancellationToken).Symbol;
            if (symbol?.Kind != SymbolKind.Field)
                return;

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);
            if (propertySymbol == null)
                return;

            string newName = NamingHelper.ToCamelCaseWithUnderscore(propertySymbol.Name);
            if (string.Equals(newName, symbol.Name, StringComparison.Ordinal))
                return;

            context.RegisterRefactoring(
                $"Rename field to '{newName}'",
                cancellationToken => symbol.RenameAsync(newName, context.Document, cancellationToken));
        }
    }
}
