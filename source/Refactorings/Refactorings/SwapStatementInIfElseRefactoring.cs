// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwapStatementInIfElseRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (CanRefactor(ifStatement))
            {
                context.RegisterRefactoring(
                    "Swap statements in if-else",
                    cancellationToken => RefactorAsync(context.Document, ifStatement, cancellationToken));
            }
        }

        public static bool CanRefactor(IfStatementSyntax ifStatement)
        {
            if (ifStatement.Condition != null
                && ifStatement.Statement != null)
            {
                StatementSyntax falseStatement = ifStatement.Else?.Statement;

                if (falseStatement?.IsKind(SyntaxKind.IfStatement) == false)
                    return true;
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementSyntax trueStatement = ifStatement.Statement;

            StatementSyntax falseStatement = ifStatement.Else.Statement;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IfStatementSyntax newIfStatement = ifStatement
                .WithCondition(CSharpUtility.LogicallyNegate(ifStatement.Condition, semanticModel, cancellationToken))
                .WithStatement(falseStatement.WithTriviaFrom(trueStatement))
                .WithElse(ifStatement.Else.WithStatement(trueStatement.WithTriviaFrom(falseStatement)))
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(ifStatement, newIfStatement, cancellationToken).ConfigureAwait(false);
        }
    }
}
