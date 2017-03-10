// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToAssignmentWithConditionalExpression : IfRefactoring
    {
        internal IfElseToAssignmentWithConditionalExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax right1,
            ExpressionSyntax right2) : base(ifStatement)
        {
            Left = left;
            Right1 = right1;
            Right2 = right2;
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfElseToAssignmentWithConditionalExpression; }
        }

        public override string Title
        {
            get { return "Use conditional expression"; }
        }

        public ExpressionSyntax Left { get; }

        public ExpressionSyntax Right1 { get; }

        public ExpressionSyntax Right2 { get; }

        public override Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            ConditionalExpressionSyntax conditionalExpression = IfRefactoringHelper.CreateConditionalExpression(IfStatement.Condition, Right1, Right2);

            ExpressionStatementSyntax newNode = SimpleAssignmentStatement(Left, conditionalExpression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken);
        }
    }
}
