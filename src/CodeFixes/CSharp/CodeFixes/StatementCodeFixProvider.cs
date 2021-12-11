// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StatementCodeFixProvider))]
    [Shared]
    public sealed class StatementCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS1522_EmptySwitchBlock,
                    CompilerDiagnosticIdentifiers.CS0139_NoEnclosingLoopOutOfWhichToBreakOrContinue);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out StatementSyntax statement))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS1522_EmptySwitchBlock:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveEmptySwitchStatement, context.Document, root.SyntaxTree))
                                break;

                            if (statement is not SwitchStatementSyntax switchStatement)
                                break;

                            CodeFixRegistrator.RemoveStatement(context, diagnostic, switchStatement);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0139_NoEnclosingLoopOutOfWhichToBreakOrContinue:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveJumpStatement, context.Document, root.SyntaxTree))
                                CodeFixRegistrator.RemoveStatement(context, diagnostic, statement);

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceBreakWithContinue, context.Document, root.SyntaxTree)
                                && statement.Kind() == SyntaxKind.BreakStatement)
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                if (semanticModel.GetEnclosingSymbol(statement.SpanStart, context.CancellationToken) is IMethodSymbol methodSymbol)
                                {
                                    if (methodSymbol.ReturnsVoid
                                        || (methodSymbol.IsAsync && methodSymbol.ReturnType.HasMetadataName(MetadataNames.System_Threading_Tasks_Task)))
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Replace 'break' with 'return'",
                                            ct =>
                                            {
                                                var breakStatement = (BreakStatementSyntax)statement;
                                                SyntaxToken breakKeyword = breakStatement.BreakKeyword;

                                                ReturnStatementSyntax newStatement = SyntaxFactory.ReturnStatement(
                                                    SyntaxFactory.Token(breakKeyword.LeadingTrivia, SyntaxKind.ReturnKeyword, breakKeyword.TrailingTrivia),
                                                    null,
                                                    breakStatement.SemicolonToken);

                                                return context.Document.ReplaceNodeAsync(statement, newStatement, ct);
                                            },
                                            GetEquivalenceKey(diagnostic, CodeFixIdentifiers.ReplaceBreakWithContinue));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveJumpStatement, context.Document, root.SyntaxTree))
                                CodeFixRegistrator.RemoveStatement(context, diagnostic, statement);

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceBreakWithContinue, context.Document, root.SyntaxTree)
                                && statement.Kind() == SyntaxKind.BreakStatement)
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                                if (semanticModel.GetEnclosingSymbol(statement.SpanStart, context.CancellationToken) is IMethodSymbol methodSymbol)
                                {
                                    if (methodSymbol.ReturnsVoid
                                        || (methodSymbol.IsAsync && methodSymbol.ReturnType.HasMetadataName(MetadataNames.System_Threading_Tasks_Task)))
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Replace 'break' with 'return'",
                                            ct =>
                                            {
                                                var breakStatement = (BreakStatementSyntax)statement;
                                                SyntaxToken breakKeyword = breakStatement.BreakKeyword;

                                                ReturnStatementSyntax newStatement = SyntaxFactory.ReturnStatement(
                                                    SyntaxFactory.Token(breakKeyword.LeadingTrivia, SyntaxKind.ReturnKeyword, breakKeyword.TrailingTrivia),
                                                    null,
                                                    breakStatement.SemicolonToken);

                                                return context.Document.ReplaceNodeAsync(statement, newStatement, ct);
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
