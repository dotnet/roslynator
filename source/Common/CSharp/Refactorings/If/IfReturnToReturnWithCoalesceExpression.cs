// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfReturnToReturnWithCoalesceExpression : IfToReturnWithCoalesceExpression
    {
        public IfReturnToReturnWithCoalesceExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax right) : base(ifStatement, left, right)
        {
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfReturnToReturnWithCoalesceExpression; }
        }

        public override string Title
        {
            get { return "Use coalesce expression"; }
        }

        public override bool IsYield
        {
            get { return false; }
        }

        protected override StatementSyntax CreateStatement(ExpressionSyntax expression)
        {
            return ReturnStatement(expression);
        }

        public override async Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken)
        {
            StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(IfStatement);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(IfStatement);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            StatementSyntax newNode = CreateStatement(CreateCoalesceExpression(semanticModel, cancellationToken))
                .WithLeadingTrivia(IfStatement.GetLeadingTrivia())
                .WithTrailingTrivia(statements[index + 1].GetTrailingTrivia())
                .WithFormatterAnnotation();

            SyntaxList<StatementSyntax> newStatements = statements
                .RemoveAt(index)
                .ReplaceAt(index, newNode);

            return await document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken).ConfigureAwait(false);
        }
    }
}