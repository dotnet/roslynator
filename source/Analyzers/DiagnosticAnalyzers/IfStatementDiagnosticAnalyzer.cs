// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Refactorings.If;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
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

            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            foreach (IfRefactoring refactoring in IfRefactoring.Analyze(ifStatement, AnalysisOptions, context.SemanticModel, context.CancellationToken))
            {
                Debug.Assert(refactoring.Kind == RefactoringKind.IfElseToAssignmentWithCoalesceExpression
                    || refactoring.Kind == RefactoringKind.IfElseToAssignmentWithExpression
                    || refactoring.Kind == RefactoringKind.IfElseToReturnWithCoalesceExpression
                    || refactoring.Kind == RefactoringKind.IfElseToYieldReturnWithCoalesceExpression
                    || refactoring.Kind == RefactoringKind.IfReturnToReturnWithCoalesceExpression
                    || refactoring.Kind == RefactoringKind.IfElseToReturnWithExpression
                    || refactoring.Kind == RefactoringKind.IfElseToYieldReturnWithExpression
                    || refactoring.Kind == RefactoringKind.IfReturnToReturnWithExpression, refactoring.Kind.ToString());

                switch (refactoring.Kind)
                {
                    case RefactoringKind.IfElseToAssignmentWithCoalesceExpression:
                    case RefactoringKind.IfElseToReturnWithCoalesceExpression:
                    case RefactoringKind.IfElseToYieldReturnWithCoalesceExpression:
                    case RefactoringKind.IfReturnToReturnWithCoalesceExpression:
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.UseCoalesceExpressionInsteadOfIf, ifStatement);
                            break;
                        }
                    case RefactoringKind.IfElseToReturnWithExpression:
                    case RefactoringKind.IfElseToYieldReturnWithExpression:
                    case RefactoringKind.IfReturnToReturnWithExpression:
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.ReplaceIfStatementWithReturnStatement, ifStatement);
                            break;
                        }
                    case RefactoringKind.IfElseToAssignmentWithExpression:
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.ReplaceIfStatementWithAssignment, ifStatement);
                            break;
                        }
                }
            }
        }
    }
}
