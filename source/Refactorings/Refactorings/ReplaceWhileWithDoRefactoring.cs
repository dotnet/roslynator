// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceWhileWithDoRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            WhileStatementSyntax whileStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            DoStatementSyntax doStatement = DoStatement(
                Token(
                    whileStatement.WhileKeyword.LeadingTrivia,
                    SyntaxKind.DoKeyword,
                    whileStatement.CloseParenToken.TrailingTrivia),
                whileStatement.Statement.WithoutTrailingTrivia(),
                WhileKeyword(),
                whileStatement.OpenParenToken,
                whileStatement.Condition,
                whileStatement.CloseParenToken.WithoutTrailingTrivia(),
                SemicolonToken());

            doStatement = doStatement
                .WithTriviaFrom(whileStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(whileStatement, doStatement, cancellationToken).ConfigureAwait(false);
        }
    }
}
