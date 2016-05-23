// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AddBooleanComparisonRefactoring
    {
        public const string Title = "Add boolean comparison";

        private static bool CanRefactor(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (expression.IsKind(SyntaxKind.LogicalNotExpression))
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)expression;

                if (logicalNot.Operand != null)
                    return IsNullableBoolean(logicalNot.Operand, semanticModel, cancellationToken);
            }
            else
            {
                return IsNullableBoolean(expression, semanticModel, cancellationToken);
            }

            return false;
        }

        private static bool IsNullableBoolean(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var namedTypeSymbol = semanticModel
                .GetTypeInfo(expression, cancellationToken)
                .ConvertedType as INamedTypeSymbol;

            return namedTypeSymbol?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                && namedTypeSymbol.TypeArguments[0].SpecialType == SpecialType.System_Boolean;
        }

        public static void Refactor(
            ExpressionSyntax expression,
            CodeRefactoringContext context,
            SemanticModel semanticModel)
        {
            if (CanRefactor(expression, semanticModel, context.CancellationToken))
            {
                context.RegisterRefactoring(
                    Title,
                    cancellationToken => RefactorAsync(context.Document, expression, cancellationToken));
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            BinaryExpressionSyntax newNode = CreateNewExpression(expression)
                .WithTriviaFrom(expression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(expression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static BinaryExpressionSyntax CreateNewExpression(ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.LogicalNotExpression))
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)expression;

                return SyntaxFactory.BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    logicalNot.Operand.WithoutTrivia(),
                    SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression));
            }
            else
            {
                return SyntaxFactory.BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    expression.WithoutTrivia(),
                    SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
            }
        }
    }
}
