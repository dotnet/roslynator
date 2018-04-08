// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class SimplifyLinqMethodChainAnalysis
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationInfo.Expression);

            if (!invocationInfo2.Success)
                return;

            if (invocationInfo2.Arguments.Count != 1)
                return;

            if (invocationInfo2.NameText != "Where")
                return;

            InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            IMethodSymbol methodSymbol = semanticModel.GetExtensionMethodInfo(invocation, cancellationToken).Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithoutParameters(methodSymbol, invocationInfo.NameText, semanticModel))
                return;

            IMethodSymbol methodSymbol2 = semanticModel.GetExtensionMethodInfo(invocationInfo2.InvocationExpression, cancellationToken).Symbol;

            if (methodSymbol2 == null)
                return;

            if (!SymbolUtility.IsLinqWhere(methodSymbol2, semanticModel, allowImmutableArrayExtension: true))
                return;

            TextSpan span = TextSpan.FromBounds(invocationInfo2.Name.SpanStart, invocation.Span.End);

            if (invocation.ContainsDirectives(span))
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.SimplifyLinqMethodChain,
                Location.Create(invocation.SyntaxTree, span));
        }
    }
}
