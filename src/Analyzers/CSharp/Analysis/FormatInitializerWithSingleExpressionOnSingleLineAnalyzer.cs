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
    public class FormatInitializerWithSingleExpressionOnSingleLineAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.FormatInitializerWithSingleExpressionOnSingleLine); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(
                AnalyzeInitializerExpression,
                SyntaxKind.ArrayInitializerExpression,
                SyntaxKind.ObjectInitializerExpression,
                SyntaxKind.CollectionInitializerExpression);
        }

        public static void AnalyzeInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var initializer = (InitializerExpressionSyntax)context.Node;

            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            ExpressionSyntax expression = expressions.SingleOrDefault(shouldThrow: false);

            if (expression == null)
                return;

            if (initializer.SpanContainsDirectives())
                return;

            if (!expression.IsSingleLine())
                return;

            if (initializer.IsSingleLine())
                return;

            if (!initializer
                .DescendantTrivia(TextSpan.FromBounds(initializer.FullSpan.Start, initializer.Span.End))
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                return;
            }

            context.ReportDiagnostic(DiagnosticDescriptors.FormatInitializerWithSingleExpressionOnSingleLine, initializer);
        }
    }
}
