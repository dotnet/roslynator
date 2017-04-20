// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToReturnWithBooleanExpression : IfToReturnWithBooleanExpression
    {
        public IfElseToReturnWithBooleanExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2) : base(ifStatement, expression1, expression2)
        {
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfElseToReturnWithBooleanExpression; }
        }

        public override string Title
        {
            get { return "Simplify if-else"; }
        }

        public override StatementSyntax CreateStatement(ExpressionSyntax expression)
        {
            return ReturnStatement(expression);
        }
    }
}