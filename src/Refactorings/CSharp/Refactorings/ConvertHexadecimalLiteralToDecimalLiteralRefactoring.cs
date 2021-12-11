// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertHexadecimalLiteralToDecimalLiteralRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            HexNumericLiteralExpressionInfo info = SyntaxInfo.HexNumericLiteralExpressionInfo(literalExpression);

            if (!info.Success)
                return;

            LiteralExpressionSyntax newLiteralExpression = CSharpFactory.LiteralExpression(info.Value);

            context.RegisterRefactoring(
                $"Convert to '{newLiteralExpression}'",
                ct => RefactorAsync(context.Document, literalExpression, newLiteralExpression, ct),
                RefactoringDescriptors.ConvertHexadecimalLiteralToDecimalLiteral);
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
