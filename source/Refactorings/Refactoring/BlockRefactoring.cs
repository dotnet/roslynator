// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring.WrapStatements;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class BlockRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, BlockSyntax block)
        {
            RemoveBracesRefactoring.ComputeRefactoring(context, block);

            if (context.Settings.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.WrapInUsingStatement,
                RefactoringIdentifiers.MergeIfStatements,
                RefactoringIdentifiers.CollapseToInitializer,
                RefactoringIdentifiers.WrapInIfStatement,
                RefactoringIdentifiers.WrapInTryCatch))
            {
                var blockSpan = new BlockSpan(block, context.Span);

                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.WrapInUsingStatement)
                    && context.SupportsSemanticModel
                    && blockSpan.HasSelectedStatement)
                {
                    var refactoring = new WrapInUsingStatementRefactoring();
                    await refactoring.ComputeRefactoringAsync(context, blockSpan);
                }

                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.CollapseToInitializer)
                    && blockSpan.HasSelectedStatement)
                {
                    await CollapseToInitializerRefactoring.ComputeRefactoringsAsync(context, blockSpan);
                }

                using (IEnumerator<StatementSyntax> en = GetSelectedStatements(block, context.Span).GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.MergeIfStatements)
                            && en.Current.IsKind(SyntaxKind.IfStatement))
                        {
                            var statements = new List<IfStatementSyntax>();
                            statements.Add((IfStatementSyntax)en.Current);

                            while (en.MoveNext())
                            {
                                if (en.Current.IsKind(SyntaxKind.IfStatement))
                                {
                                    statements.Add((IfStatementSyntax)en.Current);
                                }
                                else
                                {
                                    statements = null;
                                    break;
                                }
                            }

                            if (statements?.Count > 1)
                            {
                                context.RegisterRefactoring(
                                    "Merge if statements",
                                    cancellationToken =>
                                    {
                                        return MergeIfStatementsRefactoring.RefactorAsync(
                                            context.Document,
                                            block,
                                            statements.ToImmutableArray(),
                                            cancellationToken);
                                    });
                            }
                        }

                        if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.MergeAssignmentExpressionWithReturnStatement)
                            && en.Current.IsKind(SyntaxKind.ExpressionStatement))
                        {
                            var statement = (ExpressionStatementSyntax)en.Current;

                            if (statement.Expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true
                                && en.MoveNext()
                                && en.Current.IsKind(SyntaxKind.ReturnStatement))
                            {
                                var returnStatement = (ReturnStatementSyntax)en.Current;

                                if (returnStatement.Expression?.IsMissing == false
                                    && !en.MoveNext())
                                {
                                    var assignment = (AssignmentExpressionSyntax)statement.Expression;

                                    if (assignment.Left?.IsMissing == false
                                        && assignment.Right?.IsMissing == false
                                        && assignment.Left.IsEquivalentTo(returnStatement.Expression, topLevel: false))
                                    {
                                        context.RegisterRefactoring(
                                            "Merge statements",
                                            cancellationToken =>
                                            {
                                                return MergeAssignmentExpressionWithReturnStatementRefactoring.RefactorAsync(
                                                    context.Document,
                                                    statement,
                                                    returnStatement,
                                                    cancellationToken);
                                            });
                                    }
                                }
                            }
                        }

                        if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.WrapInIfStatement))
                        {
                            context.RegisterRefactoring(
                                "Wrap in if statement",
                                cancellationToken =>
                                {
                                    var refactoring = new WrapInIfStatementRefactoring();

                                    return refactoring.RefactorAsync(
                                        context.Document,
                                        block,
                                        context.Span,
                                        cancellationToken);
                                });
                        }

                        if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.WrapInTryCatch))
                        {
                            context.RegisterRefactoring(
                                "Wrap in try-catch",
                                cancellationToken =>
                                {
                                    var refactoring = new WrapInTryCatchRefactoring();

                                    return refactoring.RefactorAsync(
                                        context.Document,
                                        block,
                                        context.Span,
                                        cancellationToken);
                                });
                        }
                    }
                }
            }
        }

        public static IEnumerable<StatementSyntax> GetSelectedStatements(BlockSyntax block, TextSpan span)
        {
            return block.Statements
                .SkipWhile(f => span.Start > f.Span.Start)
                .TakeWhile(f => span.End >= f.Span.End);
        }
    }
}
