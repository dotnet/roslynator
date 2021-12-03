// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SimplifyBooleanComparisonCodeFixProvider))]
    [Shared]
    public sealed class SimplifyBooleanComparisonCodeFixProvider : BaseCodeFixProvider
    {
        private const string Title = "Simplify boolean comparison";

        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.SimplifyBooleanComparison); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f => f is BinaryExpressionSyntax || f.IsKind(SyntaxKind.IsPatternExpression)))
            {
                return;
            }

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            if (node is BinaryExpressionSyntax binaryExpression)
            {
                CodeAction codeAction = CodeAction.Create(
                    Title,
                    ct => SimplifyBooleanComparisonAsync(document, binaryExpression, ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else
            {
                CodeAction codeAction = CodeAction.Create(
                    Title,
                    ct => SimplifyBooleanComparisonAsync(document, (IsPatternExpressionSyntax)node, ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        public static async Task<Document> SimplifyBooleanComparisonAsync(
            Document document,
            IsPatternExpressionSyntax isPattern,
            CancellationToken cancellationToken)
        {
            PatternSyntax pattern = isPattern.Pattern;

            bool isNegative;

            if (pattern is ConstantPatternSyntax constantPattern)
            {
                isNegative = constantPattern.Expression.IsKind(SyntaxKind.FalseLiteralExpression);
            }
            else
            {
                var notPattern = (UnaryPatternSyntax)pattern;

                constantPattern = (ConstantPatternSyntax)notPattern.Pattern;

                isNegative = constantPattern.Expression.IsKind(SyntaxKind.TrueLiteralExpression);
            }

            ExpressionSyntax expression = isPattern.Expression;
            SyntaxTriviaList trailing = expression.GetTrailingTrivia().EmptyIfWhitespace().AddRange(isPattern.GetTrailingTrivia());
            ExpressionSyntax newExpression = expression.WithTrailingTrivia(trailing);

            if (isNegative)
                newExpression = LogicalNotExpression(newExpression.WithoutTrivia().Parenthesize()).WithTriviaFrom(newExpression);

            return await document.ReplaceNodeAsync(isPattern, newExpression, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> SimplifyBooleanComparisonAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newNode = await CreateNewNodeAsync(document, binaryExpression, cancellationToken).ConfigureAwait(false);

            return await document.ReplaceNodeAsync(binaryExpression, newNode.WithFormatterAnnotation(), cancellationToken).ConfigureAwait(false);
        }

        private static async Task<ExpressionSyntax> CreateNewNodeAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            SyntaxTriviaList trivia = binaryExpression
                .DescendantTrivia(TextSpan.FromBounds(left.Span.End, right.SpanStart))
                .ToSyntaxTriviaList()
                .EmptyIfWhitespace();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (CSharpFacts.IsBooleanLiteralExpression(left.Kind()))
            {
                SyntaxTriviaList leadingTrivia = binaryExpression.GetLeadingTrivia().AddRange(trivia);

                if (right.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    var logicalNot = (PrefixUnaryExpressionSyntax)right;

                    ExpressionSyntax operand = logicalNot.Operand;

                    if (semanticModel.GetTypeInfo(operand, cancellationToken).ConvertedType.IsNullableOf(SpecialType.System_Boolean))
                    {
                        return binaryExpression
                            .WithLeft(SyntaxLogicalInverter.GetInstance(document).LogicallyInvert(left, semanticModel, cancellationToken))
                            .WithRight(operand.WithTriviaFrom(right));
                    }
                }

                return SyntaxLogicalInverter.GetInstance(document).LogicallyInvert(right, semanticModel, cancellationToken)
                    .WithLeadingTrivia(leadingTrivia);
            }
            else if (CSharpFacts.IsBooleanLiteralExpression(right.Kind()))
            {
                SyntaxTriviaList trailingTrivia = trivia.AddRange(binaryExpression.GetTrailingTrivia());

                if (left.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    var logicalNot = (PrefixUnaryExpressionSyntax)left;

                    ExpressionSyntax operand = logicalNot.Operand;

                    if (semanticModel.GetTypeInfo(operand, cancellationToken).ConvertedType.IsNullableOf(SpecialType.System_Boolean))
                    {
                        return binaryExpression
                            .WithLeft(operand.WithTriviaFrom(left))
                            .WithRight(SyntaxLogicalInverter.GetInstance(document).LogicallyInvert(right, semanticModel, cancellationToken));
                    }
                }

                return SyntaxLogicalInverter.GetInstance(document).LogicallyInvert(left, semanticModel, cancellationToken)
                    .WithTrailingTrivia(trailingTrivia);
            }

            throw new InvalidOperationException();
        }
    }
}
