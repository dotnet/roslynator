// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class JoinStringExpressionsRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            StringConcatenationExpressionInfo concatenationInfo = SyntaxInfo.StringConcatenationExpressionInfo(binaryExpression, semanticModel, cancellationToken);

            ExpressionSyntax newNode = null;

            StringConcatenationAnalysis analysis = concatenationInfo.Analyze();

            if (analysis.ContainsStringLiteral)
            {
                if (analysis.ContainsVerbatimExpression
                    && concatenationInfo.ContainsMultiLineExpression())
                {
                    newNode = concatenationInfo.ToMultiLineStringLiteralExpression();
                }
                else
                {
                    newNode = concatenationInfo.ToStringLiteralExpression();
                }
            }
            else
            {
                newNode = concatenationInfo.ToInterpolatedStringExpression();
            }

            newNode = newNode.WithTriviaFrom(binaryExpression);

            return await document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
