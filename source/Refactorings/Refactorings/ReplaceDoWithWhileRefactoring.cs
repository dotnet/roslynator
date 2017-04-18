// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceDoWithWhileRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            DoStatementSyntax doStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            WhileStatementSyntax whileStatement = WhileStatement(
                doStatement.WhileKeyword,
                doStatement.OpenParenToken,
                doStatement.Condition,
                doStatement.CloseParenToken.WithTrailingTrivia(doStatement.DoKeyword.TrailingTrivia),
                doStatement.Statement);

            whileStatement = whileStatement
                .WithTriviaFrom(doStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(doStatement, whileStatement, cancellationToken);
        }
    }
}
