// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;
using Roslynator.CSharp.Refactorings.MakeMemberAbstract;
using Roslynator.CSharp.Refactorings.MakeMemberVirtual;
using Roslynator.CSharp.Refactorings.ReplacePropertyWithMethod;
using Roslynator.Utilities;

namespace Roslynator.CSharp.Refactorings
{
    internal static class PropertyDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplacePropertyWithMethod)
                && propertyDeclaration.HeaderSpan().Contains(context.Span))
            {
                ReplacePropertyWithMethodRefactoring.ComputeRefactoring(context, propertyDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemovePropertyInitializer)
                && RemovePropertyInitializerRefactoring.CanRefactor(context, propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Remove property initializer",
                    cancellationToken => RemovePropertyInitializerRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

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

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MakeMemberAbstract)
                && propertyDeclaration.HeaderSpan().Contains(context.Span))
            {
                MakePropertyAbstractRefactoring.ComputeRefactoring(context, propertyDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MakeMemberVirtual)
                && propertyDeclaration.HeaderSpan().Contains(context.Span))
            {
                MakePropertyVirtualRefactoring.ComputeRefactoring(context, propertyDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CopyDocumentationCommentFromBaseMember)
                && propertyDeclaration.HeaderSpan().Contains(context.Span))
            {
                await CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoringAsync(context, propertyDeclaration).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenamePropertyAccordingToTypeName))
            {
                TypeSyntax type = propertyDeclaration.Type;

                if (type != null)
                {
                    SyntaxToken identifier = propertyDeclaration.Identifier;

                    if (context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(identifier))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                        if (typeSymbol?.IsErrorType() == false)
                        {
                            string newName = NameGenerator.CreateName(typeSymbol);

                            if (!string.IsNullOrEmpty(newName))
                            {
                                string oldName = identifier.ValueText;

                                newName = StringUtility.FirstCharToUpper(newName);

                                if (!string.Equals(oldName, newName, StringComparison.Ordinal))
                                {
                                    ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

                                    if (await NameGenerator.IsUniqueMemberNameAsync(
                                        newName,
                                        symbol,
                                        context.Solution,
                                        cancellationToken: context.CancellationToken).ConfigureAwait(false))
                                    {
                                        context.RegisterRefactoring(
                                            $"Rename '{oldName}' to '{newName}'",
                                            cancellationToken => Renamer.RenameSymbolAsync(context.Solution, symbol, newName, default(OptionSet), cancellationToken));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
