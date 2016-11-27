// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.ReplaceCountMethod
{
    internal static class ReplaceCountMethodRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess)
        {
            if (SemanticAnalyzer.IsEnumerableExtensionMethod(invocation, "Count", 1, context.SemanticModel, context.CancellationToken))
            {
                string propertyName = SyntaxHelper.GetCountOrLengthPropertyName(memberAccess.Expression, context.SemanticModel, allowImmutableArray: true, cancellationToken: context.CancellationToken);

                if (propertyName != null)
                {
                    TextSpan span = TextSpan.FromBounds(memberAccess.Name.Span.Start, invocation.Span.End);
                    if (invocation
                         .DescendantTrivia(span)
                         .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        Diagnostic diagnostic = Diagnostic.Create(
                            DiagnosticDescriptors.ReplaceCountMethodWithCountOrLengthProperty,
                            Location.Create(context.Node.SyntaxTree, span),
                            ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("PropertyName",  propertyName) }),
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
                                DiagnosticDescriptors.ReplaceCountMethodWithAnyMethod,
                                binaryExpression.GetLocation());
                        }
                    }
                }
            }
        }

        private static bool IsFixableBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            ExpressionSyntax left = binaryExpression.Left;

            if (left?.IsKind(SyntaxKind.NumericLiteralExpression) == true)
            {
                return ((LiteralExpressionSyntax)left).IsZeroLiteralExpression()
                    && binaryExpression.IsKind(
                        SyntaxKind.EqualsExpression,
                        SyntaxKind.LessThanExpression);
            }
            else
            {
                ExpressionSyntax right = binaryExpression.Right;

                if (right?.IsKind(SyntaxKind.NumericLiteralExpression) == true)
                {
                    return ((LiteralExpressionSyntax)right).IsZeroLiteralExpression()
                        && binaryExpression.IsKind(
                            SyntaxKind.EqualsExpression,
                            SyntaxKind.GreaterThanExpression);
                }
            }

            return false;
        }
    }
}
