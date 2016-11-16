// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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

            ExpressionSyntax newNode = (conditionalExpression.WhenTrue.IsKind(SyntaxKind.TrueLiteralExpression))
                ? condition
                : condition.Negate();

            TextSpan span = TextSpan.FromBounds(
                conditionalExpression.Condition.Span.End,
                conditionalExpression.FullSpan.End);

            IEnumerable<SyntaxTrivia> trivia = conditionalExpression.DescendantTrivia(span);

            if (trivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
            {
                newNode = newNode.WithTrailingTrivia(trivia);
            }
            else
            {
                newNode = newNode.WithoutTrailingTrivia();
            }

            SyntaxNode newRoot = root.ReplaceNode(conditionalExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
