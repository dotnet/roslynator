// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class MethodDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (MarkMemberAsStaticRefactoring.CanRefactor(methodDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark method as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, methodDeclaration, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)methodDeclaration.Parent);
            }

            if (methodDeclaration.HeaderSpan().Contains(context.Span)
                && ConvertMethodToReadOnlyPropertyRefactoring.CanRefactor(methodDeclaration))
            {
                context.RegisterRefactoring(
                    "Convert to read-only property",
                    cancellationToken => ConvertMethodToReadOnlyPropertyRefactoring.RefactorAsync(context.Document, methodDeclaration, cancellationToken));
            }

            if (methodDeclaration.Body?.Span.Contains(context.Span) == true
                && context.SupportsCSharp6
                && UseExpressionBodiedMemberRefactoring.CanRefactor(methodDeclaration))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, methodDeclaration, cancellationToken));
            }

            if (methodDeclaration.HeaderSpan().Contains(context.Span)
                && MakeMemberAbstractRefactoring.CanRefactor(context, methodDeclaration))
            {
                context.RegisterRefactoring(
                    "Make method abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, methodDeclaration, cancellationToken));
            }

            if (context.SupportsSemanticModel)
                await RenameMethodAccoringToTypeNameAsync(context, methodDeclaration);
        }

        private static async Task RenameMethodAccoringToTypeNameAsync(
            RefactoringContext context,
            MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.ReturnType?.IsVoid() == false
                && methodDeclaration.Identifier.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

                ITypeSymbol typeSymbol = GetType(methodDeclaration.ReturnType, semanticModel, context.CancellationToken);

                if (typeSymbol != null)
                {
                    string newName = NamingHelper.CreateIdentifierName(typeSymbol);

                    if (!string.IsNullOrEmpty(newName))
                    {
                        newName = "Get" + newName;

                        if (methodSymbol.IsAsync)
                            newName += "Async";

                        if (!string.Equals(newName, methodDeclaration.Identifier.ValueText, StringComparison.Ordinal))
                        {
                            context.RegisterRefactoring(
                                $"Rename method to '{newName}'",
                                cancellationToken => methodSymbol.RenameAsync(newName, context.Document, cancellationToken));
                        }
                    }
                }
            }
        }

        private static ITypeSymbol GetType(
            TypeSyntax returnType,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var returnTypeSymbol = semanticModel.GetTypeInfo(returnType, cancellationToken).Type as INamedTypeSymbol;

            if (returnTypeSymbol != null)
            {
                INamedTypeSymbol taskSymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");

                if (taskSymbol != null)
                {
                    if (returnTypeSymbol.Equals(taskSymbol))
                        return null;

                    INamedTypeSymbol taskOfTSymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");

                    if (taskOfTSymbol != null
                        && returnTypeSymbol.ConstructedFrom.Equals(taskOfTSymbol))
                    {
                        return returnTypeSymbol.TypeArguments[0];
                    }
                }
            }

            return returnTypeSymbol;
        }
    }
}