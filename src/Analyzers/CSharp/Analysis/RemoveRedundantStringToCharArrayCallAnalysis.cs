// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class RemoveRedundantStringToCharArrayCallAnalysis
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
        {
            if (!IsFixable(invocation, context.SemanticModel, context.CancellationToken))
                return;

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            TextSpan span = TextSpan.FromBounds(memberAccess.OperatorToken.SpanStart, invocation.Span.End);

            if (invocation.ContainsDirectives(span))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantStringToCharArrayCall, Location.Create(invocation.SyntaxTree, span));
        }

        public static bool IsFixable(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ParentIsElementAccessOrForEachExpression(invocation.WalkUpParentheses()))
                return false;

            SimpleMemberInvocationExpressionInfo info = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocation);

            if (!info.Success)
                return false;

            if (info.Arguments.Any())
                return false;

            if (!string.Equals(info.NameText, "ToCharArray"))
                return false;

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, cancellationToken);

            if (!SymbolUtility.IsPublicInstanceNonGeneric(methodSymbol, "ToCharArray"))
                return false;

            if (methodSymbol.ContainingType?.SpecialType != SpecialType.System_String)
                return false;

            if (methodSymbol.Parameters.Any())
                return false;

            if (!(methodSymbol.ReturnType is IArrayTypeSymbol arrayType))
                return false;

            return arrayType.ElementType.SpecialType == SpecialType.System_Char;
        }

        private static bool ParentIsElementAccessOrForEachExpression(ExpressionSyntax expression)
        {
            if (expression.IsParentKind(SyntaxKind.ElementAccessExpression))
                return true;

            if (expression.IsParentKind(SyntaxKind.ForEachStatement))
            {
                var forEachStatement = (ForEachStatementSyntax)expression.Parent;

                if (expression == forEachStatement.Expression)
                    return true;
            }

            return false;
        }
    }
}