// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeIfWithParentIfRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (!ifStatement.IsTopmostIf())
                return;

            if (ifStatement.Condition.IsMissing)
                return;

            if (ifStatement.Parent is IfStatementSyntax parentIf)
            {
                parentIf = (IfStatementSyntax)ifStatement.Parent;
            }
            else
            {
                if (ifStatement.Parent is not BlockSyntax block)
                    return;

                if (block.Statements.Count != 1)
                    return;

                parentIf = block.Parent as IfStatementSyntax;

                if (parentIf == null)
                    return;
            }

            if (parentIf.Condition.IsMissing)
                return;

            context.RegisterRefactoring(
                "Merge if with parent if",
                ct => RefactorAsync(context.Document, ifStatement, ct),
                RefactoringDescriptors.MergeIfWithParentIf);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            StatementSyntax newStatement;

            if (ifStatement.Parent is IfStatementSyntax parentIf)
            {
                newStatement = ifStatement.Statement;
            }
            else
            {
                var parentBlock = (BlockSyntax)ifStatement.Parent;

                parentIf = (IfStatementSyntax)parentBlock.Parent;

                SyntaxList<StatementSyntax> statements = parentBlock.Statements;

                SyntaxList<StatementSyntax> newStatements = (ifStatement.Statement is BlockSyntax block)
                    ? block.Statements
                    : SyntaxFactory.SingletonList(ifStatement.Statement);

                newStatements = statements.ReplaceRange(ifStatement, newStatements);

                newStatement = parentBlock.WithStatements(newStatements);
            }

            IfStatementSyntax newNode = parentIf
                .WithStatement(newStatement)
                .WithCondition(LogicalAndExpression(parentIf.Condition.Parenthesize(), ifStatement.Condition.Parenthesize()))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(parentIf, newNode, cancellationToken);
        }
    }
}
