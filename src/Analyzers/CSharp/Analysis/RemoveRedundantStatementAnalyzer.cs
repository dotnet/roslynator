// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis.RemoveRedundantStatement;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantStatementAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantStatement); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeContinueStatement, SyntaxKind.ContinueStatement);
            context.RegisterSyntaxNodeAction(AnalyzeReturnStatement, SyntaxKind.ReturnStatement);
            context.RegisterSyntaxNodeAction(AnalyzeYieldBreakStatement, SyntaxKind.YieldBreakStatement);
        }

        private static void AnalyzeContinueStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var continueStatement = (ContinueStatementSyntax)context.Node;

            if (!RemoveRedundantStatementAnalysis.IsFixable(continueStatement))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantStatement, continueStatement);
        }

        private static void AnalyzeReturnStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var returnStatement = (ReturnStatementSyntax)context.Node;

            if (!RemoveRedundantStatementAnalysis.IsFixable(returnStatement))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantStatement, returnStatement);
        }

        private static void AnalyzeYieldBreakStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var yieldBreakStatement = (YieldStatementSyntax)context.Node;

            if (!RemoveRedundantStatementAnalysis.IsFixable(yieldBreakStatement))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantStatement, yieldBreakStatement);
        }
    }
}
