// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseStringLengthInsteadOfComparisonWithEmptyStringAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseStringLengthInsteadOfComparisonWithEmptyString); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeEqualsExpression, SyntaxKind.EqualsExpression);
        }

        public static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            var equalsExpression = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax left = equalsExpression.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = equalsExpression.Right;

                if (right?.IsMissing == false)
                {
                    SemanticModel semanticModel = context.SemanticModel;
                    CancellationToken cancellationToken = context.CancellationToken;

                    if (CSharpUtility.IsEmptyStringExpression(left, semanticModel, cancellationToken))
                    {
                        if (CSharpUtility.IsStringExpression(right, semanticModel, cancellationToken))
                            ReportDiagnostic(context, equalsExpression);
                    }
                    else if (CSharpUtility.IsEmptyStringExpression(right, semanticModel, cancellationToken)
                        && CSharpUtility.IsStringExpression(left, semanticModel, cancellationToken))
                    {
                        ReportDiagnostic(context, equalsExpression);
                    }
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if (!node.SpanContainsDirectives())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseStringLengthInsteadOfComparisonWithEmptyString,
                    node);
            }
        }
    }
}
