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
    public class EqualsExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteral,
                    DiagnosticDescriptors.UseLogicalNotOperator);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeEqualsExpression(f),
                SyntaxKind.EqualsExpression,
                SyntaxKind.NotEqualsExpression);
        }

        private void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            if (binaryExpression.Left == null || binaryExpression.Right == null)
                return;

            SyntaxKind kind = (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
                ? SyntaxKind.TrueLiteralExpression
                : SyntaxKind.FalseLiteralExpression;

            if (binaryExpression.Left.IsKind(kind)
                && IsBooleanExpression(context, binaryExpression.Right))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteral,
                    binaryExpression.Left.GetLocation());
            }

            if (binaryExpression.Right.IsKind(kind)
                && IsBooleanExpression(context, binaryExpression.Left))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteral,
                    binaryExpression.Right.GetLocation());
            }

            kind = (kind == SyntaxKind.TrueLiteralExpression)
                ? SyntaxKind.FalseLiteralExpression
                : SyntaxKind.TrueLiteralExpression;

            if (binaryExpression.Left.IsKind(kind)
                && IsBooleanExpression(context, binaryExpression.Right))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseLogicalNotOperator,
                    binaryExpression.Left.GetLocation());
            }

            if (binaryExpression.Right.IsKind(kind)
                && IsBooleanExpression(context, binaryExpression.Left))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseLogicalNotOperator,
                    binaryExpression.Right.GetLocation());
            }
        }

        private static bool IsBooleanExpression(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).ConvertedType;

            return typeSymbol?.SpecialType == SpecialType.System_Boolean;
        }
    }
}
