// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.If
{
    internal abstract class ToAssignmentWithConditionalExpression<TStatement> : ToAssignmentWithConditionalExpression
        where TStatement : StatementSyntax
    {
        internal ToAssignmentWithConditionalExpression(
            TStatement statement,
            IfStatementSyntax ifStatement,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse) : base(ifStatement, whenTrue, whenFalse)
        {
            Statement = statement;
        }

        public TStatement Statement { get; }

        protected abstract TStatement CreateNewStatement();

        public override Task<Document> RefactorAsync(
            Document document,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementContainer container = StatementContainer.Create(IfStatement);

            SyntaxList<StatementSyntax> statements = container.Statements;

            int index = statements.IndexOf(IfStatement);

            TStatement newStatement = CreateNewStatement();

                SyntaxList<StatementSyntax> newStatements = statements
                .RemoveAt(index)
                .ReplaceAt(index - 1, newStatement);

            return document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken);
        }
    }
}
