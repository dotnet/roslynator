// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;
using Roslynator.Text.Extensions;

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
                        && IfElseChain.GetEmbeddedStatements(topmostIf).Any(f => f != statement))
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

        public static void RegisterRefactoring(RefactoringContext context, StatementSyntax statement)
        {
            context.RegisterRefactoring(
                "Add braces",
                cancellationToken => RefactorAsync(context.Document, statement, cancellationToken));
        }

        private static bool CanRefactor(RefactoringContext context, StatementSyntax statement)
        {
            return context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(statement)
                && EmbeddedStatement.IsEmbeddedStatement(statement);
        }

        private static IfStatementSyntax GetTopmostIf(StatementSyntax statement)
        {
            SyntaxNode parent = statement.Parent;

            if (parent != null)
            {
                if (parent.IsKind(SyntaxKind.ElseClause))
                {
                    return IfElseChain.GetTopmostIf((ElseClauseSyntax)parent);
                }
                else
                {
                    var parentStatement = parent as IfStatementSyntax;

                    if (parentStatement != null)
                        return IfElseChain.GetTopmostIf(parentStatement);
                }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            BlockSyntax block = SyntaxFactory.Block(statement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(statement, block, cancellationToken).ConfigureAwait(false);
        }
    }
}
