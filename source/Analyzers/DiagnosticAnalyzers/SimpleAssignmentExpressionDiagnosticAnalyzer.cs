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
    public class SimpleAssignmentExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.SimplifyAssignmentExpression,
                    DiagnosticDescriptors.SimplifyAssignmentExpressionFadeOut,
                    DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignment,
                    DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignmentFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSimpleAssignment(f), SyntaxKind.SimpleAssignmentExpression);
        }

        private void AnalyzeSimpleAssignment(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var assignment = (AssignmentExpressionSyntax)context.Node;

            if (SimplifyAssignmentExpressionRefactoring.CanRefactor(assignment))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.SimplifyAssignmentExpression, assignment.GetLocation());
                context.FadeOutNode(DiagnosticDescriptors.SimplifyAssignmentExpressionFadeOut, ((BinaryExpressionSyntax)assignment.Right).Left);
            }

            UsePostfixUnaryOperatorInsteadOfAssignmentRefactoring.Analyze(context, assignment);
        }
    }
}
