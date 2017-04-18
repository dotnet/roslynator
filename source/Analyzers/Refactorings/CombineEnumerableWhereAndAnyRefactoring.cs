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
                MemberInvocationExpressionWithSingleParameter invocation;
                if (MemberInvocationExpressionWithSingleParameter.TryCreate(invocationExpression, out invocation)
                    && invocation.NameText == "Any")
                {
                    MemberInvocationExpressionWithSingleParameter invocation2;
                    if (MemberInvocationExpressionWithSingleParameter.TryCreate(invocation.Expression, out invocation2)
                        && invocation2.NameText == "Where")
                    {
                        SemanticModel semanticModel = context.SemanticModel;
                        CancellationToken cancellationToken = context.CancellationToken;

                        if (semanticModel.GetExtensionMethodInfo(invocationExpression, cancellationToken).MethodInfo.IsLinqExtensionOfIEnumerableOfTWithPredicate("Any")
                            && semanticModel.GetExtensionMethodInfo(invocation2.InvocationExpression, cancellationToken).MethodInfo.IsLinqWhere(allowImmutableArrayExtension: true))
                        {
                            LambdaExpressionWithSingleParameter lambda;
                            if (LambdaExpressionWithSingleParameter.TryCreate(invocation.Argument.Expression, out lambda)
                                && lambda.Body is ExpressionSyntax)
                            {
                                LambdaExpressionWithSingleParameter lambda2;
                                if (LambdaExpressionWithSingleParameter.TryCreate(invocation2.Argument.Expression, out lambda2)
                                    && lambda2.Body is ExpressionSyntax
                                    && lambda.ParameterName.Equals(lambda2.ParameterName, StringComparison.Ordinal))
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.SimplifyLinqMethodChain,
                                        Location.Create(context.SyntaxTree(), TextSpan.FromBounds(invocation2.Name.SpanStart, invocationExpression.Span.End)));
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
            MemberInvocationExpressionWithSingleParameter invocation = MemberInvocationExpressionWithSingleParameter.Create(invocationExpression);
            MemberInvocationExpressionWithSingleParameter invocation2 = MemberInvocationExpressionWithSingleParameter.Create((InvocationExpressionSyntax)invocation.Expression);

            LambdaExpressionWithSingleParameter lambda = LambdaExpressionWithSingleParameter.Create((LambdaExpressionSyntax)invocation.Argument.Expression);
            LambdaExpressionWithSingleParameter lambda2 = LambdaExpressionWithSingleParameter.Create((LambdaExpressionSyntax)invocation2.Argument.Expression);

            BinaryExpressionSyntax logicalAnd = CSharpFactory.LogicalAndExpression(
                ((ExpressionSyntax)lambda2.Body).Parenthesize(moveTrivia: true).WithSimplifierAnnotation(),
                ((ExpressionSyntax)lambda.Body).Parenthesize(moveTrivia: true).WithSimplifierAnnotation());

            InvocationExpressionSyntax newNode = invocation2.InvocationExpression
                .ReplaceNode(invocation2.Name, invocation.Name.WithTriviaFrom(invocation2.Name))
                .WithArgumentList(invocation2.ArgumentList.ReplaceNode((ExpressionSyntax)lambda2.Body, logicalAnd));

            return document.ReplaceNodeAsync(invocationExpression, newNode, cancellationToken);
        }
    }
}
