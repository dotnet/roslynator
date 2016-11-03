// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FormatBinaryOperatorOnNewLineCodeFixProvider))]
    [Shared]
    public class FormatBinaryOperatorOnNewLineCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.FormatBinaryOperatorOnNextLine);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            BinaryExpressionSyntax binaryExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<BinaryExpressionSyntax>();

            if (binaryExpression == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Format binary operator on next line",
                cancellationToken => FormatBinaryExpressionOperatorOnNewLineAsync(context.Document, binaryExpression, cancellationToken),
                DiagnosticIdentifiers.FormatBinaryOperatorOnNextLine + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        internal static async Task<Document> FormatBinaryExpressionOperatorOnNewLineAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            BinaryExpressionSyntax newBinaryExpression = GetNewBinaryExpression(binaryExpression)
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(binaryExpression, newBinaryExpression);

            return document.WithSyntaxRoot(newRoot);
        }

        public static BinaryExpressionSyntax GetNewBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression == null)
                throw new ArgumentNullException(nameof(binaryExpression));

            return BinaryExpression(
                binaryExpression.Kind(),
                binaryExpression.Left.WithTrailingTrivia(binaryExpression.OperatorToken.TrailingTrivia),
                Token(
                    binaryExpression.Right.GetLeadingTrivia(),
                    binaryExpression.OperatorToken.Kind(),
                    TriviaList(Space)),
                binaryExpression.Right.WithoutLeadingTrivia());
        }
    }
}