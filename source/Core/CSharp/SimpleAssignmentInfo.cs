// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp
{
    internal struct SimpleAssignmentInfo
    {
        private SimpleAssignmentInfo(AssignmentExpressionSyntax assignment, ExpressionSyntax left, ExpressionSyntax right)
        {
            Expression = assignment;
            Left = left;
            Right = right;
        }

        public AssignmentExpressionSyntax Expression { get; }
        public ExpressionSyntax Left { get; }
        public ExpressionSyntax Right { get; }

        public bool IsDefault
        {
            get { return Expression == null; }
        }

        public bool IsValid
        {
            get
            {
                return Left?.IsMissing == false
                    && Right?.IsMissing == false;
            }
        }

        public static SimpleAssignmentInfo FromStatement(StatementSyntax statement)
        {
            if (statement?.IsKind(SyntaxKind.ExpressionStatement) == true)
                return FromStatement((ExpressionStatementSyntax)statement);

            return default(SimpleAssignmentInfo);
        }

        public static SimpleAssignmentInfo FromStatement(ExpressionStatementSyntax expressionStatement)
        {
            ExpressionSyntax expression = expressionStatement?.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
            {
                var assignment = (AssignmentExpressionSyntax)expression;

                return new SimpleAssignmentInfo(assignment, assignment.Left, assignment.Right);
            }

            return default(SimpleAssignmentInfo);
        }
    }
}
