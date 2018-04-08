// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseInsteadOfCountMethodAnalysis
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            IMethodSymbol methodSymbol = semanticModel.GetReducedExtensionMethodInfo(invocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithoutParameters(methodSymbol, "Count", semanticModel))
                return;

            string propertyName = CSharpUtility.GetCountOrLengthPropertyName(invocationInfo.Expression, semanticModel, cancellationToken);

            if (propertyName != null)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfCountMethod,
                    Location.Create(context.Node.SyntaxTree, TextSpan.FromBounds(invocationInfo.Name.SpanStart, invocationExpression.Span.End)),
                    ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("PropertyName", propertyName) }),
                    propertyName);
            }
            else
            {
                bool isFixable = false;

                SyntaxNode parent = invocationExpression.Parent;

                SyntaxKind kind = parent.Kind();

                if (kind.Is(
                    SyntaxKind.EqualsExpression,
                    SyntaxKind.NotEqualsExpression))
                {
                    var equalsExpression = (BinaryExpressionSyntax)parent;

                    if (equalsExpression.Left == invocationExpression)
                    {
                        if (equalsExpression.Right.IsNumericLiteralExpression("0"))
                            isFixable = true;
                    }
                    else if (equalsExpression.Left.IsNumericLiteralExpression("0"))
                    {
                        isFixable = true;
                    }
                }
                else if (kind.Is(
                    SyntaxKind.GreaterThanExpression,
                    SyntaxKind.GreaterThanOrEqualExpression,
                    SyntaxKind.LessThanExpression,
                    SyntaxKind.LessThanOrEqualExpression))
                {
                    isFixable = true;
                }

                if (isFixable)
                    context.ReportDiagnostic(DiagnosticDescriptors.CallAnyInsteadOfCount, parent);
            }
        }
    }
}
