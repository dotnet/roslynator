// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReplaceDoStatementWithWhileStatementRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            DoStatementSyntax doStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            WhileStatementSyntax whileStatement = WhileStatement(
                doStatement.WhileKeyword,
                doStatement.OpenParenToken,
                doStatement.Condition,
                doStatement.CloseParenToken.WithTrailingTrivia(doStatement.DoKeyword.TrailingTrivia),
                doStatement.Statement);

            whileStatement = whileStatement
                .WithTriviaFrom(doStatement)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(doStatement, whileStatement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
