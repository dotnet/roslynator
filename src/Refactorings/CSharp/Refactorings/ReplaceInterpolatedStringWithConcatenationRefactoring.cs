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
    internal static class ReplaceInterpolatedStringWithConcatenationRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, InterpolatedStringExpressionSyntax interpolatedString)
        {
            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            if (contents.Count <= 1)
                return;

            foreach (InterpolatedStringContentSyntax content in contents)
            {
                if (content.Kind() == SyntaxKind.Interpolation)
                {
                    var interpolation = (InterpolationSyntax)content;

                    if (interpolation.AlignmentClause != null)
                        return;

                    if (interpolation.FormatClause != null)
                        return;
                }
            }

            context.RegisterRefactoring(
                "Replace interpolated string with concatenation",
                cancellationToken => RefactorAsync(context.Document, interpolatedString, cancellationToken));
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            int position = interpolatedString.SpanStart;

            bool isVerbatim = interpolatedString.IsVerbatim();

            ExpressionSyntax newNode = null;

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            InterpolatedStringContentSyntax content1 = contents[0];
            InterpolatedStringContentSyntax content2 = contents[1];

            if (content1.Kind() == SyntaxKind.InterpolatedStringText)
            {
                ExpressionSyntax expression1 = GetExpression((InterpolatedStringTextSyntax)content1, isVerbatim);
                ExpressionSyntax expression2 = ((InterpolationSyntax)content2).Expression;

                newNode = CreateAddExpression(expression1, expression2, position, isLeft: false, semanticModel: semanticModel, cancellationToken: cancellationToken);
            }
            else if (content2.Kind() == SyntaxKind.InterpolatedStringText)
            {
                ExpressionSyntax expression1 = ((InterpolationSyntax)content1).Expression;
                ExpressionSyntax expression2 = GetExpression((InterpolatedStringTextSyntax)content2, isVerbatim);

                newNode = CreateAddExpression(expression1, expression2, position, isLeft: true, semanticModel: semanticModel, cancellationToken: cancellationToken);
            }
            else
            {
                ExpressionSyntax expression1 = ((InterpolationSyntax)content1).Expression;
                ExpressionSyntax expression2 = ((InterpolationSyntax)content2).Expression;

                bool isLiteral = expression1 is LiteralExpressionSyntax;

                BinaryExpressionSyntax addExpression = CreateAddExpression(expression1, expression2, position, isLeft: !isLiteral, semanticModel: semanticModel, cancellationToken: cancellationToken);

                newNode = CreateAddExpression(addExpression.Left, addExpression.Right, position, isLeft: isLiteral, semanticModel: semanticModel, cancellationToken: cancellationToken);
            }

            for (int i = 2; i < contents.Count; i++)
            {
                InterpolatedStringContentSyntax content = contents[i];

                ExpressionSyntax expression = (content.Kind() == SyntaxKind.InterpolatedStringText)
                    ? GetExpression((InterpolatedStringTextSyntax)content, isVerbatim)
                    : ((InterpolationSyntax)content).Expression;

                newNode = CreateAddExpression(newNode, expression, position, isLeft: false, semanticModel: semanticModel, cancellationToken: cancellationToken);
            }

            newNode = newNode.Parenthesize().WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(interpolatedString, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static BinaryExpressionSyntax CreateAddExpression(
            ExpressionSyntax left,
            ExpressionSyntax right,
            int position,
            bool isLeft,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            BinaryExpressionSyntax addExpression = AddExpression(left, right);

            IMethodSymbol methodSymbol = semanticModel.GetSpeculativeMethodSymbol(position, addExpression);

            if (methodSymbol?.MethodKind != MethodKind.BuiltinOperator
                || methodSymbol.ContainingType?.SpecialType != SpecialType.System_String)
            {
                addExpression = (isLeft)
                    ? addExpression.WithLeft(AddToStringInvocation(left, semanticModel, cancellationToken))
                    : addExpression.WithRight(AddToStringInvocation(right, semanticModel, cancellationToken));
            }

            return addExpression;
        }

        private static ExpressionSyntax AddToStringInvocation(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if ((expression is LiteralExpressionSyntax)
                || semanticModel.GetTypeSymbol(expression, cancellationToken)?.IsReferenceTypeOrNullableType() == false)
            {
                return SimpleMemberInvocationExpression(
                    expression.Parenthesize(),
                    IdentifierName("ToString"),
                    ArgumentList());
            }
            else
            {
                return ConditionalAccessExpression(
                    expression.Parenthesize(),
                    InvocationExpression(IdentifierName("ToString"), ArgumentList()));
            }
        }

        private static ExpressionSyntax GetExpression(InterpolatedStringTextSyntax interpolatedStringText, bool isVerbatim)
        {
            SyntaxToken token = interpolatedStringText.TextToken;

            return LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                Literal(
                    ((isVerbatim) ? "@\"" : "\"") + token.Text + "\"",
                    token.ValueText));
        }
    }
}
