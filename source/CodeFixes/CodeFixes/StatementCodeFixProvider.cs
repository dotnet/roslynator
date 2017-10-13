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
                CodeFixIdentifiers.RemoveJumpStatement,
                CodeFixIdentifiers.ReplaceBreakWithContinue))
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

                            if (!(statement is SwitchStatementSyntax switchStatement))
                                break;

                            CodeFixRegistrator.RemoveStatement(context, diagnostic, switchStatement);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement:
                        {
                            if (Settings.IsAnyCodeFixEnabled(
                                CodeFixIdentifiers.IntroduceLocalVariable,
                                CodeFixIdentifiers.IntroduceField))
                            {
                                if (!(statement is ExpressionStatementSyntax expressionStatement))
                                    break;

                                ExpressionSyntax expression = expressionStatement.Expression;

                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                if (semanticModel.GetSymbol(expression, context.CancellationToken)?.IsErrorType() != false)
                                    break;

                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                                if (typeSymbol?.IsErrorType() != false)
                                    break;

                                if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.IntroduceLocalVariable)
                                    && !statement.IsEmbedded())
                                {
                                    bool addAwait = typeSymbol.IsConstructedFromTaskOfT(semanticModel)
                                        && semanticModel.GetEnclosingSymbol(expressionStatement.SpanStart, context.CancellationToken).IsAsyncMethod();

                                    CodeAction codeAction = CodeAction.Create(
                                        IntroduceLocalVariableRefactoring.GetTitle(expression),
                                        cancellationToken => IntroduceLocalVariableRefactoring.RefactorAsync(context.Document, expressionStatement, typeSymbol, addAwait, semanticModel, cancellationToken),
                                        GetEquivalenceKey(diagnostic, CodeFixIdentifiers.IntroduceLocalVariable));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }

                                if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.IntroduceField))
                                {
                                    CodeAction codeAction = CodeAction.Create(
                                        $"Introduce field for '{expression}'",
                                        cancellationToken => IntroduceFieldRefactoring.RefactorAsync(context.Document, expressionStatement, typeSymbol, semanticModel, cancellationToken),
                                        GetEquivalenceKey(diagnostic, CodeFixIdentifiers.IntroduceField));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                }
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NoEnclosingLoopOutOfWhichToBreakOrContinue:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveJumpStatement))
                                CodeFixRegistrator.RemoveStatement(context, diagnostic, statement);

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceBreakWithContinue)
                                && statement.Kind() == SyntaxKind.BreakStatement)
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                if (semanticModel.GetEnclosingSymbol(statement.SpanStart, context.CancellationToken) is IMethodSymbol methodSymbol)
                                {
                                    if (methodSymbol.ReturnsVoid
                                        || (methodSymbol.IsAsync && methodSymbol.ReturnType.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task))))
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Replace 'break' with 'return'",
                                            cancellationToken =>
                                            {
                                                var breakStatement = (BreakStatementSyntax)statement;
                                                SyntaxToken breakKeyword = breakStatement.BreakKeyword;

                                                ReturnStatementSyntax newStatement = SyntaxFactory.ReturnStatement(
                                                    SyntaxFactory.Token(breakKeyword.LeadingTrivia, SyntaxKind.ReturnKeyword, breakKeyword.TrailingTrivia),
                                                    null,
                                                    breakStatement.SemicolonToken);

                                                return context.Document.ReplaceNodeAsync(statement, newStatement, context.CancellationToken);
                                            },
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.ReplaceBreakWithContinue));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }

                            break;
                        }
                }
            }
        }
    }
}
