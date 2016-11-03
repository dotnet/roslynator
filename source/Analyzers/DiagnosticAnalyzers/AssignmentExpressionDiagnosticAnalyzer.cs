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
    public class AssignmentExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignment); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.AddAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.SubtractAssignmentExpression);
        }

        private void AnalyzeAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var assignment = (AssignmentExpressionSyntax)context.Node;

            ExpressionSyntax left = assignment.Left;
            ExpressionSyntax right = assignment.Right;

            if (left?.IsMissing == false
                && right?.IsNumericLiteralExpression(1) == true)
            {
                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeInfo(left, context.CancellationToken).Type;

                if (typeSymbol?.SupportsPrefixOrPostfixUnaryOperator() == true
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
