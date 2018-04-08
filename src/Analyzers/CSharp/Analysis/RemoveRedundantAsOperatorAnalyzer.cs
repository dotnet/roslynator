// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantAsOperatorAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantAsOperator); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeAsExpression, SyntaxKind.AsExpression);
        }

        public static void AnalyzeAsExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            AsExpressionInfo info = SyntaxInfo.AsExpressionInfo(binaryExpression);

            if (!info.Success)
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(info.Type, context.CancellationToken);

            if (typeSymbol?.IsErrorType() != false)
                return;

            Conversion conversion = context.SemanticModel.ClassifyConversion(info.Expression, typeSymbol);

            if (!conversion.IsIdentity)
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.RemoveRedundantAsOperator,
                Location.Create(binaryExpression.SyntaxTree, TextSpan.FromBounds(binaryExpression.OperatorToken.SpanStart, info.Type.Span.End)));
        }
    }
}
