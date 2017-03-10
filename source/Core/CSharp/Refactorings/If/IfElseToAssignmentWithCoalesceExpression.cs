// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToAssignmentWithCoalesceExpression : IfRefactoring
    {
        public IfElseToAssignmentWithCoalesceExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax condition,
            ExpressionSyntax left,
            ExpressionSyntax right1,
            ExpressionSyntax right2) : base(ifStatement)
        {
            Condition = condition;
            Left = left;
            Right1 = right1;
            Right2 = right2;
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfElseToAssignmentWithCoalesceExpression; }
        }

        public override string Title
        {
            get { return "Use coalesce expression"; }
        }

        public ExpressionSyntax Condition { get; }

        public ExpressionSyntax Left { get; }

        public ExpressionSyntax Right1 { get; }

        public ExpressionSyntax Right2 { get; }

        public override Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            BinaryExpressionSyntax coalesceExpression = CoalesceExpression(
                Right1.WithoutTrivia().Parenthesize().WithSimplifierAnnotation(),
                Right2.WithoutTrivia().Parenthesize().WithSimplifierAnnotation());

            ExpressionStatementSyntax newNode = SimpleAssignmentStatement(Left.WithoutTrivia(), coalesceExpression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken);
        }
    }
}