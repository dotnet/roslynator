// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis;

internal static class RemoveRedundantToStringCallAnalysis
{
    public static void Analyze(SyntaxNodeAnalysisContext context, in SimpleMemberInvocationExpressionInfo invocationInfo)
    {
        if (invocationInfo.OperatorToken.ContainsDirectives)
            return;

        if (invocationInfo.ArgumentList.ContainsDirectives)
            return;

        if (!IsFixable(invocationInfo, context.SemanticModel, context.CancellationToken))
            return;

        InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.RemoveRedundantToStringCall,
            Location.Create(invocationExpression.SyntaxTree, TextSpan.FromBounds(invocationInfo.OperatorToken.SpanStart, invocationExpression.Span.End)));
    }

    private static bool IsFixable(
        in SimpleMemberInvocationExpressionInfo invocationInfo,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        static bool IsToString(IMethodSymbol methodSymbol1)
        {
            return methodSymbol1?.DeclaredAccessibility == Accessibility.Public
                && !methodSymbol1.IsStatic
                && !methodSymbol1.IsGenericMethod
                && string.Equals(methodSymbol1.Name, "ToString", StringComparison.Ordinal)
                && methodSymbol1.ReturnType.SpecialType == SpecialType.System_String
                && !methodSymbol1.Parameters.Any();
        }

        if (invocationInfo.Expression.IsKind(SyntaxKind.BaseExpression))
            return false;

        if (invocationInfo.Expression.WalkDownParentheses().IsKind(SyntaxKind.StringLiteralExpression, SyntaxKind.InterpolatedStringExpression))
            return true;

        InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

        IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocationExpression, cancellationToken);

        if (IsToString(methodSymbol))
        {
            INamedTypeSymbol containingType = methodSymbol.ContainingType;

            if (containingType?.IsReferenceType == true
                && containingType.SpecialType != SpecialType.System_ValueType
                && containingType.SpecialType != SpecialType.System_Enum)
            {
                if (containingType.SpecialType == SpecialType.System_String)
                    return true;

                ExpressionSyntax expression = invocationExpression.WalkUpParentheses();
                switch (expression.Parent.Kind())
                {
                    case SyntaxKind.Interpolation:
                        {
                            return IsNotHidden(methodSymbol, containingType);
                        }
                    case SyntaxKind.AddExpression:
                        {
                            var addExpression = (BinaryExpressionSyntax)expression.Parent;
                            if (addExpression.Right == expression)
                                return semanticModel.GetTypeInfo(addExpression.Left, cancellationToken).Type?.SpecialType == SpecialType.System_String;

                            return semanticModel.GetTypeInfo(addExpression.Right, cancellationToken).Type?.SpecialType == SpecialType.System_String
                                && (addExpression.Right.WalkDownParentheses() is not InvocationExpressionSyntax invocationExpression2
                                    || !IsToString(semanticModel.GetMethodSymbol(invocationExpression2, cancellationToken)));
                        }
                }
            }
        }

        return false;
    }

    private static bool IsNotHidden(IMethodSymbol methodSymbol, INamedTypeSymbol containingType)
    {
        if (containingType.SpecialType == SpecialType.System_Object)
            return true;

        if (methodSymbol.IsOverride)
        {
            IMethodSymbol overriddenMethod = methodSymbol.OverriddenMethod;

            while (overriddenMethod is not null)
            {
                if (overriddenMethod.ContainingType?.SpecialType == SpecialType.System_Object)
                    return true;

                overriddenMethod = overriddenMethod.OverriddenMethod;
            }
        }

        return false;
    }
}
