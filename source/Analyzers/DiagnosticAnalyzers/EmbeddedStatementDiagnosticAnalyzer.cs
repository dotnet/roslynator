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
                    DiagnosticDescriptors.AddBraces,
                    DiagnosticDescriptors.FormatEmbeddedStatementOnSeparateLine,
                    DiagnosticDescriptors.AddEmptyLineAfterEmbeddedStatement);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeIfStatement(f), SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeForEachStatement(f), SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeForStatement(f), SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeUsingStatement(f), SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeWhileStatement(f), SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeDoStatement(f), SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeLockStatement(f), SyntaxKind.LockStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeFixedStatement(f), SyntaxKind.FixedStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var ifStatement = (IfStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, ifStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, ifStatement);
            AddBracesRefactoring.Analyze(context, ifStatement);
        }

        private void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var forEachStatement = (ForEachStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, forEachStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, forEachStatement);
            AddBracesRefactoring.Analyze(context, forEachStatement);
        }

        private void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var forStatement = (ForStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, forStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, forStatement);
            AddBracesRefactoring.Analyze(context, forStatement);
        }

        private void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var usingStatement = (UsingStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, usingStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, usingStatement);
            AddBracesRefactoring.Analyze(context, usingStatement);
        }

        private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var whileStatement = (WhileStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, whileStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, whileStatement);
            AddBracesRefactoring.Analyze(context, whileStatement);
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var doStatement = (DoStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, doStatement);
            AddBracesRefactoring.Analyze(context, doStatement);
        }

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var lockStatement = (LockStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, lockStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, lockStatement);
            AddBracesRefactoring.Analyze(context, lockStatement);
        }

        private void AnalyzeFixedStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var fixedStatement = (FixedStatementSyntax)context.Node;

            FormatEmbeddedStatementOnSeparateLineRefactoring.Analyze(context, fixedStatement);
            AddEmptyLineAfterEmbeddedStatementRefactoring.Analyze(context, fixedStatement);
            AddBracesRefactoring.Analyze(context, fixedStatement);
        }
    }
}
