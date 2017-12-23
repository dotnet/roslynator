// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfToReturnWithExpression : IfRefactoring
    {
        public IfToReturnWithExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression,
            bool isYield,
            bool negate = false) : base(ifStatement)
        {
            Expression = expression;
            Negate = negate;
            IsYield = isYield;
        }

        public ExpressionSyntax Expression { get; }

        public bool Negate { get; }

        public bool IsYield { get; }

        public override RefactoringKind Kind
        {
            get
            {
                if (IsYield)
                    return RefactoringKind.IfElseToYieldReturnWithExpression;

                return (IfStatement.IsSimpleIf())
                    ? RefactoringKind.IfReturnToReturnWithExpression
                    : RefactoringKind.IfElseToReturnWithExpression;
            }
        }

        public override string Title
        {
            get
            {
                if (IsYield)
                    return "Replace if-else with yield return";

                return (IfStatement.IsSimpleIf())
                    ? "Replace if-return with return"
                    : "Replace if-else with return";
            }
        }

        protected StatementSyntax CreateStatement(ExpressionSyntax expression)
        {
            if (IsYield)
            {
                return CSharpFactory.YieldReturnStatement(expression);
            }
            else
            {
                return SyntaxFactory.ReturnStatement(expression);
            }
        }

        public override async Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax expression = Expression;

            if (Negate)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                expression = CSharpUtility.LogicallyNegate(expression, semanticModel, cancellationToken);
            }

            StatementSyntax statement = CreateStatement(expression);

            if (IfStatement.IsSimpleIf())
            {
                StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(IfStatement);

                SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

                int index = statements.IndexOf(IfStatement);

                StatementSyntax newNode = statement
                    .WithLeadingTrivia(IfStatement.GetLeadingTrivia())
                    .WithTrailingTrivia(statements[index + 1].GetTrailingTrivia())
                    .WithFormatterAnnotation();

                SyntaxList<StatementSyntax> newStatements = statements
                    .RemoveAt(index)
                    .ReplaceAt(index, newNode);

                return await document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                StatementSyntax newNode = statement
                    .WithTriviaFrom(IfStatement)
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}