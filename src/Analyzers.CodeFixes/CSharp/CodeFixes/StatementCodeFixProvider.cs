// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Analysis.UseMethodChaining;
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
                    DiagnosticIdentifiers.AddEmptyLineAfterLastStatementInDoStatement,
                    DiagnosticIdentifiers.UseCoalesceExpression,
                    DiagnosticIdentifiers.InlineLazyInitialization,
                    DiagnosticIdentifiers.RemoveRedundantDisposeOrCloseCall,
                    DiagnosticIdentifiers.RemoveRedundantStatement,
                    DiagnosticIdentifiers.UseMethodChaining);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out StatementSyntax statement))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.AddEmptyLineAfterLastStatementInDoStatement:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Add empty line",
                                cancellationToken =>
                                {
                                    return AddEmptyLineAfterLastStatementInDoStatementRefactoring.RefactorAsync(
                                        context.Document,
                                        statement,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseCoalesceExpression:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use coalesce expression",
                                cancellationToken =>
                                {
                                    return UseCoalesceExpressionRefactoring.RefactorAsync(
                                        context.Document,
                                        statement,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.InlineLazyInitialization:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Inline lazy initialization",
                                cancellationToken =>
                                {
                                    return InlineLazyInitializationRefactoring.RefactorAsync(
                                        context.Document,
                                        (IfStatementSyntax)statement,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantDisposeOrCloseCall:
                        {
                            var expressionStatement = (ExpressionStatementSyntax)statement;
                            var invocation = (InvocationExpressionSyntax)expressionStatement.Expression;
                            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                            CodeAction codeAction = CodeAction.Create(
                                $"Remove redundant '{memberAccess.Name?.Identifier.ValueText}' call",
                                cancellationToken => RemoveRedundantDisposeOrCloseCallRefactoring.RefactorAsync(context.Document, expressionStatement, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantStatement:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Remove redundant {CSharpFacts.GetTitle(statement)}",
                                cancellationToken => RemoveRedundantStatementRefactoring.RefactorAsync(context.Document, statement, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseMethodChaining:
                        {
                            var expressionStatement = (ExpressionStatementSyntax)statement;

                            UseMethodChainingAnalysis analysis;
                            if (expressionStatement.Expression.Kind() == SyntaxKind.InvocationExpression)
                            {
                                analysis = UseMethodChainingAnalysis.WithoutAssignmentAnalysis;
                            }
                            else
                            {
                                analysis = UseMethodChainingAnalysis.WithAssignmentAnalysis;
                            }

                            CodeAction codeAction = CodeAction.Create(
                                "Use method chaining",
                                cancellationToken => UseMethodChainingRefactoring.RefactorAsync(context.Document, analysis, expressionStatement, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
