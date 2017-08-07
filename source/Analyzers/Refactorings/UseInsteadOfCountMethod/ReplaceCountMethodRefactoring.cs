// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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

            MethodInfo methodInfo;
            if (semanticModel.TryGetExtensionMethodInfo(invocation, out methodInfo, ExtensionMethodKind.Reduced, cancellationToken)
                && methodInfo.IsLinqExtensionOfIEnumerableOfTWithoutParameters("Count"))
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
                else if (invocation.IsParentKind(
                    SyntaxKind.EqualsExpression,
                    SyntaxKind.GreaterThanExpression,
                    SyntaxKind.GreaterThanOrEqualExpression,
                    SyntaxKind.LessThanExpression,
                    SyntaxKind.LessThanOrEqualExpression))
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
                var literalExpression = (LiteralExpressionSyntax)left;

                switch (binaryExpression.Kind())
                {
                    case SyntaxKind.EqualsExpression:
                    case SyntaxKind.LessThanExpression:
                        return string.Equals(literalExpression.Token.ValueText, "0", StringComparison.Ordinal);
                    case SyntaxKind.LessThanOrEqualExpression:
                        return string.Equals(literalExpression.Token.ValueText, "1", StringComparison.Ordinal);
                }
            }
            else
            {
                ExpressionSyntax right = binaryExpression.Right;

                if (right?.IsKind(SyntaxKind.NumericLiteralExpression) == true)
                {
                    var literalExpression = (LiteralExpressionSyntax)right;

                    switch (binaryExpression.Kind())
                    {
                        case SyntaxKind.EqualsExpression:
                        case SyntaxKind.GreaterThanExpression:
                            return string.Equals(literalExpression.Token.ValueText, "0", StringComparison.Ordinal);
                        case SyntaxKind.GreaterThanOrEqualExpression:
                            return string.Equals(literalExpression.Token.ValueText, "1", StringComparison.Ordinal);
                    }
                }
            }

            return false;
        }
    }
}
