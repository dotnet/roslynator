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
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SimplifyCodeBranchingCodeFixProvider))]
    [Shared]
    public class SimplifyCodeBranchingCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.SimplifyCodeBranching); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out IfStatementSyntax ifStatement))
                return;

            Diagnostic diagnostic = context.Diagnostics[0];

            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                "Simplify code branching",
                ct => RefactorAsync(document, ifStatement, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = ifStatement.Condition;

            ElseClauseSyntax elseClause = ifStatement.Else;

            if ((ifStatement.Statement as BlockSyntax)?.Statements.Any() == false)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                ExpressionSyntax newCondition = SyntaxInverter.LogicallyInvert(condition, semanticModel, cancellationToken);

                StatementSyntax statement = elseClause.Statement;

                if (statement is IfStatementSyntax nestedIf)
                {
                    newCondition = LogicalAndExpression(newCondition.Parenthesize(), nestedIf.Condition.Parenthesize());

                    statement = nestedIf.Statement;
                }

                cancellationToken.ThrowIfCancellationRequested();

                IfStatementSyntax newNode = ifStatement.Update(
                    ifStatement.IfKeyword,
                    ifStatement.OpenParenToken,
                    newCondition,
                    ifStatement.CloseParenToken,
                    statement,
                    default(ElseClauseSyntax));

                newNode = newNode.WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken).ConfigureAwait(false);
            }
            else if (elseClause != null)
            {
                WhileStatementSyntax whileStatement;

                if (ifStatement.Parent is BlockSyntax block)
                {
                    whileStatement = (WhileStatementSyntax)block.Parent;
                }
                else
                {
                    block = Block();
                    whileStatement = (WhileStatementSyntax)ifStatement.Parent;
                }

                cancellationToken.ThrowIfCancellationRequested();

                BlockSyntax newBlock = (ifStatement.Statement is BlockSyntax ifBlock)
                    ? block.WithStatements(ifBlock.Statements)
                    : block.WithStatements(SingletonList(ifStatement.Statement));

                SyntaxNode newNode = whileStatement.Update(
                    whileStatement.WhileKeyword,
                    whileStatement.OpenParenToken,
                    ifStatement.Condition,
                    whileStatement.CloseParenToken,
                    newBlock);

                newNode = newNode.WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(whileStatement, newNode, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                StatementSyntax statement = ifStatement.SingleNonBlockStatementOrDefault();

                if (statement.Kind() == SyntaxKind.BreakStatement)
                {
                    var block = (BlockSyntax)ifStatement.Parent;

                    SyntaxList<StatementSyntax> statements = block.Statements;

                    BlockSyntax newBlock = block.WithStatements(statements.Remove(ifStatement));

                    SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                    ExpressionSyntax newCondition = SyntaxInverter.LogicallyInvert(condition, semanticModel, cancellationToken);

                    SyntaxNode newNode = block.Parent;

                    switch (block.Parent)
                    {
                        case WhileStatementSyntax whileStatement:
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                if (statements.IsFirst(ifStatement))
                                {
                                    newNode = whileStatement.Update(
                                        whileStatement.WhileKeyword,
                                        whileStatement.OpenParenToken,
                                        newCondition,
                                        whileStatement.CloseParenToken,
                                        newBlock);
                                }
                                else
                                {
                                    newNode = DoStatement(
                                        Token(whileStatement.WhileKeyword.LeadingTrivia, SyntaxKind.DoKeyword, whileStatement.CloseParenToken.TrailingTrivia),
                                        newBlock.WithoutTrailingTrivia(),
                                        Token(SyntaxKind.WhileKeyword),
                                        OpenParenToken(),
                                        newCondition,
                                        CloseParenToken(),
                                        SemicolonToken().WithTrailingTrivia(newBlock.GetTrailingTrivia()));
                                }

                                break;
                            }
                        case DoStatementSyntax doStatement:
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                if (statements.IsLast(ifStatement))
                                {
                                    newNode = doStatement.Update(
                                        doStatement.DoKeyword,
                                        newBlock,
                                        doStatement.WhileKeyword,
                                        doStatement.OpenParenToken,
                                        newCondition,
                                        doStatement.CloseParenToken,
                                        doStatement.SemicolonToken);
                                }
                                else
                                {
                                    newNode = WhileStatement(
                                        Token(doStatement.DoKeyword.LeadingTrivia, SyntaxKind.WhileKeyword, SyntaxTriviaList.Empty),
                                        OpenParenToken(),
                                        newCondition,
                                        Token(SyntaxTriviaList.Empty, SyntaxKind.CloseParenToken, doStatement.DoKeyword.TrailingTrivia),
                                        newBlock.WithTrailingTrivia(doStatement.GetTrailingTrivia()));
                                }

                                break;
                            }
                        default:
                            {
                                Debug.Fail(block.Parent.Kind().ToString());
                                break;
                            }
                    }

                    newNode = newNode.WithFormatterAnnotation();

                    return await document.ReplaceNodeAsync(block.Parent, newNode, cancellationToken).ConfigureAwait(false);
                }
                else if (statement.Kind() == SyntaxKind.DoStatement)
                {
                    var doStatement = (DoStatementSyntax)statement;

                    WhileStatementSyntax whileStatement = WhileStatement(
                        Token(ifStatement.GetLeadingTrivia(), SyntaxKind.WhileKeyword, SyntaxTriviaList.Empty),
                        OpenParenToken(),
                        doStatement.Condition,
                        Token(SyntaxTriviaList.Empty, SyntaxKind.CloseParenToken, doStatement.DoKeyword.TrailingTrivia),
                        doStatement.Statement.WithTrailingTrivia(ifStatement.GetTrailingTrivia()));

                    whileStatement = whileStatement.WithFormatterAnnotation();

                    return await document.ReplaceNodeAsync(ifStatement, whileStatement, cancellationToken).ConfigureAwait(false);
                }
            }

            throw new InvalidOperationException();
        }
    }
}
