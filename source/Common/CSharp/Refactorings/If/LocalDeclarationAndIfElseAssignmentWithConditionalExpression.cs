// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class LocalDeclarationAndIfElseAssignmentWithConditionalExpression : ToAssignmentWithConditionalExpression<LocalDeclarationStatementSyntax>
    {
        internal LocalDeclarationAndIfElseAssignmentWithConditionalExpression(
            LocalDeclarationStatementSyntax statement,
            IfStatementSyntax ifStatement,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse) : base(statement, ifStatement, whenTrue, whenFalse)
        {
        }

        protected override LocalDeclarationStatementSyntax CreateNewStatement()
        {
            ConditionalExpressionSyntax conditionalExpression = IfRefactoringHelper.CreateConditionalExpression(IfStatement.Condition, WhenTrue, WhenFalse);

            VariableDeclaratorSyntax declarator = Statement.Declaration.Variables[0];

            EqualsValueClauseSyntax initializer = declarator.Initializer;

            EqualsValueClauseSyntax newInitializer = (initializer != null)
                ? initializer.WithValue(conditionalExpression)
                : EqualsValueClause(conditionalExpression);

            return Statement.ReplaceNode(declarator, declarator.WithInitializer(newInitializer));
        }
    }
}
