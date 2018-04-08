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
    public class AvoidChainOfAssignmentsAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AvoidChainOfAssignments); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeAssignment, CSharpFacts.AssignmentExpressionKinds);
            context.RegisterSyntaxNodeAction(AnalyzeEqualsValueClause, SyntaxKind.EqualsValueClause);
        }

        private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
        {
            var assignment = (AssignmentExpressionSyntax)context.Node;

            if (assignment.Right is AssignmentExpressionSyntax
                && !(assignment.Parent is AssignmentExpressionSyntax))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.AvoidChainOfAssignments, assignment);
            }
        }

        private static void AnalyzeEqualsValueClause(SyntaxNodeAnalysisContext context)
        {
            var equalsValue = (EqualsValueClauseSyntax)context.Node;

            if (equalsValue.Value is AssignmentExpressionSyntax)
                context.ReportDiagnostic(DiagnosticDescriptors.AvoidChainOfAssignments, equalsValue);
        }
    }
}
