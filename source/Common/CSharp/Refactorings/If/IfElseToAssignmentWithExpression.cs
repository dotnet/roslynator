// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToAssignmentWithExpression : IfRefactoring
    {
        public IfElseToAssignmentWithExpression(
            IfStatementSyntax ifStatement,
            ExpressionStatementSyntax expressionStatement) : base(ifStatement)
        {
            ExpressionStatement = expressionStatement;
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfElseToAssignmentWithExpression; }
        }

        public override string Title
        {
            get { return "Replace if-else with assignment"; }
        }

        public ExpressionStatementSyntax ExpressionStatement { get; }

        public override Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionStatementSyntax newNode = ExpressionStatement
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken);
        }
    }
}