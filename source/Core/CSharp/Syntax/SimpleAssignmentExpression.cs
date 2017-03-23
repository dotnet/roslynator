// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Syntax
{
    internal struct SimpleAssignmentExpression : IEquatable<SimpleAssignmentExpression>
    {
        private SimpleAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            Left = left;
            Right = right;
        }

        public ExpressionSyntax Left { get; }
        public ExpressionSyntax Right { get; }

        public AssignmentExpressionSyntax Expression
        {
            get { return (AssignmentExpressionSyntax)Parent; }
        }

        private SyntaxNode Parent
        {
            get { return Left?.Parent; }
        }

        public static bool TryCreate(StatementSyntax assignmentStatement, out SimpleAssignmentExpression simpleAssignment)
        {
            if (assignmentStatement?.IsKind(SyntaxKind.ExpressionStatement) == true)
                return TryCreate((ExpressionStatementSyntax)assignmentStatement, out simpleAssignment);

            simpleAssignment = default(SimpleAssignmentExpression);
            return false;
        }

        public static bool TryCreate(ExpressionStatementSyntax assignmentStatement, out SimpleAssignmentExpression simpleAssignment)
        {
            ExpressionSyntax expression = assignmentStatement?.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                return TryCreateCore((AssignmentExpressionSyntax)expression, out simpleAssignment);

            simpleAssignment = default(SimpleAssignmentExpression);
            return false;
        }

        public static bool TryCreate(AssignmentExpressionSyntax assignment, out SimpleAssignmentExpression simpleAssignment)
        {
            if (assignment?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                return TryCreateCore(assignment, out simpleAssignment);

            simpleAssignment = default(SimpleAssignmentExpression);
            return false;
        }

        private static bool TryCreateCore(AssignmentExpressionSyntax assignment, out SimpleAssignmentExpression simpleAssignment)
        {
            ExpressionSyntax left = assignment.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = assignment.Right;

                if (right?.IsMissing == false)
                {
                    simpleAssignment = new SimpleAssignmentExpression(left, right);
                    return true;
                }
            }

            simpleAssignment = default(SimpleAssignmentExpression);
            return false;
        }

        public bool Equals(SimpleAssignmentExpression other)
        {
            return Parent == other.Parent;
        }

        public override bool Equals(object obj)
        {
            return obj is SimpleAssignmentExpression
                && Equals((SimpleAssignmentExpression)obj);
        }

        public override int GetHashCode()
        {
            return Parent?.GetHashCode() ?? 0;
        }

        public static bool operator ==(SimpleAssignmentExpression left, SimpleAssignmentExpression right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SimpleAssignmentExpression left, SimpleAssignmentExpression right)
        {
            return !left.Equals(right);
        }
    }
}
