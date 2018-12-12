// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
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

                        ComputeRefactoring(context, "Replace object creation with default value", typeSymbol, localDeclarationStatement, value);
                        break;
                    }
                case ArrayCreationExpressionSyntax arrayCreation:
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(arrayCreation, context.CancellationToken);

                        ComputeRefactoring(context, "Replace array creation with default value", typeSymbol, localDeclarationStatement, value);
                        break;
                    }
                case ImplicitArrayCreationExpressionSyntax implicitArrayCreation:
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(implicitArrayCreation, context.CancellationToken);

                        ComputeRefactoring(context, "Replace array creation with default value", typeSymbol, localDeclarationStatement, value);
                        break;
                    }
            }
        }

        private static void ComputeRefactoring(
            RefactoringContext context,
            string title,
            ITypeSymbol typeSymbol,
            LocalDeclarationStatementSyntax localDeclarationStatement,
            ExpressionSyntax value)
        {
            if (typeSymbol?.SupportsExplicitDeclaration() == true)
            {
                context.RegisterRefactoring(
                    title,
                    cancellationToken => RefactorAsync(context.Document, localDeclarationStatement, value, typeSymbol, cancellationToken),
                    RefactoringIdentifiers.ReplaceObjectCreationWithDefaultValue);
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclarationStatement,
            ExpressionSyntax value,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax defaultValue = typeSymbol.GetDefaultValueSyntax(document.GetDefaultSyntaxOptions());

            LocalDeclarationStatementSyntax newNode = localDeclarationStatement.ReplaceNode(
                value,
                defaultValue.WithTriviaFrom(value));

            if (defaultValue is LiteralExpressionSyntax)
            {
                TypeSyntax oldType = newNode.Declaration.Type;

                TypeSyntax type = typeSymbol.ToTypeSyntax().WithSimplifierAnnotation().WithTriviaFrom(oldType);

                newNode = newNode.ReplaceNode(oldType, type);
            }

            return document.ReplaceNodeAsync(localDeclarationStatement, newNode, cancellationToken);
        }
    }
}
