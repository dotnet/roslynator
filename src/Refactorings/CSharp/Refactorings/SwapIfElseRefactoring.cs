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
                cancellationToken => RefactorAsync(context.Document, ifStatement, cancellationToken),
                RefactoringIdentifiers.SwapIfElse);
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

            IfStatementSyntax newIfStatement = ifStatement.Update(
                ifKeyword: ifStatement.IfKeyword,
                openParenToken: ifStatement.OpenParenToken,
                condition: Negator.LogicallyNegate(ifStatement.Condition, semanticModel, cancellationToken),
                closeParenToken: ifStatement.CloseParenToken,
                statement: whenFalse.WithTriviaFrom(whenTrue),
                @else: elseClause.WithStatement(whenTrue.WithTriviaFrom(whenFalse)));

            newIfStatement = newIfStatement.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(ifStatement, newIfStatement, cancellationToken).ConfigureAwait(false);
        }
    }
}
