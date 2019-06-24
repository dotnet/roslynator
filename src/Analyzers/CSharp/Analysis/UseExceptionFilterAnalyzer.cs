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
    public class UseExceptionFilterAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseExceptionFilter); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (((CSharpCompilation)startContext.Compilation).LanguageVersion < LanguageVersion.CSharp6)
                    return;

                startContext.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
            });
        }

        public static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
        {
            var catchClause = (CatchClauseSyntax)context.Node;

            if (catchClause.Filter != null)
                return;

            if (!(catchClause.Block.Statements.FirstOrDefault() is IfStatementSyntax ifStatement))
                return;

            StatementSyntax statement = ifStatement.Statement.SingleNonBlockStatementOrDefault();

            if (IsThrowStatementWithoutExpression(statement))
                ReportDiagnostic();

            statement = ifStatement.Else?.Statement.SingleNonBlockStatementOrDefault();

            if (IsThrowStatementWithoutExpression(statement))
                ReportDiagnostic();

            void ReportDiagnostic()
            {
                if (ifStatement.ContainsUnbalancedIfElseDirectives())
                    return;

                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.UseExceptionFilter, ifStatement.IfKeyword);
            }
        }

        private static bool IsThrowStatementWithoutExpression(StatementSyntax statement)
        {
            return statement.IsKind(SyntaxKind.ThrowStatement)
                && ((ThrowStatementSyntax)statement).Expression == null;
        }
    }
}
