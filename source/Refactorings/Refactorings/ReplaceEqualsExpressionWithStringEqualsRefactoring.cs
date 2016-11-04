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

                if (left?.IsMissing == false)
                {
                    ExpressionSyntax right = binaryExpression.Right;

                    if (right?.IsMissing == false)
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
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            INamedTypeSymbol symbol = semanticModel.Compilation.GetTypeByMetadataName("System.StringComparison");

            IFieldSymbol fieldSymbol = GetDefaultFieldSymbol(symbol);

            ExpressionSyntax newNode = InvocationExpression(
                StringType(),
                "Equals",
                ArgumentList(
                    Argument(binaryExpression.Left),
                    Argument(binaryExpression.Right),
                    Argument(
                        SimpleMemberAccessExpression(
                            ParseName("System.StringComparison").WithSimplifierAnnotation(),
                            IdentifierName(fieldSymbol.Name)))));

            if (binaryExpression.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken))
                newNode = LogicalNotExpression(newNode);

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(binaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static IFieldSymbol GetDefaultFieldSymbol(INamedTypeSymbol symbol)
        {
            foreach (ISymbol member in symbol.GetMembers())
            {
                if (member.IsField())
                {
                    var fieldSymbol = (IFieldSymbol)member;

                    if (fieldSymbol.HasConstantValue)
                    {
                        object constantValue = fieldSymbol.ConstantValue;

                        if (constantValue is int)
                        {
                            var value = (int)constantValue;

                            if (value == 0)
                                return fieldSymbol;
                        }
                    }
                }
            }

            return null;
        }
    }
}