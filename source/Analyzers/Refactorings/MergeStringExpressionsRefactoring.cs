// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeStringExpressionsRefactoring
    {
        public static void AnalyzeAddExpression(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (node.ContainsDiagnostics)
                return;

            if (node.SpanContainsDirectives())
                return;

            var addExpression = (BinaryExpressionSyntax)node;

            StringExpressionChain chain;

            if (StringExpressionChain.TryCreate(addExpression, context.SemanticModel, out chain)
                && !chain.ContainsNonSpecificExpression
                && (chain.ContainsLiteralExpression ^ chain.ContainsInterpolatedStringExpression)
                && (chain.ContainsRegular ^ chain.ContainsVerbatim)
                && (chain.ContainsVerbatim || addExpression.IsSingleLine(includeExteriorTrivia: false, cancellationToken: context.CancellationToken)))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.MergeStringExpressions, addExpression);
            }
        }

        private static bool ContainsMultiLine(StringExpressionChain chain, CancellationToken cancellationToken)
        {
            foreach (ExpressionSyntax expression in chain.Expressions)
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

            StringExpressionChain chain;
            if (StringExpressionChain.TryCreate(binaryExpression, semanticModel, out chain))
            {
                ExpressionSyntax newNode = null;

                if (chain.ContainsLiteralExpression)
                {
                    if (chain.ContainsVerbatim
                        && ContainsMultiLine(chain, cancellationToken))
                    {
                        newNode = chain.ToMultilineStringLiteral();
                    }
                    else
                    {
                        newNode = chain.ToStringLiteral();
                    }
                }
                else
                {
                    newNode = chain.ToInterpolatedString();
                }

                newNode = newNode.WithTriviaFrom(binaryExpression);

                return await document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken).ConfigureAwait(false);
            }

            Debug.Fail(binaryExpression.ToString());

            return document;
        }
    }
}
