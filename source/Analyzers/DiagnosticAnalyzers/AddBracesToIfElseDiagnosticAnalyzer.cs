// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddBracesToIfElseDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddBracesToIfElse); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeElseClause, SyntaxKind.ElseClause);
        }

        private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (!((IfStatementSyntax)node).IsSimpleIf())
                Analyze(context, node);
        }

        private static void AnalyzeElseClause(SyntaxNodeAnalysisContext context)
        {
            Analyze(context, context.Node);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            StatementSyntax statement = EmbeddedStatementHelper.GetEmbeddedStatement(node, ifInsideElse: false, usingInsideUsing: false);

            if (statement != null)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddBracesToIfElse,
                    statement,
                    node.GetTitle());
            }
        }
    }
}
