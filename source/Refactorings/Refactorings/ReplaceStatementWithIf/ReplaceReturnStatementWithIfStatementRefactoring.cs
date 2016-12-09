// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ReplaceStatementWithIf
{
    internal class ReplaceReturnStatementWithIfStatementRefactoring : ReplaceStatementWithIfStatementRefactoring<ReturnStatementSyntax>
    {
        protected override ExpressionSyntax GetExpression(ReturnStatementSyntax statement)
        {
            return statement.Expression;
        }

        protected override ReturnStatementSyntax SetExpression(ReturnStatementSyntax statement, ExpressionSyntax expression)
        {
            return statement.WithExpression(expression);
        }

        protected override string GetTitle(ReturnStatementSyntax statement)
        {
            return "Replace return with if-else";
        }
    }
}
