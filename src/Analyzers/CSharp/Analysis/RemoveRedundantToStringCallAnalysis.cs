// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class RemoveRedundantToStringCallAnalysis
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            if (!IsFixable(invocationInfo, context.SemanticModel, context.CancellationToken))
                return;

            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            TextSpan span = TextSpan.FromBounds(invocationInfo.OperatorToken.SpanStart, invocationExpression.Span.End);

            if (invocationExpression.ContainsDirectives(span))
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.RemoveRedundantToStringCall,
                Location.Create(invocationExpression.SyntaxTree, span));
        }

        public static bool IsFixable(
            SimpleMemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocationInfo.Expression.Kind() == SyntaxKind.BaseExpression)
                return false;

            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocationExpression, cancellationToken);

            if (SymbolUtility.IsPublicInstanceNonGeneric(methodSymbol, "ToString")
                && methodSymbol.ReturnType.SpecialType == SpecialType.System_String
                && !methodSymbol.Parameters.Any())
            {
                INamedTypeSymbol containingType = methodSymbol.ContainingType;

                if (containingType?.IsReferenceType == true
                    && containingType.SpecialType != SpecialType.System_Enum)
                {
                    if (containingType.SpecialType == SpecialType.System_String)
                        return true;

                    ExpressionSyntax expression = invocationExpression.WalkUpParentheses();

                    SyntaxNode parent = expression.Parent;

                    if (parent != null)
                    {
                        SyntaxKind kind = parent.Kind();

                        if (kind == SyntaxKind.Interpolation)
                        {
                            return IsNotHidden(methodSymbol, containingType);
                        }
                        else if (kind == SyntaxKind.AddExpression)
                        {
                            if (!parent.ContainsDiagnostics
                                && IsNotHidden(methodSymbol, containingType))
                            {
                                var addExpression = (BinaryExpressionSyntax)expression.Parent;

                                if (CSharpUtility.IsStringConcatenation(addExpression, semanticModel, cancellationToken))
                                {
                                    BinaryExpressionSyntax newAddExpression = addExpression.ReplaceNode(expression, invocationInfo.Expression);

                                    IMethodSymbol speculativeSymbol = semanticModel.GetSpeculativeMethodSymbol(addExpression.SpanStart, newAddExpression);

                                    return SymbolUtility.IsStringAdditionOperator(speculativeSymbol);
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsNotHidden(IMethodSymbol methodSymbol, INamedTypeSymbol containingType)
        {
            if (containingType.IsObject())
                return true;

            if (methodSymbol.IsOverride)
            {
                IMethodSymbol overriddenMethod = methodSymbol.OverriddenMethod;

                while (overriddenMethod != null)
                {
                    if (overriddenMethod.ContainingType?.SpecialType == SpecialType.System_Object)
                        return true;

                    overriddenMethod = overriddenMethod.OverriddenMethod;
                }
            }

            return false;
        }
    }
}