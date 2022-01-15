// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveInstantiationOfLocalVariableRefactoring
    {
        internal static async Task ComputeRefactoringAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclarationStatement)
        {
            EqualsValueClauseSyntax equalsValueClause = localDeclarationStatement
                .Declaration
                .Variables
                .SingleOrDefault(shouldThrow: false)?
                .Initializer;

            if (equalsValueClause == null)
                return;

            if (!context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(equalsValueClause))
                return;

            ExpressionSyntax value = equalsValueClause.Value;

            if (value == null)
                return;

            const string title = "Remove instantiation";

            switch (value)
            {
                case ObjectCreationExpressionSyntax objectCreation:
                    {
                        InitializerExpressionSyntax initializer = objectCreation.Initializer;

                        if (initializer?.Span.Contains(context.Span) == true)
                            return;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(objectCreation, context.CancellationToken);

                        ComputeRefactoring(context, title, typeSymbol, localDeclarationStatement, value);
                        break;
                    }
                case ArrayCreationExpressionSyntax arrayCreation:
                    {
                        InitializerExpressionSyntax initializer = arrayCreation.Initializer;

                        if (initializer?.Span.Contains(context.Span) == true)
                            return;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(arrayCreation, context.CancellationToken);

                        ComputeRefactoring(context, title, typeSymbol, localDeclarationStatement, value);
                        break;
                    }
                case ImplicitArrayCreationExpressionSyntax implicitArrayCreation:
                    {
                        InitializerExpressionSyntax initializer = implicitArrayCreation.Initializer;

                        if (initializer?.Span.Contains(context.Span) == true)
                            return;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(implicitArrayCreation, context.CancellationToken);

                        ComputeRefactoring(context, title, typeSymbol, localDeclarationStatement, value);
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
                    ct => RefactorAsync(context.Document, localDeclarationStatement, value, typeSymbol, ct),
                    RefactoringDescriptors.RemoveInstantiationOfLocalVariable);
            }
        }

        private static async Task<Document> RefactorAsync(
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

                TypeSyntax type = typeSymbol.ToTypeSyntax();

                if (!type.IsKind(SyntaxKind.NullableType)
                    && typeSymbol.IsReferenceType)
                {
                    SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                    if ((semanticModel.GetNullableContext(localDeclarationStatement.SpanStart) & NullableContext.AnnotationsEnabled) != 0)
                        type = SyntaxFactory.NullableType(type);
                }

                type = type.WithSimplifierAnnotation().WithTriviaFrom(oldType);

                newNode = newNode.ReplaceNode(oldType, type);
            }

            return await document.ReplaceNodeAsync(localDeclarationStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
