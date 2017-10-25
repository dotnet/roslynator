// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class JoinStringExpressionsRefactoring
    {
        public static void AnalyzeAddExpression(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (node.ContainsDiagnostics)
                return;

            if (node.SpanContainsDirectives())
                return;

            var addExpression = (BinaryExpressionSyntax)node;

            StringConcatenationExpressionInfo concatenationInfo = SyntaxInfo.StringConcatenationExpressionInfo(addExpression, context.SemanticModel, context.CancellationToken);

            if (concatenationInfo.Success
                && !concatenationInfo.ContainsNonSpecificExpression
                && (concatenationInfo.ContainsLiteralExpression ^ concatenationInfo.ContainsInterpolatedStringExpression)
                && (concatenationInfo.ContainsRegular ^ concatenationInfo.ContainsVerbatim)
                && (concatenationInfo.ContainsVerbatim || addExpression.IsSingleLine(includeExteriorTrivia: false, cancellationToken: context.CancellationToken)))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.JoinStringExpressions, addExpression);
            }
        }

        private static bool ContainsMultiLine(StringConcatenationExpressionInfo concatenationInfo, CancellationToken cancellationToken)
        {
            foreach (ExpressionSyntax expression in concatenationInfo.Expressions)
            {
                if (expression.IsMultiLine(includeExteriorTrivia: false, cancellationToken: cancellationToken))
                    return true;
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            StringConcatenationExpressionInfo concatenationInfo = SyntaxInfo.StringConcatenationExpressionInfo(binaryExpression, semanticModel, cancellationToken);
            if (concatenationInfo.Success)
            {
                ExpressionSyntax newNode = null;

                if (concatenationInfo.ContainsLiteralExpression)
                {
                    if (concatenationInfo.ContainsVerbatim
                        && ContainsMultiLine(concatenationInfo, cancellationToken))
                    {
                        newNode = concatenationInfo.ToMultilineStringLiteral();
                    }
                    else
                    {
                        newNode = concatenationInfo.ToStringLiteral();
                    }
                }
                else
                {
                    newNode = concatenationInfo.ToInterpolatedString();
                }

                newNode = newNode.WithTriviaFrom(binaryExpression);

                return await document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken).ConfigureAwait(false);
            }

            Debug.Fail(binaryExpression.ToString());

            return document;
        }
    }
}
