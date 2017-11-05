// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToYieldReturnWithExpression : IfToReturnWithExpression
    {
        public IfElseToYieldReturnWithExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression) : base(ifStatement, expression)
        {
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfElseToYieldReturnWithExpression; }
        }

        public override string Title
        {
            get { return "Replace if-else with yield return"; }
        }

        public override bool IsYield
        {
            get { return true; }
        }

        protected override StatementSyntax CreateStatement(ExpressionSyntax expression)
        {
            return YieldReturnStatement(expression);
        }
    }
}