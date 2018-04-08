// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MergeIfStatementWithNestedIfStatementAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.MergeIfStatementWithNestedIfStatement,
                    DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        public static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            if (ifStatement.SpanContainsDirectives())
                return;

            SimpleIfStatementInfo simpleIf = SyntaxInfo.SimpleIfStatementInfo(ifStatement);

            if (!simpleIf.Success)
                return;

            if (simpleIf.Condition.Kind() == SyntaxKind.LogicalOrExpression)
                return;

            SimpleIfStatementInfo nestedIf = SyntaxInfo.SimpleIfStatementInfo(GetNestedIfStatement(ifStatement));

            if (!nestedIf.Success)
                return;

            if (nestedIf.Condition.Kind() == SyntaxKind.LogicalOrExpression)
                return;

            if (!CheckTrivia(ifStatement, nestedIf.IfStatement))
                return;

            ReportDiagnostic(context, ifStatement, nestedIf.IfStatement);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement, IfStatementSyntax nestedIf)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatement, ifStatement);

            context.ReportToken(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, nestedIf.IfKeyword);
            context.ReportToken(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, nestedIf.OpenParenToken);
            context.ReportToken(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, nestedIf.CloseParenToken);

            if (ifStatement.Statement.IsKind(SyntaxKind.Block)
                && nestedIf.Statement.IsKind(SyntaxKind.Block))
            {
                context.ReportBraces(DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut, (BlockSyntax)nestedIf.Statement);
            }
        }

        private static bool CheckTrivia(IfStatementSyntax ifStatement, IfStatementSyntax nestedIf)
        {
            TextSpan span = TextSpan.FromBounds(
                nestedIf.FullSpan.Start,
                nestedIf.CloseParenToken.FullSpan.End);

            if (nestedIf.DescendantTrivia(span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                if (ifStatement.Statement.IsKind(SyntaxKind.Block)
                    && nestedIf.Statement.IsKind(SyntaxKind.Block))
                {
                    var block = (BlockSyntax)nestedIf.Statement;

                    return block.OpenBraceToken.LeadingTrivia.IsEmptyOrWhitespace()
                        && block.OpenBraceToken.TrailingTrivia.IsEmptyOrWhitespace()
                        && block.CloseBraceToken.LeadingTrivia.IsEmptyOrWhitespace()
                        && block.CloseBraceToken.TrailingTrivia.IsEmptyOrWhitespace();
                }

                return true;
            }

            return false;
        }

        internal static IfStatementSyntax GetNestedIfStatement(IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ifStatement.Statement;

            switch (statement?.Kind())
            {
                case SyntaxKind.Block:
                    return ((BlockSyntax)statement).Statements.SingleOrDefault(shouldThrow: false) as IfStatementSyntax;
                case SyntaxKind.IfStatement:
                    return (IfStatementSyntax)statement;
            }

            return null;
        }
    }
}
