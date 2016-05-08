// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(PropertyDeclarationCodeRefactoringProvider))]
    public class PropertyDeclarationCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            PropertyDeclarationSyntax propertyDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<PropertyDeclarationSyntax>();

            if (propertyDeclaration == null)
                return;

            MemberDeclarationRefactoring.Remove(context, propertyDeclaration);
            MemberDeclarationRefactoring.Duplicate(context, propertyDeclaration);

            if (PropertyDeclarationRefactoring.CanConvertToMethod(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Convert to method",
                    cancellationToken => PropertyDeclarationRefactoring.ConvertToMethodAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            SemanticModel semanticModel = null;

            if (context.Document.SupportsSemanticModel)
            {
                if (semanticModel == null)
                    semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                if (PropertyDeclarationRefactoring.CanExpand(propertyDeclaration, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Expand property",
                        cancellationToken => PropertyDeclarationRefactoring.ExpandPropertyAsync(context.Document, propertyDeclaration, cancellationToken));

                    context.RegisterRefactoring(
                        "Expand property and add backing field",
                        cancellationToken => PropertyDeclarationRefactoring.ExpandPropertyAndAddBackingFieldAsync(context.Document, propertyDeclaration, cancellationToken));
                }
            }

            if (MakeMemberAbstractRefactoring.CanRefactor(context, propertyDeclaration))
            {
                context.RegisterRefactoring(
                    $"Make property abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.Document.SupportsSemanticModel)
            {
                if (semanticModel == null)
                    semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                RenamePropertyAccordingToTypeName(context, semanticModel, propertyDeclaration);
            }

            if (propertyDeclaration.Initializer != null)
            {
                context.RegisterRefactoring(
                    "Remove initializer",
                    cancellationToken => PropertyDeclarationRefactoring.RemoveInitializerAsync(context.Document, propertyDeclaration, cancellationToken));
            }
        }

        private static void RenamePropertyAccordingToTypeName(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.Type == null)
                return;

            if (!propertyDeclaration.Identifier.Span.Contains(context.Span))
                return;

            string newName = NamingHelper.CreateIdentifierName(propertyDeclaration.Type, semanticModel);

            if (string.Equals(newName, propertyDeclaration.Identifier.ToString(), StringComparison.Ordinal))
                return;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

            context.RegisterRefactoring(
                $"Rename property to '{newName}'",
                cancellationToken => symbol.RenameAsync(newName, context.Document, cancellationToken));
        }
    }
}