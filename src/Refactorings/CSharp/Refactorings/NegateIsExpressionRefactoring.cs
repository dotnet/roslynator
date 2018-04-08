// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class NegateIsExpressionRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression.IsKind(SyntaxKind.IsExpression))
                ComputeRefactoringCore(context, binaryExpression);
        }

        public static void ComputeRefactoring(RefactoringContext context, IsPatternExpressionSyntax isPatternExpression)
        {
            ComputeRefactoringCore(context, isPatternExpression);
        }

        private static void ComputeRefactoringCore(RefactoringContext context, ExpressionSyntax expression)
        {
            if (!context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(expression))
                return;

            SyntaxNode parent = expression.Parent;

            if (parent?.Kind() == SyntaxKind.ParenthesizedExpression)
            {
                if (parent.IsParentKind(SyntaxKind.LogicalNotExpression))
                {
                    RegisterRefactoring(context, (ExpressionSyntax)parent.Parent);
                }
                else
                {
                    RegisterRefactoring(context, (ExpressionSyntax)parent);
                }
            }
            else
            {
                RegisterRefactoring(context, expression);
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, ExpressionSyntax expression)
        {
            context.RegisterRefactoring(
                "Negate is",
                cancellationToken => RefactorAsync(context.Document, expression, cancellationToken));
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax newNode = Negator.LogicallyNegate(expression, semanticModel, cancellationToken);

            return await document.ReplaceNodeAsync(expression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}