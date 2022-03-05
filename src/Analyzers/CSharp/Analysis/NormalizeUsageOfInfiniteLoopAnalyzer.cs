// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class NormalizeUsageOfInfiniteLoopAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.NormalizeUsageOfInfiniteLoop);

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeForStatement(f), SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeWhileStatement(f), SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeDoStatement(f), SyntaxKind.DoStatement);
        }

        private void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            InfiniteLoopStyle style = context.GetInfiniteLoopStyle();

            if (style == InfiniteLoopStyle.WhileStatement)
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
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticRules.NormalizeUsageOfInfiniteLoop,
                        forStatement.ForKeyword,
                        "while");
                }
            }
        }

        private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            InfiniteLoopStyle style = context.GetInfiniteLoopStyle();

            if (style == InfiniteLoopStyle.ForStatement)
            {
                var whileStatement = (WhileStatementSyntax)context.Node;

                if (whileStatement.Condition.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    TextSpan span = TextSpan.FromBounds(
                        whileStatement.OpenParenToken.Span.End,
                        whileStatement.CloseParenToken.SpanStart);

                    if (whileStatement
                        .DescendantTrivia(span)
                        .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        DiagnosticHelpers.ReportDiagnostic(
                            context,
                            DiagnosticRules.NormalizeUsageOfInfiniteLoop,
                            whileStatement.WhileKeyword,
                            "for");
                    }
                }
            }
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;

            if (doStatement.Condition.IsKind(SyntaxKind.TrueLiteralExpression))
            {
                InfiniteLoopStyle style = context.GetInfiniteLoopStyle();

                if (style == InfiniteLoopStyle.ForStatement)
                {
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticRules.NormalizeUsageOfInfiniteLoop,
                        doStatement.DoKeyword,
                        "for");
                }
                else if (style == InfiniteLoopStyle.WhileStatement)
                {
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticRules.NormalizeUsageOfInfiniteLoop,
                        doStatement.DoKeyword,
                        "while");
                }
            }
        }
    }
}
