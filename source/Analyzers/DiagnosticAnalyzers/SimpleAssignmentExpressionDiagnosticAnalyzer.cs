// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimpleAssignmentExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.SimplifyAssignmentExpression,
                    DiagnosticDescriptors.SimplifyAssignmentExpressionFadeOut,
                    DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignment);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSimpleAssignment(f), SyntaxKind.SimpleAssignmentExpression);
        }

        private void AnalyzeSimpleAssignment(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var assignment = (AssignmentExpressionSyntax)context.Node;

            ExpressionSyntax left = assignment.Left;
            ExpressionSyntax right = assignment.Right;

            if (left?.IsMissing == false
                && right?.IsMissing == false)
            {
                if (SupportsCompoundAssignment(right))
                {
                    var binaryExpression = (BinaryExpressionSyntax)right;
                    ExpressionSyntax binaryLeft = binaryExpression.Left;
                    ExpressionSyntax binaryRight = binaryExpression.Right;

                    if (binaryLeft?.IsMissing == false
                        && binaryRight?.IsMissing == false
                        && left.IsEquivalentTo(binaryLeft, topLevel: false)
                        && ContainsOnlyWhitespaceOrEndOfLineTrivia(assignment))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.SimplifyAssignmentExpression,
                            assignment.GetLocation());

                        context.FadeOutNode(DiagnosticDescriptors.SimplifyAssignmentExpressionFadeOut, binaryLeft);
                    }
                }

                if (right.IsKind(SyntaxKind.AddExpression, SyntaxKind.SubtractExpression))
                {
                    var binaryExpression = (BinaryExpressionSyntax)right;
                    ExpressionSyntax binaryLeft = binaryExpression.Left;
                    ExpressionSyntax binaryRight = binaryExpression.Right;

                    if (binaryLeft?.IsMissing == false
                        && binaryRight?.IsNumericLiteralExpression(1) == true)
                    {
                        ITypeSymbol typeSymbol = context.SemanticModel.GetTypeInfo(left, context.CancellationToken).Type;

                        if (typeSymbol?.SupportsPrefixOrPostfixUnaryOperator() == true
                            && left.IsEquivalentTo(binaryLeft, topLevel: false)
                            && !assignment.SpanContainsDirectives())
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignment,
                                assignment.GetLocation(),
                                UsePostfixUnaryOperatorInsteadOfAssignmentRefactoring.GetOperatorText(assignment));
                        }
                    }
                }
            }
        }

        private static bool SupportsCompoundAssignment(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                    return true;
                default:
                    return false;
            }
        }

        private static bool ContainsOnlyWhitespaceOrEndOfLineTrivia(AssignmentExpressionSyntax assignment)
        {
            return assignment
                .DescendantTrivia(assignment.Span)
                .All(f => f.IsWhitespaceOrEndOfLineTrivia());
        }
    }
}
