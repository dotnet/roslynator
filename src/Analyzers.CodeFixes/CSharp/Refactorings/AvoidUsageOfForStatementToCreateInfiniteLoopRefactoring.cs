// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidUsageOfForStatementToCreateInfiniteLoopRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken)
        {
            LiteralExpressionSyntax trueLiteral = TrueLiteralExpression();

            TextSpan span = TextSpan.FromBounds(
                forStatement.OpenParenToken.FullSpan.End,
                forStatement.CloseParenToken.FullSpan.Start);

            IEnumerable<SyntaxTrivia> trivia = forStatement.DescendantTrivia(span);

            if (!trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                trueLiteral = trueLiteral.WithTrailingTrivia(trivia);

            WhileStatementSyntax whileStatement = WhileStatement(
                WhileKeyword().WithTriviaFrom(forStatement.ForKeyword),
                forStatement.OpenParenToken,
                trueLiteral,
                forStatement.CloseParenToken,
                forStatement.Statement);

            whileStatement = whileStatement
                .WithTriviaFrom(forStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(forStatement, whileStatement, cancellationToken);
        }
    }
}
