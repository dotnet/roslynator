// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyConditionalExpressionRefactoring
    {
        public static bool CanRefactor(
            ConditionalExpressionSyntax conditionalExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax condition = conditionalExpression.Condition;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(condition, cancellationToken);

            if (typeSymbol?.IsBoolean() == true)
            {
                ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;

                if (whenTrue?.IsBooleanLiteralExpression() == true)
                {
                    ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;

                    if (whenFalse?.IsBooleanLiteralExpression() == true
                        && whenTrue.IsKind(SyntaxKind.TrueLiteralExpression) != whenFalse.IsKind(SyntaxKind.TrueLiteralExpression))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax condition = conditionalExpression.Condition;

            ExpressionSyntax newNode = condition.AppendTrailingTrivia(conditionalExpression.GetTrailingTrivia());

            if (condition.IsKind(SyntaxKind.ParenthesizedExpression)
                && SyntaxUtility.AreParenthesesRedundantOrInvalid(conditionalExpression))
            {
                newNode = ((ParenthesizedExpressionSyntax)condition).Expression.WithTriviaFrom(newNode);
            }

            if (conditionalExpression.WhenTrue.IsKind(SyntaxKind.FalseLiteralExpression))
                newNode = LogicalNotExpression(newNode).WithTriviaFrom(newNode);

            SyntaxNode newRoot = root.ReplaceNode(
                conditionalExpression,
                newNode.WithFormatterAnnotation());

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
