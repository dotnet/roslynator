// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidUsageOfWhileStatementToCreateInfiniteLoopRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            WhileStatementSyntax whileStatement,
            CancellationToken cancellationToken)
        {
            ForStatementSyntax newNode = ForStatement(
                ForKeyword().WithTriviaFrom(whileStatement.WhileKeyword),
                Token(
                    whileStatement.OpenParenToken.LeadingTrivia,
                    SyntaxKind.OpenParenToken,
                    default(SyntaxTriviaList)),
                default(VariableDeclarationSyntax),
                default(SeparatedSyntaxList<ExpressionSyntax>),
                SemicolonToken(),
                default(ExpressionSyntax),
                SemicolonToken(),
                default(SeparatedSyntaxList<ExpressionSyntax>),
                Token(
                    default(SyntaxTriviaList),
                    SyntaxKind.CloseParenToken,
                    whileStatement.CloseParenToken.TrailingTrivia),
                whileStatement.Statement);

            newNode = newNode
                .WithTriviaFrom(whileStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(whileStatement, newNode, cancellationToken);
        }
    }
}
