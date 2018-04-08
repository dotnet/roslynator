// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.SyntaxInfo;

namespace Roslynator.CSharp.Analysis
{
    internal static class CombineEnumerableWhereAndAnyAnalysis
    {
        internal static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (invocationExpression.ContainsDiagnostics)
                return;

            if (invocationExpression.SpanContainsDirectives())
                return;

            SimpleMemberInvocationExpressionInfo invocationInfo = SimpleMemberInvocationExpressionInfo(invocationExpression);

            if (!invocationInfo.Success)
                return;

            if (invocationInfo.NameText != "Any")
                return;

            ArgumentSyntax argument1 = invocationInfo.Arguments.SingleOrDefault(shouldThrow: false);

            if (argument1 == null)
                return;

            SimpleMemberInvocationExpressionInfo invocationInfo2 = SimpleMemberInvocationExpressionInfo(invocationInfo.Expression);

            if (!invocationInfo2.Success)
                return;

            if (invocationInfo2.NameText != "Where")
                return;

            ArgumentSyntax argument2 = invocationInfo2.Arguments.SingleOrDefault(shouldThrow: false);

            if (argument2 == null)
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            IMethodSymbol methodSymbol = semanticModel.GetExtensionMethodInfo(invocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol, semanticModel, "Any"))
                return;

            IMethodSymbol methodSymbol2 = semanticModel.GetExtensionMethodInfo(invocationInfo2.InvocationExpression, cancellationToken).Symbol;

            if (methodSymbol2 == null)
                return;

            if (!SymbolUtility.IsLinqWhere(methodSymbol2, semanticModel, allowImmutableArrayExtension: true))
                return;

            SingleParameterLambdaExpressionInfo lambda = SingleParameterLambdaExpressionInfo(argument1.Expression);

            if (!lambda.Success)
                return;

            if (!(lambda.Body is ExpressionSyntax))
                return;

            SingleParameterLambdaExpressionInfo lambda2 = SingleParameterLambdaExpressionInfo(argument2.Expression);

            if (!lambda2.Success)
                return;

            if (!(lambda2.Body is ExpressionSyntax))
                return;

            if (!lambda.Parameter.Identifier.ValueText.Equals(lambda2.Parameter.Identifier.ValueText, StringComparison.Ordinal))
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.SimplifyLinqMethodChain,
                Location.Create(invocationExpression.SyntaxTree, TextSpan.FromBounds(invocationInfo2.Name.SpanStart, invocationExpression.Span.End)));
        }
    }
}
