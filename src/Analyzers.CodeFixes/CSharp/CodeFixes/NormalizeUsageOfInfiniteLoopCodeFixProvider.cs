// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
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
using Roslynator.CodeFixes;
using Roslynator.CSharp.CodeStyle;
using Roslynator.CSharp.Refactorings;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NormalizeUsageOfInfiniteLoopCodeFixProvider))]
    [Shared]
    public sealed class NormalizeUsageOfInfiniteLoopCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.NormalizeUsageOfInfiniteLoop); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out StatementSyntax statement))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            if (statement.IsKind(SyntaxKind.ForStatement))
            {
                var forStatement = (ForStatementSyntax)statement;

                CodeAction codeAction = CodeAction.Create(
                    "Convert to 'while'",
                    ct => ConvertForToWhileAsync(document, forStatement, ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else if (statement.IsKind(SyntaxKind.WhileStatement))
            {
                var whileStatement = (WhileStatementSyntax)statement;

                CodeAction codeAction = CodeAction.Create(
                    "Convert to 'for'",
                    ct =>
                    {
                        ForStatementSyntax forStatement = SyntaxRefactorings.ConvertWhileStatementToForStatement(whileStatement)
                            .WithFormatterAnnotation();

                        return document.ReplaceNodeAsync(whileStatement, forStatement, ct);
                    },
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else if (statement.IsKind(SyntaxKind.DoStatement))
            {
                var doStatement = (DoStatementSyntax)statement;

                if (document.GetConfigOptions(statement.SyntaxTree).GetInfiniteLoopStyle() == InfiniteLoopStyle.ForStatement)
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Convert to 'for'",
                        ct => ConvertDoToForAsync(document, doStatement, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                }
                else
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Convert to 'while'",
                        ct => ConvertDoToWhileRefactoring.RefactorAsync(document, doStatement, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                }
            }
        }

        private static Task<Document> ConvertForToWhileAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken)
        {
            LiteralExpressionSyntax trueLiteral = TrueLiteralExpression();

            TextSpan span = TextSpan.FromBounds(
                forStatement.OpenParenToken.FullSpan.End,
                forStatement.CloseParenToken.FullSpan.Start);

            IEnumerable<SyntaxTrivia> trivia = forStatement.DescendantTrivia(span);

            if (!trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                trueLiteral = trueLiteral.WithTrailingTrivia(trivia);

            WhileStatementSyntax whileStatement = WhileStatement(
                Token(SyntaxKind.WhileKeyword).WithTriviaFrom(forStatement.ForKeyword),
                forStatement.OpenParenToken,
                trueLiteral,
                forStatement.CloseParenToken,
                forStatement.Statement);

            whileStatement = whileStatement
                .WithTriviaFrom(forStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(forStatement, whileStatement, cancellationToken);
        }

        private static Task<Document> ConvertDoToForAsync(
            Document document,
            DoStatementSyntax doStatement,
            CancellationToken cancellationToken)
        {
            ForStatementSyntax forStatement = ForStatement(
                forKeyword: Token(SyntaxKind.ForKeyword).WithTriviaFrom(doStatement.WhileKeyword),
                openParenToken: Token(doStatement.OpenParenToken.LeadingTrivia, SyntaxKind.OpenParenToken, default),
                declaration: default,
                initializers: default,
                firstSemicolonToken: SemicolonToken(),
                condition: default,
                secondSemicolonToken: SemicolonToken(),
                incrementors: default,
                closeParenToken: Token(default, SyntaxKind.CloseParenToken, doStatement.CloseParenToken.TrailingTrivia),
                statement: doStatement.Statement);

            return document.ReplaceNodeAsync(doStatement, forStatement, cancellationToken);
        }
    }
}
