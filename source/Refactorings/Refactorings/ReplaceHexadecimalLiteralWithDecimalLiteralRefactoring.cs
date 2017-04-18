// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceHexadecimalLiteralWithDecimalLiteralRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression.IsHexadecimalNumericLiteral())
            {
                HexadecimalLiteralInfo info;
                if (HexadecimalLiteralInfo.TryCreate(literalExpression, out info))
                {
                    LiteralExpressionSyntax newLiteralExpression = info.ToDecimalLiteral();

                    context.RegisterRefactoring(
                        $"Replace '{info.Text}' with '{newLiteralExpression}'",
                        cancellationToken => RefactorAsync(context.Document, literalExpression, newLiteralExpression, cancellationToken));
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            LiteralExpressionSyntax newLiteralExpression,
            CancellationToken cancellationToken)
        {
            newLiteralExpression = newLiteralExpression.WithTriviaFrom(literalExpression);

            return document.ReplaceNodeAsync(literalExpression, newLiteralExpression, cancellationToken);
        }
    }
}