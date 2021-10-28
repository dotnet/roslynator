// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ConvertReturnToIf
{
    internal class ConvertYieldReturnToIfElseRefactoring : ConvertReturnToIfRefactoring<YieldStatementSyntax>
    {
        protected override ExpressionSyntax GetExpression(YieldStatementSyntax statement)
        {
            return statement.Expression;
        }

        protected override YieldStatementSyntax SetExpression(YieldStatementSyntax statement, ExpressionSyntax expression)
        {
            return statement.WithExpression(expression);
        }

        protected override string GetTitle(YieldStatementSyntax statement)
        {
            return "Convert to 'if'";
        }
    }
}
