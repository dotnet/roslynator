// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceCastWithAsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, CastExpressionSyntax castExpression)
        {
            TypeSyntax type = castExpression.Type;

            if (type?.IsMissing != false)
                return;

            ExpressionSyntax expression = castExpression.Expression;

            if (expression?.IsMissing != false)
                return;

            context.RegisterRefactoring(
                "Replace cast with as",
                cancellationToken => RefactorAsync(context.Document, castExpression, cancellationToken));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            CastExpressionSyntax castExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = castExpression.Expression;
            TypeSyntax type = castExpression.Type;

            BinaryExpressionSyntax newNode = CSharpFactory.AsExpression(expression.WithTriviaFrom(type), type.WithTriviaFrom(expression))
                .WithTriviaFrom(castExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(castExpression, newNode, cancellationToken);
        }
    }
}