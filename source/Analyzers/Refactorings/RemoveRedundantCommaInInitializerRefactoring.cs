// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantCommaInInitializerRefactoring
    {
        public static void AnalyzeInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var initializer = (InitializerExpressionSyntax)context.Node;

            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            if (expressions.Any()
                && expressions.Count == expressions.SeparatorCount)
            {
                SyntaxToken token = expressions.GetSeparators().Last();

                if (!token.IsMissing)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveRedundantCommaInInitializer,
                        token);
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken)
        {
            SyntaxToken lastComma = initializer.Expressions.GetSeparators().Last();

            SyntaxTriviaList newTrailingTrivia = initializer.Expressions.Last().GetTrailingTrivia()
                .AddRange(lastComma.LeadingTrivia)
                .AddRange(lastComma.TrailingTrivia);

            SeparatedSyntaxList<ExpressionSyntax> newExpressions = initializer
                .Expressions
                .ReplaceSeparator(
                    lastComma,
                    SyntaxFactory.MissingToken(SyntaxKind.CommaToken));

            ExpressionSyntax lastExpression = newExpressions.Last();

            newExpressions = newExpressions
                .Replace(lastExpression, lastExpression.WithTrailingTrivia(newTrailingTrivia));

            InitializerExpressionSyntax newInitializer = initializer
                .WithExpressions(newExpressions);

            return document.ReplaceNodeAsync(initializer, newInitializer, cancellationToken);
        }
    }
}
