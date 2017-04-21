// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToAssignmentWithConditionalExpression : ToAssignmentWithConditionalExpression
    {
        internal IfElseToAssignmentWithConditionalExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax right1,
            ExpressionSyntax right2) : base(ifStatement, right1, right2)
        {
            Left = left;
        }

        public ExpressionSyntax Left { get; }

        public override Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            ConditionalExpressionSyntax conditionalExpression = IfRefactoringHelper.CreateConditionalExpression(IfStatement.Condition, WhenTrue, WhenFalse);

            ExpressionStatementSyntax newNode = SimpleAssignmentStatement(Left, conditionalExpression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken);
        }
    }
}
