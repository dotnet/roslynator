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
    public class AvoidUsageOfForStatementToCreateInfiniteLoopAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AvoidUsageOfForStatementToCreateInfiniteLoop); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeForStatement(f), SyntaxKind.ForStatement);
        }

        private static void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            var forStatement = (ForStatementSyntax)context.Node;

            if (forStatement.Declaration == null
                && forStatement.Condition == null
                && !forStatement.Incrementors.Any()
                && !forStatement.Initializers.Any()
                && !forStatement.OpenParenToken.ContainsDirectives
                && !forStatement.FirstSemicolonToken.ContainsDirectives
                && !forStatement.SecondSemicolonToken.ContainsDirectives
                && !forStatement.CloseParenToken.ContainsDirectives)
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.AvoidUsageOfForStatementToCreateInfiniteLoop, forStatement.ForKeyword);
            }
        }
    }
}
