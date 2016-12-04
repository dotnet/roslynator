// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ReplaceStatementWithIf
{
    internal class ReplaceExpressionStatementWithIfStatementRefactoring : ReplaceStatementWithIfStatementRefactoring<ExpressionStatementSyntax>
    {
        protected override ExpressionSyntax GetExpression(ExpressionStatementSyntax statement)
        {
            ExpressionSyntax expression = statement.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
            {
                var assignment = (AssignmentExpressionSyntax)expression;

                if (assignment.Left?.IsMissing == false)
                {
                    ExpressionSyntax right = assignment.Right;

                    if (right?.IsKind(SyntaxKind.ConditionalExpression) == true)
                        return right;
                }
            }

            return null;
        }

        protected override ExpressionStatementSyntax SetExpression(ExpressionStatementSyntax statement, ExpressionSyntax expression)
        {
            var assignment = (AssignmentExpressionSyntax)statement.Expression;

            return statement.ReplaceNode(assignment.Right, expression);
        }

        protected override string GetTitle(ExpressionStatementSyntax statement)
        {
            return "Replace assignment with if-else";
        }
    }
}
