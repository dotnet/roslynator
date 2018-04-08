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
                .SingleOrDefault(shouldThrow: false)?
                .Initializer;

            if (initializer == null)
                return;

            if (!context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(initializer))
                return;

            ExpressionSyntax value = initializer.Value;

            if (value == null)
                return;

            switch (value)
            {
                case ObjectCreationExpressionSyntax objectCreation:
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(objectCreation, context.CancellationToken);

                        ComputeRefactoring(context, "Replace object creation with default value", typeSymbol, localDeclarationStatement, value, semanticModel);
                        break;
                    }
                case ArrayCreationExpressionSyntax arrayCreation:
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(arrayCreation, context.CancellationToken);

                        ComputeRefactoring(context, "Replace array creation with default value", typeSymbol, localDeclarationStatement, value, semanticModel);
                        break;
                    }
                case ImplicitArrayCreationExpressionSyntax implicitArrayCreation:
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(implicitArrayCreation, context.CancellationToken);

                        ComputeRefactoring(context, "Replace array creation with default value", typeSymbol, localDeclarationStatement, value, semanticModel);
                        break;
                    }
            }
        }

        private static void ComputeRefactoring(
            RefactoringContext context,
            string title,
            ITypeSymbol typeSymbol,
            LocalDeclarationStatementSyntax localDeclarationStatement,
            ExpressionSyntax value,
            SemanticModel semanticModel)
        {
            if (typeSymbol?.SupportsExplicitDeclaration() == true)
            {
                context.RegisterRefactoring(
                    title,
                    cancellationToken => RefactorAsync(context.Document, localDeclarationStatement, value, typeSymbol, semanticModel, cancellationToken));
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

            ExpressionSyntax defaultValue = typeSymbol.GetDefaultValueSyntax(semanticModel, position);

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
