// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.SyntaxWalkers;

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

        private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
        {
            var catchClause = (CatchClauseSyntax)context.Node;

            if (catchClause.Filter != null)
                return;

            if (!(catchClause.Block.Statements.FirstOrDefault() is IfStatementSyntax ifStatement))
                return;

            if (IsThrowStatementWithoutExpression(ifStatement.Statement.SingleNonBlockStatementOrDefault())
                ^ IsThrowStatementWithoutExpression(ifStatement.Else?.Statement.SingleNonBlockStatementOrDefault()))
            {
                if (AwaitExpressionWalker.ContainsAwaitExpression(ifStatement.Condition))
                    return;

                if (ifStatement.ContainsUnbalancedIfElseDirectives())
                    return;

                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.UseExceptionFilter, ifStatement.IfKeyword);
            }
        }

        private static bool IsThrowStatementWithoutExpression(StatementSyntax statement)
        {
            return (statement is ThrowStatementSyntax throwStatement)
                && throwStatement.Expression == null;
        }
    }
}
