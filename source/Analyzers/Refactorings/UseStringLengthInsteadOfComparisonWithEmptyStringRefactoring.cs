// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseStringLengthInsteadOfComparisonWithEmptyStringRefactoring
    {
        public static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            var equalsExpression = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax left = equalsExpression.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = equalsExpression.Right;

                if (right?.IsMissing == false)
                {
                    SemanticModel semanticModel = context.SemanticModel;
                    CancellationToken cancellationToken = context.CancellationToken;

                    if (CSharpUtility.IsEmptyString(left, semanticModel, cancellationToken))
                    {
                        if (IsString(right, semanticModel, cancellationToken))
                            ReportDiagnostic(context, equalsExpression);
                    }
                    else if (CSharpUtility.IsEmptyString(right, semanticModel, cancellationToken)
                        && IsString(left, semanticModel, cancellationToken))
                    {
                        ReportDiagnostic(context, equalsExpression);
                    }
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if (!node.SpanContainsDirectives())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseStringLengthInsteadOfComparisonWithEmptyString,
                    node);
            }
        }

        private static bool IsString(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return expression?.IsMissing == false
                && semanticModel.GetTypeInfo(expression, cancellationToken).ConvertedType?.IsString() == true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax left = binaryExpression.Left;

            ExpressionSyntax right = binaryExpression.Right;

            BinaryExpressionSyntax newNode;

            if (CSharpUtility.IsEmptyString(left, semanticModel, cancellationToken))
            {
                newNode = binaryExpression
                    .WithLeft(NumericLiteralExpression(0))
                    .WithRight(CreateConditionalAccess(right));
            }
            else if (CSharpUtility.IsEmptyString(right, semanticModel, cancellationToken))
            {
                newNode = binaryExpression
                    .WithLeft(CreateConditionalAccess(left))
                    .WithRight(NumericLiteralExpression(0));
            }
            else
            {
                Debug.Fail(binaryExpression.ToString());
                return document;
            }

            newNode = newNode.WithTriviaFrom(binaryExpression).WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static ConditionalAccessExpressionSyntax CreateConditionalAccess(ExpressionSyntax expression)
        {
            return ConditionalAccessExpression(
                expression.Parenthesize(),
                MemberBindingExpression(IdentifierName("Length")));
        }
    }
}
