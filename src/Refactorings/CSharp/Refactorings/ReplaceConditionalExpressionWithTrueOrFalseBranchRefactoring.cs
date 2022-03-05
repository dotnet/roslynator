// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceConditionalExpressionWithTrueOrFalseBranchRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            if (!parent.IsKind(SyntaxKind.ConditionalExpression))
                return;

            if (!context.Span.IsBetweenSpans(expression))
                return;

            var conditionalExpression = (ConditionalExpressionSyntax)parent;

            string title = null;

            if (expression == conditionalExpression.WhenTrue)
                title = "Replace ?: with true branch";

            if (expression == conditionalExpression.WhenFalse)
                title = "Replace ?: with false branch";

            if (title != null)
            {
                context.RegisterRefactoring(
                    title,
                    ct => RefactorAsync(context.Document, expression, ct),
                    RefactoringDescriptors.ReplaceConditionalExpressionWithTrueOrFalseBranch);
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default)
        {
            SyntaxNode parent = expression.Parent;

            ExpressionSyntax newNode = expression.WithTriviaFrom(parent);

            return document.ReplaceNodeAsync(parent, newNode, cancellationToken);
        }
    }
}
