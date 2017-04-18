// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeStringExpressionsRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, BinaryExpressionSelection binaryExpressionSelection)
        {
            if (binaryExpressionSelection.BinaryExpression.IsKind(SyntaxKind.AddExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                StringExpressionChain chain = StringExpressionChain.TryCreate(binaryExpressionSelection, semanticModel, context.CancellationToken);

                if (chain != null)
                    ComputeRefactoring(context, chain);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression.IsKind(SyntaxKind.AddExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                StringExpressionChain chain = StringExpressionChain.TryCreate(binaryExpression, semanticModel, context.CancellationToken);

                if (chain != null)
                    ComputeRefactoring(context, chain);
            }
        }

        private static void ComputeRefactoring(RefactoringContext context, StringExpressionChain chain)
        {
            if (chain.ContainsExpression)
            {
                if (chain.ContainsLiteral || chain.ContainsInterpolatedStringExpression)
                {
                    context.RegisterRefactoring(
                        "Merge string expressions",
                        cancellationToken => ToInterpolatedStringAsync(context.Document, chain, cancellationToken));
                }
            }
            else if (chain.ContainsLiteral)
            {
                context.RegisterRefactoring(
                    "Merge string literals",
                    cancellationToken => ToStringLiteralAsync(context.Document, chain, multiline: false, cancellationToken: cancellationToken));

                if (chain.OriginalExpression
                        .DescendantTrivia(chain.Span ?? chain.OriginalExpression.Span)
                        .Any(f => f.IsKind(SyntaxKind.EndOfLineTrivia)))
                {
                    context.RegisterRefactoring(
                        "Merge string literals into multiline string literal",
                        cancellationToken => ToStringLiteralAsync(context.Document, chain, multiline: true, cancellationToken: cancellationToken));
                }
            }
        }

        private static Task<Document> ToInterpolatedStringAsync(
            Document document,
            StringExpressionChain chain,
            CancellationToken cancellationToken)
        {
            BinaryExpressionSyntax binaryExpression = chain.OriginalExpression;

            SyntaxNode newNode = chain.ToInterpolatedString();

            if (chain.Span.HasValue)
            {
                TextSpan span = chain.Span.Value;

                int start = binaryExpression.SpanStart;

                string s = binaryExpression.ToString();

                s = s.Remove(span.Start - start)
                    + newNode
                    + s.Substring(span.End - start);

                newNode = SyntaxFactory.ParseExpression(s);
            }

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }

        public static Task<Document> ToStringLiteralAsync(
            Document document,
            StringExpressionChain chain,
            bool multiline,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            BinaryExpressionSyntax binaryExpression = chain.OriginalExpression;

            LiteralExpressionSyntax newNode = (multiline)
                ? chain.ToMultilineStringLiteral()
                : chain.ToStringLiteral();

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }
    }
}
