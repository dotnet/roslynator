// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BitwiseOperatorDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.BitwiseOperationOnEnumWithoutFlagsAttribute); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeBinaryExpression,
                SyntaxKind.BitwiseAndExpression,
                SyntaxKind.BitwiseOrExpression,
                SyntaxKind.ExclusiveOrExpression);

            context.RegisterSyntaxNodeAction(AnalyzePrefixUnaryExpression,
                SyntaxKind.BitwiseNotExpression);
        }

        private void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            if (IsEnumWithoutFlags(context, binaryExpression.Left)
                || IsEnumWithoutFlags(context, binaryExpression.Right))
            {
                ReportDiagnostic(context, binaryExpression);
            }
        }

        private void AnalyzePrefixUnaryExpression(SyntaxNodeAnalysisContext context)
        {
            var prefixUnaryExpression = (PrefixUnaryExpressionSyntax)context.Node;

            if (IsEnumWithoutFlags(context, prefixUnaryExpression.Operand))
                ReportDiagnostic(context, prefixUnaryExpression);
        }

        private static bool IsEnumWithoutFlags(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            if (expression?.IsMissing == false)
            {
                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

                return typeSymbol?.IsEnum() == true
                    && !typeSymbol.HasAttribute(context.SemanticModel.GetTypeByMetadataName(MetadataNames.System_FlagsAttribute));
            }

            return false;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.BitwiseOperationOnEnumWithoutFlagsAttribute,
                expression);
        }
    }
}
