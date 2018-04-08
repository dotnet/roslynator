// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnreachableCodeCodeFixProvider))]
    [Shared]
    public class UnreachableCodeCodeFixProvider : BaseCodeFixProvider
    {
        private const string Title = "Remove unreachable code";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.UnreachableCodeDetected); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveUnreachableCode))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out StatementSyntax statement))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.UnreachableCodeDetected:
                        {
                            Debug.Assert(context.Span.Start == statement.SpanStart, statement.ToString());

                            if (context.Span.Start != statement.SpanStart)
                                break;

                            CodeAction codeAction = CreateCodeActionForIfElse(context.Document, diagnostic, statement.Parent);

                            if (codeAction != null)
                            {
                                context.RegisterCodeFix(codeAction, diagnostic);
                                break;
                            }

                            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(statement);
                            if (statementsInfo.Success)
                            {
                                codeAction = CodeAction.Create(
                                    Title,
                                    cancellationToken =>
                                    {
                                        SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

                                        int index = statements.IndexOf(statement);

                                        if (index == statements.Count - 1)
                                        {
                                            return context.Document.RemoveStatementAsync(statement, cancellationToken);
                                        }
                                        else
                                        {
                                            SyntaxRemoveOptions removeOptions = SyntaxRemover.DefaultRemoveOptions;

                                            if (statement.GetLeadingTrivia().IsEmptyOrWhitespace())
                                                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

                                            if (statements.Last().GetTrailingTrivia().IsEmptyOrWhitespace())
                                                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

                                            return context.Document.RemoveNodesAsync(statements.Skip(index), removeOptions, cancellationToken);
                                        }
                                    },
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                }
            }
        }

        private CodeAction CreateCodeActionForIfElse(Document document, Diagnostic diagnostic, SyntaxNode node)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.IfStatement:
                    {
                        var ifStatement = (IfStatementSyntax)node;

                        StatementSyntax statement = ifStatement.Else?.Statement;

                        if (statement != null)
                        {
                            if (statement.IsKind(SyntaxKind.Block))
                            {
                                var block = (BlockSyntax)statement;

                                SyntaxList<StatementSyntax> statements = block.Statements;

                                if (statements.Any())
                                {
                                    return CreateCodeAction(document, diagnostic, ifStatement, statements);
                                }
                            }
                            else
                            {
                                return CreateCodeAction(document, diagnostic, ifStatement, statement);
                            }
                        }

                        return CodeAction.Create(
                            Title,
                            cancellationToken => document.RemoveStatementAsync(ifStatement, cancellationToken),
                            GetEquivalenceKey(diagnostic));
                    }
                case SyntaxKind.ElseClause:
                    {
                        var elseClause = (ElseClauseSyntax)node;

                        if (elseClause.IsParentKind(SyntaxKind.IfStatement))
                        {
                            var ifStatement = (IfStatementSyntax)elseClause.Parent;

                            if (ifStatement.IsTopmostIf())
                            {
                                StatementSyntax statement = ifStatement.Statement;

                                if (statement != null)
                                {
                                    if (statement.IsKind(SyntaxKind.Block))
                                    {
                                        var block = (BlockSyntax)statement;

                                        SyntaxList<StatementSyntax> statements = block.Statements;

                                        if (statements.Any())
                                            return CreateCodeAction(document, diagnostic, ifStatement, statements);
                                    }
                                    else
                                    {
                                        return CreateCodeAction(document, diagnostic, ifStatement, statement);
                                    }
                                }
                            }
                        }

                        return CodeAction.Create(
                            Title,
                            cancellationToken => document.RemoveNodeAsync(elseClause, cancellationToken),
                            GetEquivalenceKey(diagnostic));
                    }
                case SyntaxKind.Block:
                    {
                        return CreateCodeActionForIfElse(document, diagnostic, node.Parent);
                    }
            }

            return null;
        }

        private CodeAction CreateCodeAction(Document document, Diagnostic diagnostic, IfStatementSyntax ifStatement, SyntaxList<StatementSyntax> statements)
        {
            return CodeAction.Create(
                Title,
                cancellationToken =>
                {
                    StatementSyntax firstStatement = statements.First();

                    StatementSyntax newFirstStatement = firstStatement
                        .WithLeadingTrivia(ifStatement.GetLeadingTrivia().AddRange(firstStatement.GetLeadingTrivia().EmptyIfWhitespace()));

                    statements = statements.Replace(firstStatement, newFirstStatement);

                    StatementSyntax lastStatement = statements.Last();

                    StatementSyntax newLastStatement = lastStatement
                        .WithTrailingTrivia(lastStatement.GetTrailingTrivia().EmptyIfWhitespace().AddRange(ifStatement.GetTrailingTrivia()));

                    statements = statements.Replace(lastStatement, newLastStatement);

                    return document.ReplaceNodeAsync(ifStatement, statements, cancellationToken);
                },
                GetEquivalenceKey(diagnostic));
        }

        private CodeAction CreateCodeAction(Document document, Diagnostic diagnostic, IfStatementSyntax ifStatement, StatementSyntax statement)
        {
            return CodeAction.Create(
                Title,
                cancellationToken =>
                {
                    StatementSyntax newNode = statement
                        .WithLeadingTrivia(ifStatement.GetLeadingTrivia().AddRange(statement.GetLeadingTrivia().EmptyIfWhitespace()))
                        .WithTrailingTrivia(statement.GetTrailingTrivia().EmptyIfWhitespace().AddRange(ifStatement.GetTrailingTrivia()));

                    return document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken);
                },
                GetEquivalenceKey(diagnostic));
        }
    }
}
