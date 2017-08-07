// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddBracesRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.AddBraces,
                    RefactoringIdentifiers.AddBracesToIfElse)
                && CanRefactor(context, statement))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddBraces))
                {
                    RegisterRefactoring(context, statement);
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddBracesToIfElse))
                {
                    IfStatementSyntax topmostIf = GetTopmostIf(statement);

                    if (topmostIf?.Else != null
                        && GetEmbeddedStatements(topmostIf).Any(f => f != statement))
                    {
                        context.RegisterRefactoring(
                            "Add braces to if-else",
                            cancellationToken =>
                            {
                                return AddBracesToIfElseRefactoring.RefactorAsync(
                                    context.Document,
                                    topmostIf,
                                    cancellationToken);
                            });
                    }
                }
            }
        }

        private static IEnumerable<StatementSyntax> GetEmbeddedStatements(IfStatementSyntax topmostIf)
        {
            foreach (IfStatementOrElseClause ifOrElse in topmostIf.GetChain())
            {
                StatementSyntax statement = ifOrElse.Statement;

                if (statement?.IsKind(SyntaxKind.Block) == false)
                    yield return statement;
            }
        }

        public static void RegisterRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            context.RegisterRefactoring(
                "Add braces",
                cancellationToken => RefactorAsync(context.Document, statement, cancellationToken));
        }

        private static bool CanRefactor(RefactoringContext context, StatementSyntax statement)
        {
            return context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(statement)
                && EmbeddedStatementHelper.IsEmbeddedStatement(statement);
        }

        private static IfStatementSyntax GetTopmostIf(StatementSyntax statement)
        {
            SyntaxNode parent = statement.Parent;

            if (parent != null)
            {
                if (parent.IsKind(SyntaxKind.ElseClause))
                {
                    return ((ElseClauseSyntax)parent).GetTopmostIf();
                }
                else
                {
                    var parentStatement = parent as IfStatementSyntax;

                    if (parentStatement != null)
                        return parentStatement.GetTopmostIf();
                }
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            BlockSyntax block = SyntaxFactory.Block(statement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(statement, block, cancellationToken);
        }
    }
}
