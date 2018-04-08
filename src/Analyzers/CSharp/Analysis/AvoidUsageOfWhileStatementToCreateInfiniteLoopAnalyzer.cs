// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsageOfWhileStatementToCreateInfiniteLoopAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AvoidUsageOfWhileStatementToCreateInfiniteLoop); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeWhileStatement, SyntaxKind.WhileStatement);
        }

        public static void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            var whileStatement = (WhileStatementSyntax)context.Node;

            if (whileStatement.Condition?.Kind() != SyntaxKind.TrueLiteralExpression)
                return;

            TextSpan span = TextSpan.FromBounds(
                whileStatement.OpenParenToken.Span.End,
                whileStatement.CloseParenToken.SpanStart);

            if (!whileStatement
                .DescendantTrivia(span)
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                return;
            }

            context.ReportDiagnostic(DiagnosticDescriptors.AvoidUsageOfWhileStatementToCreateInfiniteLoop, whileStatement.WhileKeyword);
        }
    }
}
