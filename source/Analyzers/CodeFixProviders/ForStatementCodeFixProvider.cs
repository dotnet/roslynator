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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ForStatementCodeFixProvider))]
    [Shared]
    public class ForStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.AvoidUsageOfForStatementToCreateInfiniteLoop);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            ForStatementSyntax forStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ForStatementSyntax>();

            if (forStatement == null)
                return;

            TextSpan span = TextSpan.FromBounds(
                forStatement.OpenParenToken.Span.End,
                forStatement.CloseParenToken.Span.Start);

            if (forStatement
                .DescendantTrivia(span)
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                CodeAction codeAction = CodeAction.Create(
                    "Use while statement to create an infinite loop",
                    cancellationToken =>
                    {
                        return ConvertForStatementToWhileStatementAsync(
                            context.Document,
                            forStatement,
                            cancellationToken);
                    },
                    DiagnosticIdentifiers.AvoidUsageOfForStatementToCreateInfiniteLoop);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }

        private static async Task<Document> ConvertForStatementToWhileStatementAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            WhileStatementSyntax newNode = WhileStatement(
                Token(SyntaxKind.WhileKeyword)
                    .WithTriviaFrom(forStatement.ForKeyword),
                Token(
                    forStatement.OpenParenToken.LeadingTrivia,
                    SyntaxKind.OpenParenToken,
                    default(SyntaxTriviaList)),
                LiteralExpression(SyntaxKind.TrueLiteralExpression),
                Token(
                    default(SyntaxTriviaList),
                    SyntaxKind.CloseParenToken,
                    forStatement.CloseParenToken.TrailingTrivia),
                forStatement.Statement);

            newNode = newNode
                .WithTriviaFrom(forStatement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(forStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
