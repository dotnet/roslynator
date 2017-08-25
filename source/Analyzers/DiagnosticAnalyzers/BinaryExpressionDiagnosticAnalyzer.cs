// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BinaryExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.FormatBinaryOperatorOnNextLine,
                    DiagnosticDescriptors.AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression,
                    DiagnosticDescriptors.UseStringIsNullOrEmptyMethod);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeBinaryExpression,
                SyntaxKind.AddExpression,
                SyntaxKind.SubtractExpression,
                SyntaxKind.MultiplyExpression,
                SyntaxKind.DivideExpression,
                SyntaxKind.ModuloExpression,
                SyntaxKind.LeftShiftExpression,
                SyntaxKind.RightShiftExpression,
                SyntaxKind.LogicalOrExpression,
                SyntaxKind.LogicalAndExpression,
                SyntaxKind.BitwiseOrExpression,
                SyntaxKind.BitwiseAndExpression,
                SyntaxKind.ExclusiveOrExpression,
                SyntaxKind.EqualsExpression,
                SyntaxKind.NotEqualsExpression,
                SyntaxKind.LessThanExpression,
                SyntaxKind.LessThanOrEqualExpression,
                SyntaxKind.GreaterThanExpression,
                SyntaxKind.GreaterThanOrEqualExpression,
                SyntaxKind.IsExpression,
                SyntaxKind.AsExpression);
        }

        private void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            FormatBinaryOperatorOnNextLineRefactoring.Analyze(context, binaryExpression);

            AvoidNullLiteralExpressionOnLeftSideOfBinaryExpressionRefactoring.Analyze(context, binaryExpression);

            UseStringIsNullOrEmptyMethodRefactoring.Analyze(context, binaryExpression);
        }
    }
}
