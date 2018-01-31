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
    public class EmbeddedStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddBracesWhenExpressionSpansOverMultipleLines,
                    DiagnosticDescriptors.FormatEmbeddedStatementOnSeparateLine,
                    DiagnosticDescriptors.AddEmptyLineAfterEmbeddedStatement);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeCommonForEachStatement, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeCommonForEachStatement, SyntaxKind.ForEachVariableStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForStatement, SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(AnalyzeUsingStatement, SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(AnalyzeWhileStatement, SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDoStatement, SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(AnalyzeLockStatement, SyntaxKind.LockStatement);
            context.RegisterSyntaxNodeAction(AnalyzeFixedStatement, SyntaxKind.FixedStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, ifStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, ifStatement);
            AddBracesRefactoring.Analyze(context, ifStatement);
        }

        private void AnalyzeCommonForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var forEachStatement = (CommonForEachStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, forEachStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, forEachStatement);
            AddBracesRefactoring.Analyze(context, forEachStatement);
        }

        private void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            var forStatement = (ForStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, forStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, forStatement);
            AddBracesRefactoring.Analyze(context, forStatement);
        }

        private void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            var usingStatement = (UsingStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, usingStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, usingStatement);
            AddBracesRefactoring.Analyze(context, usingStatement);
        }

        private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            var whileStatement = (WhileStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, whileStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, whileStatement);
            AddBracesRefactoring.Analyze(context, whileStatement);
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, doStatement);
            AddBracesRefactoring.Analyze(context, doStatement);
        }

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            var lockStatement = (LockStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, lockStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, lockStatement);
            AddBracesRefactoring.Analyze(context, lockStatement);
        }

        private void AnalyzeFixedStatement(SyntaxNodeAnalysisContext context)
        {
            var fixedStatement = (FixedStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, fixedStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, fixedStatement);
            AddBracesRefactoring.Analyze(context, fixedStatement);
        }
    }
}
