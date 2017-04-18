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
    public class DoStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AvoidUsageOfDoStatementToCreateInfiniteLoop,
                    DiagnosticDescriptors.AddEmptyLineAfterLastStatementInDoStatement);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeDoStatement(f), SyntaxKind.DoStatement);
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;

            if (ReplaceDoStatementWithWhileStatementRefactoring.CanRefactor(doStatement))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AvoidUsageOfDoStatementToCreateInfiniteLoop,
                    doStatement.DoKeyword);
            }

            AddEmptyLineAfterLastStatementInDoStatementRefactoring.Analyze(context, doStatement);
        }
    }
}
