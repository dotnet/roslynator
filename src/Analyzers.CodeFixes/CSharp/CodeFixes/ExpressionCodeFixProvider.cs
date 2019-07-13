// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExpressionCodeFixProvider))]
    [Shared]
    public class ExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.ExpressionIsAlwaysEqualToTrueOrFalse); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ExpressionSyntax expression))
                return;

            Document document = context.Document;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.ExpressionIsAlwaysEqualToTrueOrFalse:
                        {
                            if (expression.IsKind(
                                SyntaxKind.LessThanExpression,
                                SyntaxKind.LessThanOrEqualExpression,
                                SyntaxKind.GreaterThanExpression,
                                SyntaxKind.GreaterThanOrEqualExpression))
                            {
                                var binaryExpression = (BinaryExpressionSyntax)expression;

                                LiteralExpressionSyntax newNode = CSharpFactory.BooleanLiteralExpression(binaryExpression.IsKind(SyntaxKind.GreaterThanOrEqualExpression, SyntaxKind.LessThanOrEqualExpression));

                                CodeAction codeAction = CodeAction.Create(
                                    $"Replace expression with '{newNode}'",
                                    ct => document.ReplaceNodeAsync(binaryExpression, newNode.WithTriviaFrom(binaryExpression), ct),
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }
                            else
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Remove null check",
                                    ct => RemoveUnnecessaryNullCheckAsync(document, expression, ct),
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                }
            }
        }

        private static Task<Document> RemoveUnnecessaryNullCheckAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            expression = expression.WalkUpParentheses();

            var binaryExpression = (BinaryExpressionSyntax)expression.Parent;

            ExpressionSyntax newExpression = binaryExpression.Right.WithLeadingTrivia(expression.GetLeadingTrivia());

            return document.ReplaceNodeAsync(binaryExpression, newExpression, cancellationToken);
        }
    }
}