// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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

            StringConcatenationExpression concatenation;

            if (StringConcatenationExpression.TryCreate(addExpression, context.SemanticModel, out concatenation)
                && !concatenation.ContainsNonSpecificExpression
                && (concatenation.ContainsLiteralExpression ^ concatenation.ContainsInterpolatedStringExpression)
                && (concatenation.ContainsRegular ^ concatenation.ContainsVerbatim)
                && (concatenation.ContainsVerbatim || addExpression.IsSingleLine(includeExteriorTrivia: false, cancellationToken: context.CancellationToken)))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.JoinStringExpressions, addExpression);
            }
        }

        private static bool ContainsMultiLine(StringConcatenationExpression concatenation, CancellationToken cancellationToken)
        {
            foreach (ExpressionSyntax expression in concatenation.Expressions)
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

            StringConcatenationExpression concatenation;
            if (StringConcatenationExpression.TryCreate(binaryExpression, semanticModel, out concatenation))
            {
                ExpressionSyntax newNode = null;

                if (concatenation.ContainsLiteralExpression)
                {
                    if (concatenation.ContainsVerbatim
                        && ContainsMultiLine(concatenation, cancellationToken))
                    {
                        newNode = concatenation.ToMultilineStringLiteral();
                    }
                    else
                    {
                        newNode = concatenation.ToStringLiteral();
                    }
                }
                else
                {
                    newNode = concatenation.ToInterpolatedString();
                }

                newNode = newNode.WithTriviaFrom(binaryExpression);

                return await document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken).ConfigureAwait(false);
            }

            Debug.Fail(binaryExpression.ToString());

            return document;
        }
    }
}
