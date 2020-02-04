// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
using Roslynator.CodeFixes;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Analysis.If;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Refactorings.ReduceIfNesting;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IfStatementCodeFixProvider))]
    [Shared]
    public class IfStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.MergeIfStatementWithNestedIfStatement,
                    DiagnosticIdentifiers.UseCoalesceExpressionInsteadOfIf,
                    DiagnosticIdentifiers.ConvertIfToReturnStatement,
                    DiagnosticIdentifiers.ConvertIfToAssignment,
                    DiagnosticIdentifiers.ReduceIfNesting,
                    DiagnosticIdentifiers.UseExceptionFilter);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out IfStatementSyntax ifStatement))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.MergeIfStatementWithNestedIfStatement:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Merge if with nested if",
                                cancellationToken =>
                                {
                                    return MergeIfStatementWithNestedIfStatementRefactoring.RefactorAsync(
                                        context.Document,
                                        ifStatement,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseCoalesceExpressionInsteadOfIf:
                    case DiagnosticIdentifiers.ConvertIfToReturnStatement:
                    case DiagnosticIdentifiers.ConvertIfToAssignment:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            IfAnalysis analysis = IfAnalysis.Analyze(
                                ifStatement,
                                IfStatementAnalyzer.AnalysisOptions,
                                semanticModel,
                                context.CancellationToken).First();

                            CodeAction codeAction = CodeAction.Create(
                                analysis.Title,
                                cancellationToken => IfRefactoring.RefactorAsync(context.Document, analysis, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.ReduceIfNesting:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Invert if",
                                cancellationToken =>
                                {
                                    return ReduceIfNestingRefactoring.RefactorAsync(
                                        context.Document,
                                        ifStatement,
                                        (SyntaxKind)Enum.Parse(typeof(SyntaxKind), diagnostic.Properties["JumpKind"]),
                                        recursive: true,
                                        cancellationToken: cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseExceptionFilter:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use exception filter",
                                cancellationToken =>
                                {
                                    return UseExceptionFilterAsync(
                                        context.Document,
                                        ifStatement,
                                        cancellationToken: cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static async Task<Document> UseExceptionFilterAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            ElseClauseSyntax elseClause = ifStatement.Else;

            var catchClause = (CatchClauseSyntax)ifStatement.Parent.Parent;

            SyntaxList<StatementSyntax> statements = catchClause.Block.Statements;

            ExpressionSyntax filterExpression = ifStatement.Condition;

            SyntaxList<StatementSyntax> newStatements;

            if (ifStatement.Statement.SingleNonBlockStatementOrDefault() is ThrowStatementSyntax throwStatement
                && throwStatement.Expression == null)
            {
                if (elseClause != null)
                {
                    newStatements = ReplaceStatement(elseClause.Statement);
                }
                else
                {
                    newStatements = statements.RemoveAt(0);
                }

                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                filterExpression = SyntaxInverter.LogicallyInvert(filterExpression, semanticModel, cancellationToken);
            }
            else
            {
                newStatements = ReplaceStatement(ifStatement.Statement);
            }

            CatchClauseSyntax newCatchClause = catchClause.Update(
                catchKeyword: catchClause.CatchKeyword,
                declaration: catchClause.Declaration,
                filter: SyntaxFactory.CatchFilterClause(filterExpression.WalkDownParentheses()),
                block: catchClause.Block.WithStatements(newStatements));

            newCatchClause = newCatchClause.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(catchClause, newCatchClause, cancellationToken).ConfigureAwait(false);

            SyntaxList<StatementSyntax> ReplaceStatement(StatementSyntax statement)
            {
                if (statement is BlockSyntax block)
                {
                    return statements.ReplaceRange(ifStatement, block.Statements);
                }
                else
                {
                    return statements.Replace(ifStatement, statement);
                }
            }
        }
    }
}
