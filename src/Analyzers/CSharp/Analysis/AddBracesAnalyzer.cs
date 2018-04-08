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
    public class AddBracesAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddBraces); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeElseClause, SyntaxKind.ElseClause);
            context.RegisterSyntaxNodeAction(AnalyzeCommonForEachStatement, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeCommonForEachStatement, SyntaxKind.ForEachVariableStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForStatement, SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(AnalyzeUsingStatement, SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(AnalyzeWhileStatement, SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDoStatement, SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(AnalyzeLockStatement, SyntaxKind.LockStatement);
            context.RegisterSyntaxNodeAction(AnalyzeFixedStatement, SyntaxKind.FixedStatement);
        }

        private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            StatementSyntax statement = ifStatement.EmbeddedStatement();

            if (statement == null)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddBraces, statement, CSharpFacts.GetTitle(ifStatement));
        }

        private static void AnalyzeElseClause(SyntaxNodeAnalysisContext context)
        {
            var elseClause = (ElseClauseSyntax)context.Node;

            StatementSyntax statement = elseClause.EmbeddedStatement(allowIfStatement: false);

            if (statement == null)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddBraces, statement, CSharpFacts.GetTitle(elseClause));
        }

        private static void AnalyzeCommonForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var forEachStatement = (CommonForEachStatementSyntax)context.Node;

            StatementSyntax statement = forEachStatement.EmbeddedStatement();

            if (statement == null)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddBraces, statement, CSharpFacts.GetTitle(forEachStatement));
        }

        private static void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            var forStatement = (ForStatementSyntax)context.Node;

            StatementSyntax statement = forStatement.EmbeddedStatement();

            if (statement == null)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddBraces, statement, CSharpFacts.GetTitle(forStatement));
        }

        private static void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            var usingStatement = (UsingStatementSyntax)context.Node;

            StatementSyntax statement = usingStatement.EmbeddedStatement(allowUsingStatement: false);

            if (statement == null)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddBraces, statement, CSharpFacts.GetTitle(usingStatement));
        }

        private static void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            var whileStatement = (WhileStatementSyntax)context.Node;

            StatementSyntax statement = whileStatement.EmbeddedStatement();

            if (statement == null)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddBraces, statement, CSharpFacts.GetTitle(whileStatement));
        }

        private static void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;

            StatementSyntax statement = doStatement.EmbeddedStatement();

            if (statement == null)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddBraces, statement, CSharpFacts.GetTitle(doStatement));
        }

        private static void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            var lockStatement = (LockStatementSyntax)context.Node;

            StatementSyntax statement = lockStatement.EmbeddedStatement();

            if (statement == null)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddBraces, statement, CSharpFacts.GetTitle(lockStatement));
        }

        private static void AnalyzeFixedStatement(SyntaxNodeAnalysisContext context)
        {
            var fixedStatement = (FixedStatementSyntax)context.Node;

            StatementSyntax statement = fixedStatement.EmbeddedStatement();

            if (statement == null)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddBraces, statement, CSharpFacts.GetTitle(fixedStatement));
        }
    }
}
