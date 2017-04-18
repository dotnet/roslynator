// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceEqualsExpressionWithStringEqualsRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression.IsKind(SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression))
            {
                ExpressionSyntax left = binaryExpression.Left;

                if (left?.IsMissing == false
                    && !left.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    ExpressionSyntax right = binaryExpression.Right;

                    if (right?.IsMissing == false
                        && !right.IsKind(SyntaxKind.NullLiteralExpression))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol leftSymbol = semanticModel.GetTypeInfo(left, context.CancellationToken).ConvertedType;

                        if (leftSymbol?.IsString() == true)
                        {
                            ITypeSymbol rightSymbol = semanticModel.GetTypeInfo(right, context.CancellationToken).ConvertedType;

                            if (rightSymbol?.IsString() == true)
                            {
                                string title = (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
                                    ? "Replace == with string.Equals"
                                    : "Replace != with !string.Equals";

                                context.RegisterRefactoring(
                                    title,
                                    cancellationToken => RefactorAsync(context.Document, binaryExpression, cancellationToken));
                            }
                        }
                    }
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IFieldSymbol fieldSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_StringComparison).FindFieldWithConstantValue(0);

            ExpressionSyntax newNode = SimpleMemberInvocationExpression(
                StringType(),
                IdentifierName("Equals"),
                ArgumentList(
                    Argument(binaryExpression.Left),
                    Argument(binaryExpression.Right),
                    Argument(
                        SimpleMemberAccessExpression(
                            ParseName(MetadataNames.System_StringComparison).WithSimplifierAnnotation(),
                            IdentifierName(fieldSymbol.Name)))));

            if (binaryExpression.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken))
                newNode = LogicalNotExpression(newNode);

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}