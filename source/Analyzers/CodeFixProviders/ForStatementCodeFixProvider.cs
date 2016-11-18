// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ForStatementCodeFixProvider))]
    [Shared]
    public class ForStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AvoidUsageOfForStatementToCreateInfiniteLoop,
                    DiagnosticIdentifiers.RemoveRedundantBooleanLiteral);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            ForStatementSyntax forStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ForStatementSyntax>();

            Debug.Assert(forStatement != null, $"{nameof(forStatement)} is null");

            if (forStatement == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.AvoidUsageOfForStatementToCreateInfiniteLoop:
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
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantBooleanLiteral:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant boolean literal",
                                cancellationToken =>
                                {
                                    return RemoveTrueLiteralFromForStatementConditionAsync(
                                        context.Document,
                                        forStatement,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static async Task<Document> ConvertForStatementToWhileStatementAsync(
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
                WhileKeyword().WithTriviaFrom(forStatement.ForKeyword),
                forStatement.OpenParenToken,
                trueLiteral,
                forStatement.CloseParenToken,
                forStatement.Statement);

            whileStatement = whileStatement
                .WithTriviaFrom(forStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(forStatement, whileStatement, cancellationToken).ConfigureAwait(false);
        }

        private async Task<Document> RemoveTrueLiteralFromForStatementConditionAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken)
        {
            ForStatementSyntax newForStatement = forStatement;

            if (forStatement
                .DescendantTrivia(TextSpan.FromBounds(forStatement.FirstSemicolonToken.Span.End, forStatement.SecondSemicolonToken.Span.Start))
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                newForStatement = forStatement.Update(
                    forStatement.ForKeyword,
                    forStatement.OpenParenToken,
                    forStatement.Declaration,
                    forStatement.Initializers,
                    forStatement.FirstSemicolonToken.WithTrailingTrivia(SpaceTrivia()),
                    default(ExpressionSyntax),
                    forStatement.SecondSemicolonToken.WithoutLeadingTrivia(),
                    forStatement.Incrementors,
                    forStatement.CloseParenToken,
                    forStatement.Statement);
            }
            else
            {
                newForStatement = forStatement.RemoveNode(forStatement.Condition, SyntaxRemoveOptions.KeepExteriorTrivia);
            }

            return await document.ReplaceNodeAsync(forStatement, newForStatement, cancellationToken).ConfigureAwait(false);
        }
    }
}
