// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class PropertyDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (MarkMemberAsStaticRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark property as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)propertyDeclaration.Parent);
            }

            if (propertyDeclaration.HeaderSpan().Contains(context.Span)
                && ConvertPropertyToMethodRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Convert to method",
                    cancellationToken => ConvertPropertyToMethodRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (propertyDeclaration.AccessorList?.Span.Contains(context.Span) == true
                && context.SupportsCSharp6
                && UseExpressionBodiedMemberRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (RemovePropertyInitializerRefactoring.CanRefactor(context, propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Remove property initializer",
                    cancellationToken => RemovePropertyInitializerRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.SupportsSemanticModel)
            {
                if (propertyDeclaration.Span.Contains(context.Span)
                    && ExpandPropertyRefactoring.CanRefactor(propertyDeclaration))
                {
                    context.RegisterRefactoring(
                        "Expand property",
                        cancellationToken => ExpandPropertyRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));

                    context.RegisterRefactoring(
                        "Expand property and add backing field",
                        cancellationToken => ExpandPropertyAndAddBackingFieldRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
                }

                await NotifyPropertyChangedRefactoring.RefactorAsync(context, propertyDeclaration);
            }

            if (propertyDeclaration.HeaderSpan().Contains(context.Span)
                && MakeMemberAbstractRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Make property abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.SupportsSemanticModel
                && propertyDeclaration.Type != null
                && propertyDeclaration.Identifier.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                string newName = NamingHelper.CreateIdentifierName(propertyDeclaration.Type, semanticModel);

                if (!string.IsNullOrEmpty(newName)
                    && !string.Equals(newName, propertyDeclaration.Identifier.ValueText, StringComparison.Ordinal))
                {
                    ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

                    context.RegisterRefactoring(
                        $"Rename property to '{newName}'",
                        cancellationToken => symbol.RenameAsync(newName, context.Document, cancellationToken));
                }
            }
        }
    }
}
