// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConditionalExpressionCodeFixProvider))]
    [Shared]
    public class ConditionalExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.WrapConditionalExpressionConditionInParentheses,
                    DiagnosticIdentifiers.ReplaceConditionalExpressionWithCoalesceExpression);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            ConditionalExpressionSyntax node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ConditionalExpressionSyntax>();

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.WrapConditionalExpressionConditionInParentheses:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Wrap condition in parentheses",
                                cancellationToken => AddParenthesesToConditionAsync(context.Document, node, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                    case DiagnosticIdentifiers.ReplaceConditionalExpressionWithCoalesceExpression:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Replace ?: with ??",
                                cancellationToken =>
                                {
                                    return UseCoalesceExpressionInsteadOfConditionalExpressionAsync(
                                        context.Document,
                                        node,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                }
            }
        }

        private static async Task<Document> AddParenthesesToConditionAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ConditionalExpressionSyntax newNode = conditionalExpression
                .WithCondition(
                    SyntaxFactory.ParenthesizedExpression(
                        conditionalExpression.Condition.WithoutTrivia()
                    ).WithTriviaFrom(conditionalExpression.Condition)
                ).WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(conditionalExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> UseCoalesceExpressionInsteadOfConditionalExpressionAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var binaryExpression = (BinaryExpressionSyntax)conditionalExpression.Condition.UnwrapParentheses();

            ExpressionSyntax left = (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
                ? conditionalExpression.WhenFalse
                : conditionalExpression.WhenTrue;

            ExpressionSyntax right = (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
                ? conditionalExpression.WhenTrue
                : conditionalExpression.WhenFalse;

            BinaryExpressionSyntax newNode = SyntaxFactory.BinaryExpression(
                SyntaxKind.CoalesceExpression,
                left.WithoutTrivia(),
                right.WithoutTrivia());

            SyntaxNode newRoot = oldRoot.ReplaceNode(
                conditionalExpression,
                newNode.WithTriviaFrom(conditionalExpression));

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
