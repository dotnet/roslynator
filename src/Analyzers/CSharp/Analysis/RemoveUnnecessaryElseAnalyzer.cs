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
    public class RemoveUnnecessaryElseAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveUnnecessaryElse); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => Analyze(f), SyntaxKind.ElseClause);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var elseClause = (ElseClauseSyntax)context.Node;

            if (elseClause.ContainsDiagnostics)
                return;

            if (!IsFixable(elseClause))
                return;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.RemoveUnnecessaryElse, elseClause.ElseKeyword);
        }

        public static bool IsFixable(ElseClauseSyntax elseClause)
        {
            if (elseClause.Statement?.IsKind(SyntaxKind.IfStatement) != false)
                return false;

            if (!(elseClause.Parent is IfStatementSyntax ifStatement))
                return false;

            if (!ifStatement.IsTopmostIf())
                return false;

            StatementSyntax statement = ifStatement.Statement;

            if (statement is BlockSyntax block)
                statement = block.Statements.LastOrDefault();

            return statement != null
                && CSharpFacts.IsJumpStatement(statement.Kind());
        }
    }
}
