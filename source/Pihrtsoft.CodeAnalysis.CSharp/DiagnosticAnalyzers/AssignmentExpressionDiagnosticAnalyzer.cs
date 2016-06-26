// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AssignmentExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.SimplifyAssignmentExpression,
                    DiagnosticDescriptors.SimplifyAssignmentExpressionFadeOut);
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

            if (assignment.Left?.IsMissing == false
                && assignment.Right?.IsMissing == false
                && IsBinaryExpression(assignment.Right))
            {
                var binaryExpression = (BinaryExpressionSyntax)assignment.Right;

                if (binaryExpression.Left?.IsMissing == false
                    && binaryExpression.Right?.IsMissing == false
                    && assignment.Left.IsEquivalentTo(binaryExpression.Left, topLevel: false))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.SimplifyAssignmentExpression,
                        assignment.GetLocation());

                    context.FadeOutNode(DiagnosticDescriptors.SimplifyAssignmentExpressionFadeOut, binaryExpression.Left);
                }
            }
        }

        private static bool IsBinaryExpression(ExpressionSyntax expression)
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
    }
}
