// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AddBooleanComparisonRefactoring
    {
        public const string Title = "Add boolean comparison";

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (await CanRefactorAsync(context, expression))
            {
                context.RegisterRefactoring(
                    Title,
                    cancellationToken => RefactorAsync(context.Document, expression, cancellationToken));
            }
        }

        private static async Task<bool> CanRefactorAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.LogicalNotExpression))
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)expression;

                if (logicalNot.Operand != null)
                    return await IsNullableBooleanAsync(context, logicalNot.Operand);
            }
            else
            {
                return await IsNullableBooleanAsync(context, expression);
            }

            return false;
        }

        private static async Task<bool> IsNullableBooleanAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (context.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                var namedTypeSymbol = semanticModel
                    .GetTypeInfo(expression, context.CancellationToken)
                    .ConvertedType as INamedTypeSymbol;

                return namedTypeSymbol?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                    && namedTypeSymbol.TypeArguments[0].SpecialType == SpecialType.System_Boolean;
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
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
