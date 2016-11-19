// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLinqMethodChainRefactoring
    {
        public static void Analyze(
            SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocation,
            MemberAccessExpressionSyntax memberAccess,
            string methodName)
        {
            if (memberAccess.Expression?.IsKind(SyntaxKind.InvocationExpression) == true)
            {
                var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;

                if (invocation2.ArgumentList?.Arguments.Count == 1
                    && invocation2.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

                    if (memberAccess2.Name?.Identifier.ValueText == "Where"
                        && SyntaxAnalyzer.IsEnumerableExtensionMethod(invocation, methodName, 1, context.SemanticModel, context.CancellationToken)
                        && (SyntaxAnalyzer.IsEnumerableWhereOrImmutableArrayWhereMethod(invocation2, context.SemanticModel, context.CancellationToken)))
                    {
                        TextSpan span = TextSpan.FromBounds(memberAccess2.Name.Span.Start, invocation.Span.End);

                        if (!invocation.ContainsDirectives(span))
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.SimplifyLinqMethodChain,
                                Location.Create(invocation.SyntaxTree, span));
                        }
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;

            var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

            InvocationExpressionSyntax newInvocation = invocation2.WithExpression(
                memberAccess2.WithName(memberAccess.Name.WithTriviaFrom(memberAccess2.Name)));

            return await document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken).ConfigureAwait(false);
        }
    }
}
