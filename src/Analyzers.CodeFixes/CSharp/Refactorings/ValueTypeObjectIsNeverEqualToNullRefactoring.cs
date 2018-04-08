// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ValueTypeObjectIsNeverEqualToNullRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax right = binaryExpression.Right;

            ExpressionSyntax newNode = null;

            if (CSharpFacts.IsSimpleType(typeSymbol.SpecialType)
                || typeSymbol.ContainsMember<IMethodSymbol>(WellKnownMemberNames.EqualityOperatorName))
            {
                newNode = typeSymbol.GetDefaultValueSyntax(semanticModel, right.SpanStart)
                    .WithTriviaFrom(right)
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(right, newNode, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                INamedTypeSymbol equalityComparerSymbol = semanticModel
                    .GetTypeByMetadataName(MetadataNames.System_Collections_Generic_EqualityComparer_T)
                    .Construct(typeSymbol);

                newNode = InvocationExpression(
                    SimpleMemberAccessExpression(
                        SimpleMemberAccessExpression(equalityComparerSymbol.ToMinimalTypeSyntax(semanticModel, binaryExpression.SpanStart), IdentifierName("Default")), IdentifierName("Equals")),
                    ArgumentList(
                        Argument(binaryExpression.Left.WithoutTrivia()),
                        Argument(DefaultExpression(typeSymbol.ToMinimalTypeSyntax(semanticModel, right.SpanStart)))));

                if (binaryExpression.IsKind(SyntaxKind.NotEqualsExpression))
                    newNode = LogicalNotExpression(newNode);

                newNode = newNode
                    .WithTriviaFrom(binaryExpression)
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
