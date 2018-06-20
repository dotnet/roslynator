// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;
using Roslynator.CSharp.Refactorings.MakeMemberAbstract;
using Roslynator.CSharp.Refactorings.MakeMemberVirtual;
using Roslynator.CSharp.Refactorings.ReplaceMethodWithProperty;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MethodDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)
                && context.Span.IsEmptyAndContainedInSpan(methodDeclaration))
            {
                await ChangeMethodReturnTypeToVoidRefactoring.ComputeRefactoringAsync(context, methodDeclaration).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddTypeParameter))
                AddTypeParameterRefactoring.ComputeRefactoring(context, methodDeclaration);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceMethodWithProperty)
                && methodDeclaration.HeaderSpan().Contains(context.Span)
                && ReplaceMethodWithPropertyRefactoring.CanRefactor(methodDeclaration))
            {
                context.RegisterRefactoring(
                    $"Replace '{methodDeclaration.Identifier.ValueText}' with property",
                    cancellationToken => ReplaceMethodWithPropertyRefactoring.RefactorAsync(context.Document, methodDeclaration, cancellationToken),
                    RefactoringIdentifiers.ReplaceMethodWithProperty);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && context.SupportsCSharp6
                && UseExpressionBodiedMemberRefactoring.CanRefactor(methodDeclaration, context.Span))
            {
                context.RegisterRefactoring(
                    UseExpressionBodiedMemberRefactoring.Title,
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, methodDeclaration, cancellationToken),
                    RefactoringIdentifiers.UseExpressionBodiedMember);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MakeMemberAbstract)
                && methodDeclaration.HeaderSpan().Contains(context.Span))
            {
                MakeMethodAbstractRefactoring.ComputeRefactoring(context, methodDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MakeMemberVirtual)
                && methodDeclaration.HeaderSpan().Contains(context.Span))
            {
                MakeMethodVirtualRefactoring.ComputeRefactoring(context, methodDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CopyDocumentationCommentFromBaseMember)
                && methodDeclaration.HeaderSpan().Contains(context.Span)
                && !methodDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoring(context, methodDeclaration, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameMethodAccordingToTypeName))
                await RenameMethodAccoringToTypeNameAsync(context, methodDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddMemberToInterface)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(methodDeclaration.Identifier))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                AddMemberToInterfaceRefactoring.ComputeRefactoring(context, methodDeclaration, semanticModel);
            }
        }

        private static async Task RenameMethodAccoringToTypeNameAsync(
            RefactoringContext context,
            MethodDeclarationSyntax methodDeclaration)
        {
            TypeSyntax returnType = methodDeclaration.ReturnType;

            if (returnType?.IsVoid() != false)
                return;

            SyntaxToken identifier = methodDeclaration.Identifier;

            if (!context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(identifier))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            ITypeSymbol typeSymbol = GetType(returnType, semanticModel, context.CancellationToken);

            if (typeSymbol == null)
                return;

            string newName = NameGenerator.CreateName(typeSymbol);

            if (string.IsNullOrEmpty(newName))
                return;

            newName = "Get" + newName;

            if (methodSymbol.IsAsync)
                newName += "Async";

            string oldName = identifier.ValueText;

            if (string.Equals(oldName, newName, StringComparison.Ordinal))
                return;

            if (!await MemberNameGenerator.IsUniqueMemberNameAsync(
                newName,
                methodSymbol,
                context.Solution,
                cancellationToken: context.CancellationToken).ConfigureAwait(false))
            {
                return;
            }

            context.RegisterRefactoring(
                $"Rename '{oldName}' to '{newName}'",
                cancellationToken => Renamer.RenameSymbolAsync(context.Solution, methodSymbol, newName, default(OptionSet), cancellationToken),
                RefactoringIdentifiers.RenameMethodAccordingToTypeName);
        }

        private static ITypeSymbol GetType(
            TypeSyntax returnType,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ITypeSymbol returnTypeSymbol = semanticModel.GetTypeSymbol(returnType, cancellationToken);

            if (returnTypeSymbol == null)
                return null;

            if (returnTypeSymbol.HasMetadataName(MetadataNames.System_Threading_Tasks_Task))
                return null;

            if (!returnTypeSymbol.OriginalDefinition.HasMetadataName(MetadataNames.System_Threading_Tasks_Task_T))
                return null;

            return ((INamedTypeSymbol)returnTypeSymbol).TypeArguments[0];
        }
    }
}