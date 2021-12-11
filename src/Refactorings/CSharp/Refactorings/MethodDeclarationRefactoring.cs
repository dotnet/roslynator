// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using Microsoft.CodeAnalysis.CodeActions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MethodDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ChangeMethodReturnTypeToVoid)
                && context.Span.IsEmptyAndContainedInSpan(methodDeclaration))
            {
                await ChangeMethodReturnTypeToVoidRefactoring.ComputeRefactoringAsync(context, methodDeclaration).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddGenericParameterToDeclaration))
                AddGenericParameterToDeclarationRefactoring.ComputeRefactoring(context, methodDeclaration);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ReplaceMethodWithProperty)
                && methodDeclaration.HeaderSpan().Contains(context.Span)
                && ReplaceMethodWithPropertyRefactoring.CanRefactor(methodDeclaration))
            {
                context.RegisterRefactoring(
                    $"Replace '{methodDeclaration.Identifier.ValueText}' with property",
                    ct => ReplaceMethodWithPropertyRefactoring.RefactorAsync(context.Document, methodDeclaration, ct),
                    RefactoringDescriptors.ReplaceMethodWithProperty);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertBlockBodyToExpressionBody)
                && context.SupportsCSharp6
                && ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(methodDeclaration, context.Span))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    ct => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, methodDeclaration, ct),
                    RefactoringDescriptors.ConvertBlockBodyToExpressionBody);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.MakeMemberAbstract)
                && context.Span.IsEmptyAndContainedInSpan(methodDeclaration.Identifier))
            {
                MakeMethodAbstractRefactoring.ComputeRefactoring(context, methodDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.MakeMemberVirtual)
                && methodDeclaration.HeaderSpan().Contains(context.Span))
            {
                MakeMethodVirtualRefactoring.ComputeRefactoring(context, methodDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.CopyDocumentationCommentFromBaseMember)
                && methodDeclaration.HeaderSpan().Contains(context.Span)
                && !methodDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoring(context, methodDeclaration, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.RenameMethodAccordingToTypeName))
                await RenameMethodAccordingToTypeNameAsync(context, methodDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddParameterToInterfaceMember)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(methodDeclaration.Identifier))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                foreach (CodeAction codeAction in AddParameterToInterfaceMemberRefactoring.ComputeRefactoringForImplicitImplementation(
                    new CommonFixContext(context.Document, EquivalenceKey.Create(RefactoringDescriptors.AddParameterToInterfaceMember), semanticModel, context.CancellationToken),
                    methodDeclaration))
                {
                    context.RegisterRefactoring(codeAction);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddMemberToInterface)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(methodDeclaration.Identifier))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                AddMemberToInterfaceRefactoring.ComputeRefactoring(context, methodDeclaration, semanticModel);
            }
        }

        private static async Task RenameMethodAccordingToTypeNameAsync(
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
                cancellationToken: context.CancellationToken)
                .ConfigureAwait(false))
            {
                return;
            }

            context.RegisterRefactoring(
                $"Rename '{oldName}' to '{newName}'",
                ct => Renamer.RenameSymbolAsync(context.Solution, methodSymbol, newName, default(OptionSet), ct),
                RefactoringDescriptors.RenameMethodAccordingToTypeName);
        }

        private static ITypeSymbol GetType(
            TypeSyntax returnType,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
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
