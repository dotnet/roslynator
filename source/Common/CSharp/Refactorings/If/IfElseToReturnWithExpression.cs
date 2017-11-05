// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToReturnWithExpression : IfToReturnWithExpression
    {
        public IfElseToReturnWithExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression) : base(ifStatement, expression)
        {
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfElseToReturnWithExpression; }
        }

        public override string Title
        {
            get { return "Replace if-else with return"; }
        }

        public override bool IsYield
        {
            get { return false; }
        }

        protected override StatementSyntax CreateStatement(ExpressionSyntax expression)
        {
            return ReturnStatement(expression);
        }
    }
}