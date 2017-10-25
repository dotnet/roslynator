// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceObjectCreationWithDefaultValueRefactoring
    {
        internal static async Task ComputeRefactoringAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclarationStatement)
        {
            EqualsValueClauseSyntax initializer = localDeclarationStatement
                .Declaration
                .Variables
                .SingleOrDefault(shouldthrow: false)?
                .Initializer;

            if (initializer != null
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(initializer))
            {
                ExpressionSyntax value = initializer.Value;

                if (value != null)
                {
                    switch (value.Kind())
                    {
                        case SyntaxKind.ObjectCreationExpression:
                            {
                                var objectCreation = (ObjectCreationExpressionSyntax)value;

                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(objectCreation, context.CancellationToken);

                                if (typeSymbol?.IsErrorType() == false)
                                {
                                    context.RegisterRefactoring(
                                        "Replace object creation with default value",
                                        cancellationToken => RefactorAsync(context.Document, localDeclarationStatement, value, typeSymbol, semanticModel, cancellationToken));
                                }

                                break;
                            }
                        case SyntaxKind.ArrayCreationExpression:
                            {
                                var arrayCreation = (ArrayCreationExpressionSyntax)value;

                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(arrayCreation, context.CancellationToken);

                                if (typeSymbol?.IsArrayType() == true)
                                {
                                    context.RegisterRefactoring(
                                   "Replace array creation with default value",
                                   cancellationToken => RefactorAsync(context.Document, localDeclarationStatement, value, typeSymbol, semanticModel, cancellationToken));
                                }

                                break;
                            }
                        case SyntaxKind.ImplicitArrayCreationExpression:
                            {
                                var implicitArrayCreation = (ImplicitArrayCreationExpressionSyntax)value;

                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(implicitArrayCreation, context.CancellationToken);

                                if (typeSymbol?.IsArrayType() == true)
                                {
                                    context.RegisterRefactoring(
                                        "Replace array creation with default value",
                                        cancellationToken => RefactorAsync(context.Document, localDeclarationStatement, value, typeSymbol, semanticModel, cancellationToken));
                                }

                                break;
                            }
                    }
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclarationStatement,
            ExpressionSyntax value,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            int position = localDeclarationStatement.SpanStart;

            ExpressionSyntax defaultValue = typeSymbol.ToDefaultValueSyntax(semanticModel, position);

            LocalDeclarationStatementSyntax newNode = localDeclarationStatement.ReplaceNode(
                value,
                defaultValue.WithTriviaFrom(value));

            TypeSyntax oldType = newNode.Declaration.Type;

            if (oldType.IsVar
                && !defaultValue.IsKind(SyntaxKind.DefaultExpression, SyntaxKind.SimpleMemberAccessExpression))
            {
                TypeSyntax type = typeSymbol.ToMinimalTypeSyntax(semanticModel, position);

                newNode = newNode.ReplaceNode(oldType, type.WithTriviaFrom(oldType));
            }

            return document.ReplaceNodeAsync(localDeclarationStatement, newNode, cancellationToken);
        }
    }
}
