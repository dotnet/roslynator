// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
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
                    CompilerDiagnosticIdentifiers.EmptySwitchBlock,
                    CompilerDiagnosticIdentifiers.OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement,
                    CompilerDiagnosticIdentifiers.NoEnclosingLoopOutOfWhichToBreakOrContinue);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.RemoveEmptySwitchStatement,
                CodeFixIdentifiers.IntroduceLocalVariable,
                CodeFixIdentifiers.RemoveJumpStatement))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out StatementSyntax statement))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
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
