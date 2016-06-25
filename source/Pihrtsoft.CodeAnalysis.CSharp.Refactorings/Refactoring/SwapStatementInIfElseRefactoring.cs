// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SwapStatementInIfElseRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (ifStatement.Condition != null
                && ifStatement.Statement != null)
            {
                StatementSyntax falseStatement = ifStatement.Else?.Statement;

                if (falseStatement != null
                    && !falseStatement.IsKind(SyntaxKind.IfStatement))
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            StatementSyntax trueStatement = ifStatement.Statement;

            StatementSyntax falseStatement = ifStatement.Else.Statement;

            IfStatementSyntax newIfStatement = ifStatement
                .WithCondition(ifStatement.Condition.Negate())
                .WithStatement(falseStatement.WithTriviaFrom(trueStatement))
                .WithElse(ifStatement.Else.WithStatement(trueStatement.WithTriviaFrom(falseStatement)))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(ifStatement, newIfStatement);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
