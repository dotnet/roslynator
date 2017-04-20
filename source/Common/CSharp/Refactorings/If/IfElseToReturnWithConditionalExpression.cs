// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToReturnWithConditionalExpression : IfToReturnWithConditionalExpression
    {
        public IfElseToReturnWithConditionalExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2) : base(ifStatement, expression1, expression2)
        {
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfElseToReturnWithConditionalExpression; }
        }

        public override string Title
        {
            get { return "Use conditional expression"; }
        }

        protected override StatementSyntax CreateStatement(ExpressionSyntax expression)
        {
            return ReturnStatement(expression);
        }

        public override Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            ConditionalExpressionSyntax conditionalExpression = IfRefactoringHelper.CreateConditionalExpression(IfStatement.Condition, Expression1, Expression2);

            StatementSyntax newNode = CreateStatement(conditionalExpression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken);
        }
    }
}