// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CombineEnumerableWhereAndAnyRefactoring
    {
        internal static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (!invocationExpression.ContainsDiagnostics
                && !invocationExpression.SpanContainsDirectives())
            {
                MemberInvocationExpressionInfo invocationInfo = SyntaxInfo.MemberInvocationExpressionInfo(invocationExpression);
                if (invocationInfo.Success
                    && invocationInfo.NameText == "Any")
                {
                    ArgumentSyntax argument1 = invocationInfo.Arguments.SingleOrDefault(shouldthrow: false);

                    if (argument1 != null)
                    {
                        MemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.MemberInvocationExpressionInfo(invocationInfo.Expression);
                        if (invocationInfo2.Success
                            && invocationInfo2.NameText == "Where")
                        {
                            ArgumentSyntax argument2 = invocationInfo2.Arguments.SingleOrDefault(shouldthrow: false);

                            if (argument2 != null)
                            {
                                SemanticModel semanticModel = context.SemanticModel;
                                CancellationToken cancellationToken = context.CancellationToken;

                                MethodInfo methodInfo;
                                if (semanticModel.TryGetExtensionMethodInfo(invocationExpression, out methodInfo, ExtensionMethodKind.None, cancellationToken)
                                    && methodInfo.IsLinqExtensionOfIEnumerableOfTWithPredicate("Any"))
                                {
                                    MethodInfo methodInfo2;
                                    if (semanticModel.TryGetExtensionMethodInfo(invocationInfo2.InvocationExpression, out methodInfo2, ExtensionMethodKind.None, cancellationToken)
                                        && methodInfo2.IsLinqWhere(allowImmutableArrayExtension: true))
                                    {
                                        SingleParameterLambdaExpressionInfo lambda = SyntaxInfo.SingleParameterLambdaExpressionInfo(argument1.Expression);
                                        if (lambda.Success
                                            && lambda.Body is ExpressionSyntax)
                                        {
                                            SingleParameterLambdaExpressionInfo lambda2 = SyntaxInfo.SingleParameterLambdaExpressionInfo(argument2.Expression);
                                            if (lambda2.Success
                                                && lambda2.Body is ExpressionSyntax
                                                && lambda.ParameterName.Equals(lambda2.ParameterName, StringComparison.Ordinal))
                                            {
                                                context.ReportDiagnostic(
                                                    DiagnosticDescriptors.SimplifyLinqMethodChain,
                                                    Location.Create(context.SyntaxTree(), TextSpan.FromBounds(invocationInfo2.Name.SpanStart, invocationExpression.Span.End)));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            CancellationToken cancellationToken)
        {
            MemberInvocationExpressionInfo invocationInfo = SyntaxInfo.MemberInvocationExpressionInfo(invocationExpression);
            MemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.MemberInvocationExpressionInfo(invocationInfo.Expression);

            SingleParameterLambdaExpressionInfo lambda = SyntaxInfo.SingleParameterLambdaExpressionInfo((LambdaExpressionSyntax)invocationInfo.Arguments.First().Expression);
            SingleParameterLambdaExpressionInfo lambda2 = SyntaxInfo.SingleParameterLambdaExpressionInfo((LambdaExpressionSyntax)invocationInfo2.Arguments.First().Expression);

            BinaryExpressionSyntax logicalAnd = CSharpFactory.LogicalAndExpression(
                ((ExpressionSyntax)lambda2.Body).Parenthesize(),
                ((ExpressionSyntax)lambda.Body).Parenthesize());

            InvocationExpressionSyntax newNode = invocationInfo2.InvocationExpression
                .ReplaceNode(invocationInfo2.Name, invocationInfo.Name.WithTriviaFrom(invocationInfo2.Name))
                .WithArgumentList(invocationInfo2.ArgumentList.ReplaceNode((ExpressionSyntax)lambda2.Body, logicalAnd));

            return document.ReplaceNodeAsync(invocationExpression, newNode, cancellationToken);
        }
    }
}
