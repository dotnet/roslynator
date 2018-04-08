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
    internal static class SplitIfElseRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (ifStatement.IsParentKind(SyntaxKind.ElseClause))
                return;

            if (ifStatement.Else == null)
                return;

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                StatementSyntax statement = ifOrElse.Statement;

                if (statement is BlockSyntax block)
                    statement = block.Statements.LastOrDefault();

                if (statement == null)
                    return;

                if (!statement.IsKind(
                    SyntaxKind.ReturnStatement,
                    SyntaxKind.ContinueStatement,
                    SyntaxKind.BreakStatement,
                    SyntaxKind.ThrowStatement))
                {
                    return;
                }
            }

            context.RegisterRefactoring(
                "Split if-else",
                cancellationToken => RefactorAsync(context.Document, ifStatement, cancellationToken));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            return document.ReplaceNodeAsync(ifStatement, GetNewStatements(), cancellationToken);

            IEnumerable<StatementSyntax> GetNewStatements()
            {
                ElseClauseSyntax parentElse = null;

                foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
                {
                    if (ifOrElse.IsIf)
                    {
                        IfStatementSyntax newIfStatement = ifOrElse.AsIf();

                        ElseClauseSyntax elseClause = newIfStatement.Else;

                        newIfStatement = newIfStatement.WithElse(null);

                        if (parentElse != null)
                            newIfStatement = newIfStatement.PrependToLeadingTrivia(parentElse.GetLeadingTrivia());

                        if (elseClause != null)
                            newIfStatement = newIfStatement.AppendToTrailingTrivia(CSharpFactory.NewLine());

                        yield return newIfStatement.WithFormatterAnnotation();

                        parentElse = ifStatement.Else;
                    }
                    else
                    {
                        StatementSyntax statement = ifOrElse.Statement;

                        if (statement is BlockSyntax block)
                        {
                            foreach (StatementSyntax newStatement in block.Statements.Select(f => f.WithFormatterAnnotation()))
                                yield return newStatement;
                        }
                        else
                        {
                            yield return statement;
                        }
                    }
                }
            }
        }
    }
}
