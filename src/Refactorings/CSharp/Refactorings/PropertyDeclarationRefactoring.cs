// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

namespace Roslynator.CSharp.Refactorings
{
    internal static class PropertyDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ReplacePropertyWithMethod)
                && propertyDeclaration.HeaderSpan().Contains(context.Span))
            {
                ReplacePropertyWithMethodRefactoring.ComputeRefactoring(context, propertyDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemovePropertyInitializer)
                && RemovePropertyInitializerRefactoring.CanRefactor(context, propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Remove property initializer",
                    ct => RemovePropertyInitializerRefactoring.RefactorAsync(context.Document, propertyDeclaration, ct),
                    RefactoringDescriptors.RemovePropertyInitializer);
            }

            if (context.IsAnyRefactoringEnabled(
                RefactoringDescriptors.ConvertAutoPropertyToFullProperty,
                RefactoringDescriptors.ConvertAutoPropertyToFullPropertyWithoutBackingField)
                && propertyDeclaration.Span.Contains(context.Span)
                && ConvertAutoPropertyToFullPropertyWithoutBackingFieldRefactoring.CanRefactor(propertyDeclaration))
            {
                if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertAutoPropertyToFullProperty))
                {
                    context.RegisterRefactoring(
                        "Convert to full property",
                        ct => ConvertAutoPropertyToFullPropertyRefactoring.RefactorAsync(context.Document, propertyDeclaration, context.PrefixFieldIdentifierWithUnderscore, ct),
                        RefactoringDescriptors.ConvertAutoPropertyToFullProperty);
                }

                if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertAutoPropertyToFullPropertyWithoutBackingField))
                {
                    context.RegisterRefactoring(
                        "Convert to full property (without backing field)",
                        ct => ConvertAutoPropertyToFullPropertyWithoutBackingFieldRefactoring.RefactorAsync(context.Document, propertyDeclaration, ct),
                        RefactoringDescriptors.ConvertAutoPropertyToFullPropertyWithoutBackingField);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertBlockBodyToExpressionBody)
                && context.SupportsCSharp6
                && ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(propertyDeclaration, context.Span))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    ct => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, propertyDeclaration, ct),
                    RefactoringDescriptors.ConvertBlockBodyToExpressionBody);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.NotifyWhenPropertyChanges))
                await NotifyWhenPropertyChangesRefactoring.ComputeRefactoringAsync(context, propertyDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.MakeMemberAbstract)
                && propertyDeclaration.HeaderSpan().Contains(context.Span))
            {
                MakePropertyAbstractRefactoring.ComputeRefactoring(context, propertyDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.MakeMemberVirtual)
                && context.Span.IsEmptyAndContainedInSpan(propertyDeclaration.Identifier))
            {
                MakePropertyVirtualRefactoring.ComputeRefactoring(context, propertyDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.CopyDocumentationCommentFromBaseMember)
                && propertyDeclaration.HeaderSpan().Contains(context.Span)
                && !propertyDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoring(context, propertyDeclaration, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.RenamePropertyAccordingToTypeName))
                await RenamePropertyAccordingToTypeName(context, propertyDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddMemberToInterface)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(propertyDeclaration.Identifier))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                AddMemberToInterfaceRefactoring.ComputeRefactoring(context, propertyDeclaration, semanticModel);
            }
        }

        private static async Task RenamePropertyAccordingToTypeName(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            TypeSyntax type = propertyDeclaration.Type;

            if (type == null)
                return;

            SyntaxToken identifier = propertyDeclaration.Identifier;

            if (!context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(identifier))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

            if (typeSymbol?.IsErrorType() != false)
                return;

            string newName = NameGenerator.CreateName(typeSymbol);

            if (string.IsNullOrEmpty(newName))
                return;

            string oldName = identifier.ValueText;

            newName = StringUtility.FirstCharToUpper(newName);

            if (string.Equals(oldName, newName, StringComparison.Ordinal))
                return;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

            if (!await MemberNameGenerator.IsUniqueMemberNameAsync(
                newName,
                symbol,
                context.Solution,
                cancellationToken: context.CancellationToken)
                .ConfigureAwait(false))
            {
                return;
            }

            context.RegisterRefactoring(
                $"Rename '{oldName}' to '{newName}'",
                ct => Renamer.RenameSymbolAsync(context.Solution, symbol, newName, default(OptionSet), ct),
                RefactoringDescriptors.RenamePropertyAccordingToTypeName);
        }
    }
}
