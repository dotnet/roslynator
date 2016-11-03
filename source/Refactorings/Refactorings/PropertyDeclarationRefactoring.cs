// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.ReplacePropertyWithMethod;

namespace Roslynator.CSharp.Refactorings
{
    internal static class PropertyDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MarkMemberAsStatic)
                && propertyDeclaration.Span.Contains(context.Span)
                && MarkMemberAsStaticRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark property as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)propertyDeclaration.Parent);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplacePropertyWithMethod)
                && propertyDeclaration.HeaderSpan().Contains(context.Span)
                && ReplacePropertyWithMethodRefactoring.CanRefactor(context, propertyDeclaration))
            {
                string propertyName = propertyDeclaration.Identifier.ValueText;

                string title = $"Replace '{propertyName}' with method";

                if (propertyDeclaration.AccessorList.Accessors.Count > 1)
                    title += "s";

                context.RegisterRefactoring(
                    title,
                    cancellationToken => ReplacePropertyWithMethodRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && propertyDeclaration.AccessorList?.Span.Contains(context.Span) == true
                && context.SupportsCSharp6
                && UseExpressionBodiedMemberRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemovePropertyInitializer)
                && RemovePropertyInitializerRefactoring.CanRefactor(context, propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Remove property initializer",
                    cancellationToken => RemovePropertyInitializerRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.SupportsSemanticModel)
            {
                if (context.IsAnyRefactoringEnabled(
                        RefactoringIdentifiers.ExpandProperty,
                        RefactoringIdentifiers.ExpandPropertyAndAddBackingField)
                    && propertyDeclaration.Span.Contains(context.Span)
                    && ExpandPropertyRefactoring.CanRefactor(propertyDeclaration))
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandProperty))
                    {
                        context.RegisterRefactoring(
                            "Expand property",
                            cancellationToken => ExpandPropertyRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
                    }

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandPropertyAndAddBackingField))
                    {
                        context.RegisterRefactoring(
                            "Expand property and add backing field",
                            cancellationToken => ExpandPropertyAndAddBackingFieldRefactoring.RefactorAsync(context.Document, propertyDeclaration, context.Settings.PrefixFieldIdentifierWithUnderscore, cancellationToken));
                    }
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.NotifyPropertyChanged)
                    && await NotifyPropertyChangedRefactoring.CanRefactorAsync(context, propertyDeclaration).ConfigureAwait(false))
                {
                    context.RegisterRefactoring(
                        "Notify property changed",
                        cancellationToken =>
                        {
                            return NotifyPropertyChangedRefactoring.RefactorAsync(
                                context.Document,
                                propertyDeclaration,
                                context.SupportsCSharp6,
                                cancellationToken);
                        });
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MakeMemberAbstract)
                && propertyDeclaration.HeaderSpan().Contains(context.Span)
                && MakeMemberAbstractRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Make property abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenamePropertyAccordingToTypeName)
                && context.SupportsSemanticModel
                && propertyDeclaration.Type != null
                && propertyDeclaration.Identifier.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel
                    .GetTypeInfo(propertyDeclaration.Type, context.CancellationToken)
                    .Type;

                if (typeSymbol?.IsErrorType() == false)
                {
                    string newName = SyntaxUtility.CreateIdentifier(typeSymbol);

                    if (!string.IsNullOrEmpty(newName))
                    {
                        newName = TextUtility.FirstCharToUpper(newName);

                        if (!string.Equals(newName, propertyDeclaration.Identifier.ValueText, StringComparison.Ordinal))
                        {
                            ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

                            context.RegisterRefactoring(
                                $"Rename property to '{newName}'",
                                cancellationToken => SymbolRenamer.RenameAsync(context.Document, symbol, newName, cancellationToken));
                        }
                    }
                }
            }
        }
    }
}
