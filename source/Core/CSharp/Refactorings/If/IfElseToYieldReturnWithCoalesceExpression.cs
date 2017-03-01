// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToYieldReturnWithCoalesceExpression : IfToReturnWithCoalesceExpression
    {
        public IfElseToYieldReturnWithCoalesceExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax right) : base(ifStatement, left, right)
        {
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfElseToYieldReturnWithCoalesceExpression; }
        }

        public override string Title
        {
            get { return "Use coalesce expression"; }
        }

        protected override StatementSyntax CreateStatement(ExpressionSyntax expression)
        {
            return YieldReturnStatement(expression);
        }
    }
}