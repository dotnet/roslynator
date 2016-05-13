// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class DoStatementRefactoring
    {
        public static bool CanConvertToWhileStatement(DoStatementSyntax doStatement)
        {
            if (doStatement == null)
                throw new ArgumentNullException(nameof(doStatement));

            return doStatement.Condition?.IsKind(SyntaxKind.TrueLiteralExpression) == true;
        }

        public static async Task<Document> ConvertToWhileStatementAsync(
            Document document,
            DoStatementSyntax doStatement,
            CancellationToken cancellationToken)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (doStatement == null)
                throw new ArgumentNullException(nameof(doStatement));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            WhileStatementSyntax newNode = WhileStatement(
                Token(SyntaxKind.WhileKeyword),
                Token(SyntaxKind.OpenParenToken),
                LiteralExpression(SyntaxKind.TrueLiteralExpression),
                Token(
                    default(SyntaxTriviaList),
                    SyntaxKind.CloseParenToken,
                    doStatement.DoKeyword.TrailingTrivia),
                doStatement.Statement);

            newNode = newNode
                .WithTriviaFrom(doStatement)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(doStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
