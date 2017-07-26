// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToAssignmentWithCoalesceExpression : IfRefactoring
    {
        public IfElseToAssignmentWithCoalesceExpression(
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
            get { return RefactoringKind.IfElseToAssignmentWithCoalesceExpression; }
        }

        public override string Title
        {
            get { return "Use coalesce expression"; }
        }

        public ExpressionSyntax Left { get; }

        public ExpressionSyntax Right1 { get; }

        public ExpressionSyntax Right2 { get; }

        public override async Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            BinaryExpressionSyntax coalesceExpression = RefactoringHelper.CreateCoalesceExpression(
                semanticModel.GetTypeSymbol(Left, cancellationToken),
                Right1.WithoutTrivia(),
                Right2.WithoutTrivia(),
                IfStatement.SpanStart,
                semanticModel);

            ExpressionStatementSyntax newNode = SimpleAssignmentStatement(Left.WithoutTrivia(), coalesceExpression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}