// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValueTypeObjectIsNeverEqualToNullAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ValueTypeObjectIsNeverEqualToNull); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeEqualsExpression, SyntaxKind.EqualsExpression);
            context.RegisterSyntaxNodeAction(AnalyzeNotEqualsExpression, SyntaxKind.NotEqualsExpression);
        }

        internal static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            Analyze(context, (BinaryExpressionSyntax)context.Node);
        }

        internal static void AnalyzeNotEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            Analyze(context, (BinaryExpressionSyntax)context.Node);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binaryExpression)
        {
            ExpressionSyntax left = binaryExpression.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = binaryExpression.Right;

                if (right?.Kind() == SyntaxKind.NullLiteralExpression
                    && IsStructButNotNullableOfT(context.SemanticModel.GetTypeSymbol(left, context.CancellationToken))
                    && !binaryExpression.SpanContainsDirectives())
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.ValueTypeObjectIsNeverEqualToNull,
                        binaryExpression);
                }
            }
        }

        private static bool IsStructButNotNullableOfT(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol?.TypeKind)
            {
                case TypeKind.Struct:
                    return !typeSymbol.IsNullableType();
                case TypeKind.TypeParameter:
                    return ((ITypeParameterSymbol)typeSymbol).HasValueTypeConstraint;
                default:
                    return false;
            }
        }
    }
}
