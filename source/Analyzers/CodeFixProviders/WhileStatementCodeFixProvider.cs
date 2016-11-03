// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WhileStatementCodeFixProvider))]
    [Shared]
    public class WhileStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.AvoidUsageOfWhileStatementToCreateInfiniteLoop);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            WhileStatementSyntax whileStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<WhileStatementSyntax>();

            if (whileStatement == null)
                return;

            TextSpan span = TextSpan.FromBounds(
                whileStatement.OpenParenToken.Span.End,
                whileStatement.CloseParenToken.Span.Start);

            if (root
                .DescendantTrivia(span)
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                CodeAction codeAction = CodeAction.Create(
                    "Use for statement to create an infinite loop",
                    cancellationToken =>
                    {
                        return ConvertWhileStatementToForStatementAsync(
                            context.Document,
                            whileStatement,
                            cancellationToken);
                    },
                    DiagnosticIdentifiers.AvoidUsageOfWhileStatementToCreateInfiniteLoop + EquivalenceKeySuffix);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }

        private static async Task<Document> ConvertWhileStatementToForStatementAsync(
            Document document,
            WhileStatementSyntax whileStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ForStatementSyntax newNode = ForStatement(
                Token(SyntaxKind.ForKeyword)
                    .WithTriviaFrom(whileStatement.WhileKeyword),
                Token(
                    whileStatement.OpenParenToken.LeadingTrivia,
                    SyntaxKind.OpenParenToken,
                    default(SyntaxTriviaList)),
                null,
                default(SeparatedSyntaxList<ExpressionSyntax>),
                Token(SyntaxKind.SemicolonToken),
                null,
                Token(SyntaxKind.SemicolonToken),
                default(SeparatedSyntaxList<ExpressionSyntax>),
                Token(
                    default(SyntaxTriviaList),
                    SyntaxKind.CloseParenToken,
                    whileStatement.CloseParenToken.TrailingTrivia),
                whileStatement.Statement);

            newNode = newNode
                .WithTriviaFrom(whileStatement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(whileStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
