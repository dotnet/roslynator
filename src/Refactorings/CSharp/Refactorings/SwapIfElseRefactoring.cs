// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwapIfElseRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            SimpleIfElseInfo simpleIfElse = SyntaxInfo.SimpleIfElseInfo(ifStatement);

            if (!simpleIfElse.Success)
                return;

            context.RegisterRefactoring(
                "Swap if-else",
                cancellationToken => RefactorAsync(context.Document, ifStatement, cancellationToken));
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ElseClauseSyntax elseClause = ifStatement.Else;
            StatementSyntax whenTrue = ifStatement.Statement;
            StatementSyntax whenFalse = elseClause.Statement;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ElseClauseSyntax newElseClause = null;

            if ((whenFalse as BlockSyntax)?.Statements.Any() != false)
                newElseClause = elseClause.WithStatement(whenTrue.WithTriviaFrom(whenFalse));

            IfStatementSyntax newIfStatement = ifStatement
                .WithCondition(Negator.LogicallyNegate(ifStatement.Condition, semanticModel, cancellationToken))
                .WithStatement(whenFalse.WithTriviaFrom(whenTrue))
                .WithElse(newElseClause)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(ifStatement, newIfStatement, cancellationToken).ConfigureAwait(false);
        }
    }
}
