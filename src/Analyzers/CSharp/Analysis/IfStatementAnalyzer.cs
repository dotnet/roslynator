// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis.If;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfStatementAnalyzer : BaseDiagnosticAnalyzer
    {
        internal static IfAnalysisOptions AnalysisOptions { get; } = new IfAnalysisOptions(
            useCoalesceExpression: true,
            useConditionalExpression: false,
            useBooleanExpression: false,
            useExpression: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UseCoalesceExpressionInsteadOfIf,
                    DiagnosticDescriptors.ReplaceIfStatementWithReturnStatement,
                    DiagnosticDescriptors.ReplaceIfStatementWithReturnStatementFadeOut,
                    DiagnosticDescriptors.ReplaceIfStatementWithAssignment);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (startContext.AreAnalyzersSuppressed(
                    DiagnosticDescriptors.UseCoalesceExpressionInsteadOfIf,
                    DiagnosticDescriptors.ReplaceIfStatementWithReturnStatement,
                    DiagnosticDescriptors.ReplaceIfStatementWithAssignment))
                {
                    return;
                }

                startContext.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            });
        }

        private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            foreach (IfAnalysis refactoring in IfAnalysis.Analyze(ifStatement, AnalysisOptions, context.SemanticModel, context.CancellationToken))
            {
                Debug.Assert(refactoring.Kind == IfAnalysisKind.IfElseToAssignmentWithCoalesceExpression
                    || refactoring.Kind == IfAnalysisKind.IfElseToAssignmentWithExpression
                    || refactoring.Kind == IfAnalysisKind.IfElseToAssignmentWithCondition
                    || refactoring.Kind == IfAnalysisKind.IfElseToReturnWithCoalesceExpression
                    || refactoring.Kind == IfAnalysisKind.IfElseToYieldReturnWithCoalesceExpression
                    || refactoring.Kind == IfAnalysisKind.IfReturnToReturnWithCoalesceExpression
                    || refactoring.Kind == IfAnalysisKind.IfElseToReturnWithExpression
                    || refactoring.Kind == IfAnalysisKind.IfElseToYieldReturnWithExpression
                    || refactoring.Kind == IfAnalysisKind.IfReturnToReturnWithExpression, refactoring.Kind.ToString());

                switch (refactoring.Kind)
                {
                    case IfAnalysisKind.IfElseToAssignmentWithCoalesceExpression:
                    case IfAnalysisKind.IfElseToReturnWithCoalesceExpression:
                    case IfAnalysisKind.IfElseToYieldReturnWithCoalesceExpression:
                    case IfAnalysisKind.IfReturnToReturnWithCoalesceExpression:
                        {
                            if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.UseCoalesceExpressionInsteadOfIf))
                                context.ReportDiagnostic(DiagnosticDescriptors.UseCoalesceExpressionInsteadOfIf, ifStatement);

                            break;
                        }
                    case IfAnalysisKind.IfElseToReturnWithExpression:
                    case IfAnalysisKind.IfElseToYieldReturnWithExpression:
                    case IfAnalysisKind.IfReturnToReturnWithExpression:
                        {
                            if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.ReplaceIfStatementWithReturnStatement))
                                context.ReportDiagnostic(DiagnosticDescriptors.ReplaceIfStatementWithReturnStatement, ifStatement);

                            break;
                        }
                    case IfAnalysisKind.IfElseToAssignmentWithExpression:
                    case IfAnalysisKind.IfElseToAssignmentWithCondition:
                        {
                            if (!context.IsAnalyzerSuppressed(DiagnosticDescriptors.ReplaceIfStatementWithAssignment))
                                context.ReportDiagnostic(DiagnosticDescriptors.ReplaceIfStatementWithAssignment, ifStatement);

                            break;
                        }
                }
            }
        }
    }
}
