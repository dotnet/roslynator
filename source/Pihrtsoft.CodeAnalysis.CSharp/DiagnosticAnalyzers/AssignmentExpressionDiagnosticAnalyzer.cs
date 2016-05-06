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

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.SimpleAssignmentExpression);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var assignment = (AssignmentExpressionSyntax)context.Node;

            if (assignment.Left != null
                && assignment.Right != null
                && IsBinaryExpression(assignment.Right))
            {
                var binaryExpression = (BinaryExpressionSyntax)assignment.Right;

                if (binaryExpression.Left != null)
                {
                    ISymbol symbol = context.SemanticModel.GetSymbolInfo(assignment.Left, context.CancellationToken).Symbol;

                    if (symbol != null)
                    {
                        ISymbol symbol2 = context.SemanticModel.GetSymbolInfo(binaryExpression.Left, context.CancellationToken).Symbol;

                        if (symbol.Equals(symbol2))
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.SimplifyAssignmentExpression,
                                context.Node.GetLocation());

                            DiagnosticHelper.FadeOutNode(context, binaryExpression.Left, DiagnosticDescriptors.SimplifyAssignmentExpressionFadeOut);
                        }
                    }
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
