// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class MethodDeclarationRefactoring
    {
        public static bool CanConvertToReadOnlyProperty(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.ReturnType?.IsVoid() == false
                && methodDeclaration.ParameterList?.Parameters.Count == 0
                && methodDeclaration.TypeParameterList == null;
        }

        public static async Task<Document> ConvertToReadOnlyPropertyAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PropertyDeclarationSyntax propertyDeclaration = ConvertToReadOnlyProperty(methodDeclaration)
                .WithTriviaFrom(methodDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(methodDeclaration, propertyDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        public static PropertyDeclarationSyntax ConvertToReadOnlyProperty(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            if (methodDeclaration.ExpressionBody != null)
            {
                return PropertyDeclaration(
                    methodDeclaration.AttributeLists,
                    methodDeclaration.Modifiers,
                    methodDeclaration.ReturnType,
                    methodDeclaration.ExplicitInterfaceSpecifier,
                    methodDeclaration.Identifier,
                    null,
                    methodDeclaration.ExpressionBody,
                    null);
            }
            else
            {
                return PropertyDeclaration(
                    methodDeclaration.AttributeLists,
                    methodDeclaration.Modifiers,
                    methodDeclaration.ReturnType,
                    methodDeclaration.ExplicitInterfaceSpecifier,
                    methodDeclaration.Identifier,
                    CreateAccessorList(methodDeclaration));
            }
        }

        private static AccessorListSyntax CreateAccessorList(MethodDeclarationSyntax method)
        {
            if (method.Body != null)
            {
                bool singleline = method.Body.Statements.Count == 1
                    && method.Body.Statements[0].IsSingleline();

                return CreateAccessorList(Block(method.Body?.Statements), singleline)
                    .WithOpenBraceToken(method.Body.OpenBraceToken)
                    .WithCloseBraceToken(method.Body.CloseBraceToken);
            }

            return CreateAccessorList(Block(), singleline: true);
        }

        private static AccessorListSyntax CreateAccessorList(BlockSyntax block, bool singleline)
        {
            AccessorListSyntax accessorList =
                AccessorList(
                    SingletonList(
                        AccessorDeclaration(
                            SyntaxKind.GetAccessorDeclaration,
                            block)));

            if (singleline)
                accessorList = WhitespaceOrEndOfLineRemover.RemoveFrom(accessorList);

            return accessorList;
        }

        internal static async Task RenameAccordingToTypeNameAsync(
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
            CancellationToken cancellationToken)
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
