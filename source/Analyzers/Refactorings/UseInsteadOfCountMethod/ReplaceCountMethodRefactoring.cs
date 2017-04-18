// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings.UseInsteadOfCountMethod
{
    internal static class UseInsteadOfCountMethodRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess)
        {
            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            if (semanticModel
                .GetExtensionMethodInfo(invocation, ExtensionMethodKind.Reduced, cancellationToken)
                .MethodInfo
                .IsLinqExtensionOfIEnumerableOfTWithoutParameters("Count"))
            {
                string propertyName = GetCountOrLengthPropertyName(memberAccess.Expression, semanticModel, cancellationToken);

                if (propertyName != null)
                {
                    TextSpan span = TextSpan.FromBounds(memberAccess.Name.Span.Start, invocation.Span.End);
                    if (invocation
                         .DescendantTrivia(span)
                         .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        Diagnostic diagnostic = Diagnostic.Create(
                            DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfCountMethod,
                            Location.Create(context.Node.SyntaxTree, span),
                            ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("PropertyName", propertyName) }),
                            propertyName);

                        context.ReportDiagnostic(diagnostic);
                    }
                }
                else if (invocation.Parent?.IsKind(
                    SyntaxKind.EqualsExpression,
                    SyntaxKind.GreaterThanExpression,
                    SyntaxKind.LessThanExpression) == true)
                {
                    var binaryExpression = (BinaryExpressionSyntax)invocation.Parent;

                    if (IsFixableBinaryExpression(binaryExpression))
                    {
                        TextSpan span = TextSpan.FromBounds(invocation.Span.End, binaryExpression.Span.End);

                        if (binaryExpression
                            .DescendantTrivia(span)
                            .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.UseAnyMethodInsteadOfCountMethod,
                                binaryExpression);
                        }
                    }
                }
            }
        }

        private static string GetCountOrLengthPropertyName(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (typeSymbol?.IsErrorType() == false
                && !typeSymbol.IsConstructedFromIEnumerableOfT())
            {
                if (typeSymbol.IsArrayType()
                    || typeSymbol.IsConstructedFromImmutableArrayOfT(semanticModel))
                {
                    return "Length";
                }

                if (typeSymbol.ImplementsICollectionOfT())
                    return "Count";
            }

            return null;
        }

        private static bool IsFixableBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            ExpressionSyntax left = binaryExpression.Left;

            if (left?.IsKind(SyntaxKind.NumericLiteralExpression) == true)
            {
                return ((LiteralExpressionSyntax)left).IsZeroNumericLiteral()
                    && binaryExpression.IsKind(
                        SyntaxKind.EqualsExpression,
                        SyntaxKind.LessThanExpression);
            }
            else
            {
                ExpressionSyntax right = binaryExpression.Right;

                if (right?.IsKind(SyntaxKind.NumericLiteralExpression) == true)
                {
                    return ((LiteralExpressionSyntax)right).IsZeroNumericLiteral()
                        && binaryExpression.IsKind(
                            SyntaxKind.EqualsExpression,
                            SyntaxKind.GreaterThanExpression);
                }
            }

            return false;
        }
    }
}
