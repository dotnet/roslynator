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
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StatementCodeFixProvider))]
    [Shared]
    public class StatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.UnreachableCodeDetected,
                    CompilerDiagnosticIdentifiers.EmptySwitchBlock,
                    CompilerDiagnosticIdentifiers.OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement,
                    CompilerDiagnosticIdentifiers.NoEnclosingLoopOutOfWhichToBreakOrContinue);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.RemoveUnreachableCode,
                CodeFixIdentifiers.RemoveEmptySwitchStatement,
                CodeFixIdentifiers.IntroduceLocalVariable,
                CodeFixIdentifiers.RemoveJumpStatement))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            StatementSyntax statement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<StatementSyntax>();

            Debug.Assert(statement != null, $"{nameof(statement)} is null");

            if (statement == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.UnreachableCodeDetected:
                        {
                            if (context.Span.Start == statement.SpanStart)
                            {
                                StatementContainer container;
                                if (StatementContainer.TryCreate(statement, out container))
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        "Remove unreachable code",
                                        cancellationToken =>
                                        {
                                            SyntaxList<StatementSyntax> statements = container.Statements;

                                            int index = statements.IndexOf(statement);

                                            if (index == statements.Count - 1)
                                            {
                                                return context.Document.RemoveStatementAsync(statement, context.CancellationToken);
                                            }
                                            else
                                            {
                                                SyntaxRemoveOptions removeOptions = RemoveHelper.DefaultRemoveOptions;

                                                if (statement.GetLeadingTrivia().IsEmptyOrWhitespace())
                                                    removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

                                                if (statements.Last().GetTrailingTrivia().IsEmptyOrWhitespace())
                                                    removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

                                                return context.Document.RemoveNodesAsync(statements.Skip(index), removeOptions, context.CancellationToken);
                                            }
                                        },
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.EmptySwitchBlock:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveEmptySwitchStatement))
                                break;

                            if (!statement.IsKind(SyntaxKind.SwitchStatement))
                                break;

                            var switchStatement = (SwitchStatementSyntax)statement;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove switch statement",
                                cancellationToken => context.Document.RemoveStatementAsync(switchStatement, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.IntroduceLocalVariable))
                                break;

                            if (!statement.IsKind(SyntaxKind.ExpressionStatement))
                                break;

                            var expressionStatement = (ExpressionStatementSyntax)statement;

                            ExpressionSyntax expression = expressionStatement.Expression;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            if (semanticModel.GetSymbol(expression, context.CancellationToken)?.IsErrorType() == false)
                            {
                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                                if (typeSymbol?.IsErrorType() == false)
                                {
                                    bool addAwait = typeSymbol.IsConstructedFromTaskOfT(semanticModel)
                                        && semanticModel.GetEnclosingSymbol(expressionStatement.SpanStart, context.CancellationToken).IsAsyncMethod();

                                    CodeAction codeAction = CodeAction.Create(
                                        IntroduceLocalVariableRefactoring.GetTitle(expression),
                                        cancellationToken => IntroduceLocalVariableRefactoring.RefactorAsync(context.Document, expressionStatement, typeSymbol, addAwait, semanticModel, cancellationToken),
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NoEnclosingLoopOutOfWhichToBreakOrContinue:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveJumpStatement))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                $"Remove {statement.GetTitle()}",
                                cancellationToken => context.Document.RemoveStatementAsync(statement, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
