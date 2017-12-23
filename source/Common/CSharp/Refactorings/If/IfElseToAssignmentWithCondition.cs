// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToAssignmentWithCondition : IfRefactoring
    {
        public IfElseToAssignmentWithCondition(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax right,
            bool negate) : base(ifStatement)
        {
            Left = left;
            Right = right;
            Negate = negate;
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfElseToAssignmentWithExpression; }
        }

        public override string Title
        {
            get { return "Replace if-else with assignment"; }
        }

        public ExpressionSyntax Left { get; }

        public ExpressionSyntax Right { get; }

        public bool Negate { get; }

        public override async Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax right = Right.WithoutTrivia();

            if (Negate)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                right = CSharpUtility.LogicallyNegate(right, semanticModel, cancellationToken);
            }

            ExpressionStatementSyntax newNode = SimpleAssignmentStatement(Left.WithoutTrivia(), right)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}