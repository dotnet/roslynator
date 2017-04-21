// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatInitializerWithSingleExpressionOnSingleLineRefactoring
    {
        public static void AnalyzeInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var initializer = (InitializerExpressionSyntax)context.Node;

            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            if (expressions.Count == 1
                && !initializer.SpanContainsDirectives()
                && expressions[0].IsSingleLine()
                && !initializer.IsSingleLine()
                && initializer
                    .DescendantTrivia(TextSpan.FromBounds(initializer.FullSpan.Start, initializer.Span.End))
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.FormatInitializerWithSingleExpressionOnSingleLine,
                    initializer);
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken)
        {
            return CSharpFormatter.ToSingleLineAsync(document, initializer, cancellationToken);
        }
    }
}
