// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRedundantBooleanLiteralCodeFixProvider))]
    [Shared]
    public class RemoveRedundantBooleanLiteralCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveRedundantBooleanLiteral); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(f => f.IsKind(
                    SyntaxKind.TrueLiteralExpression,
                    SyntaxKind.EqualsExpression,
                    SyntaxKind.NotEqualsExpression,
                    SyntaxKind.LogicalAndExpression,
                    SyntaxKind.LogicalOrExpression));

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            switch (node.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                    {
                        RegisterCodeFix(
                            context,
                            node.ToString(),
                            cancellationToken =>
                            {
                                return RemoveRedundantBooleanLiteralRefactoring.RefactorAsync(
                                    context.Document,
                                    (ForStatementSyntax)node.Parent,
                                    cancellationToken);
                            });

                        break;
                    }
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalOrExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)node;

                        TextSpan span = RemoveRedundantBooleanLiteralRefactoring.GetSpanToRemove(binaryExpression, binaryExpression.Left, binaryExpression.Right);

                        RegisterCodeFix(
                            context,
                            binaryExpression.ToString(span),
                            cancellationToken =>
                            {
                                return RemoveRedundantBooleanLiteralRefactoring.RefactorAsync(
                                    context.Document,
                                    binaryExpression,
                                    cancellationToken);
                            });

                        break;
                    }
            }
        }

        private static string GetTextToRemove(BinaryExpressionSyntax binaryExpression)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;
            SyntaxToken operatorToken = binaryExpression.OperatorToken;

            if (left.IsBooleanLiteralExpression())
            {
                return binaryExpression.ToString(TextSpan.FromBounds(left.SpanStart, operatorToken.Span.End));
            }
            else if (right.IsBooleanLiteralExpression())
            {
                return binaryExpression.ToString(TextSpan.FromBounds(operatorToken.SpanStart, right.Span.End));
            }

            Debug.Assert(false, binaryExpression.ToString());

            return "";
        }

        private static void RegisterCodeFix(CodeFixContext context, string textToRemove, Func<CancellationToken, Task<Document>> createChangedDocument)
        {
            CodeAction codeAction = CodeAction.Create(
                $"Remove redundant '{textToRemove}'",
                createChangedDocument,
                DiagnosticIdentifiers.RemoveRedundantBooleanLiteral + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}