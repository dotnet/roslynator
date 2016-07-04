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
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.MarkMemberAsStatic)
                && MarkMemberAsStaticRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark property as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)propertyDeclaration.Parent);
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplacePropertyWithMethod)
                && propertyDeclaration.HeaderSpan().Contains(context.Span)
                && ReplacePropertyWithMethodRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Replace property with method",
                    cancellationToken => ReplacePropertyWithMethodRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && propertyDeclaration.AccessorList?.Span.Contains(context.Span) == true
                && context.SupportsCSharp6
                && UseExpressionBodiedMemberRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemovePropertyInitializer)
                && RemovePropertyInitializerRefactoring.CanRefactor(context, propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Remove property initializer",
                    cancellationToken => RemovePropertyInitializerRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.SupportsSemanticModel)
            {
                if (context.Settings.IsAnyRefactoringEnabled(
                        RefactoringIdentifiers.ExpandProperty,
                        RefactoringIdentifiers.ExpandPropertyAndAddBackingField)
                    && propertyDeclaration.Span.Contains(context.Span)
                    && ExpandPropertyRefactoring.CanRefactor(propertyDeclaration))
                {
                    if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ExpandProperty))
                    {
                        context.RegisterRefactoring(
                            "Expand property",
                            cancellationToken => ExpandPropertyRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
                    }

                    if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ExpandPropertyAndAddBackingField))
                    {
                        context.RegisterRefactoring(
                            "Expand property and add backing field",
                            cancellationToken => ExpandPropertyAndAddBackingFieldRefactoring.RefactorAsync(context.Document, propertyDeclaration, context.Settings.PrefixFieldIdentifierWithUnderscore, cancellationToken));
                    }
                }

                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.NotifyPropertyChanged)
                    && await NotifyPropertyChangedRefactoring.CanRefactorAsync(context, propertyDeclaration))
                {
                    context.RegisterRefactoring(
                        "Notify property changed",
                        cancellationToken =>
                        {
                            return NotifyPropertyChangedRefactoring.RefactorAsync(
                                context.Document,
                                propertyDeclaration,
                                cancellationToken);
                        });
                }
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

                string newName = IdentifierHelper.CreateIdentifierName(propertyDeclaration.Type, semanticModel);

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
