// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    internal struct SimpleAssignmentStatement : IEquatable<SimpleAssignmentStatement>
    {
        public SimpleAssignmentStatement(
            AssignmentExpressionSyntax assignmentExpression,
            ExpressionSyntax left,
            ExpressionSyntax right)
        {
            AssignmentExpression = assignmentExpression;
            Left = left;
            Right = right;
        }

        public AssignmentExpressionSyntax AssignmentExpression { get; }
        public ExpressionSyntax Left { get; }
        public ExpressionSyntax Right { get; }

        public ExpressionStatementSyntax ExpressionStatement
        {
            get { return (ExpressionStatementSyntax)Node; }
        }

        private SyntaxNode Node
        {
            get { return AssignmentExpression?.Parent; }
        }

        public static SimpleAssignmentStatement Create(ExpressionStatementSyntax expressionStatement)
        {
            if (expressionStatement == null)
                throw new ArgumentNullException(nameof(expressionStatement));

            ExpressionSyntax expression = expressionStatement.Expression;

            if (expression == null
                || !expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
            {
                throw new ArgumentException("", nameof(expressionStatement));
            }

            var simpleAssignment = (AssignmentExpressionSyntax)expression;

            return new SimpleAssignmentStatement(simpleAssignment, simpleAssignment.Left, simpleAssignment.Right);
        }

        public static bool TryCreate(SyntaxNode assignmentStatement, out SimpleAssignmentStatement result)
        {
            if (assignmentStatement?.IsKind(SyntaxKind.ExpressionStatement) == true)
                return TryCreate((ExpressionStatementSyntax)assignmentStatement, out result);

            result = default(SimpleAssignmentStatement);
            return false;
        }

        public static bool TryCreate(ExpressionStatementSyntax assignmentStatement, out SimpleAssignmentStatement result)
        {
            if (assignmentStatement != null)
                return TryCreateCore(assignmentStatement, out result);

            result = default(SimpleAssignmentStatement);
            return false;
        }

        private static bool TryCreateCore(ExpressionStatementSyntax expressionStatement, out SimpleAssignmentStatement result)
        {
            ExpressionSyntax expression = expressionStatement.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
            {
                var simpleAssignment = (AssignmentExpressionSyntax)expression;

                result = new SimpleAssignmentStatement(simpleAssignment, simpleAssignment.Left, simpleAssignment.Right);
                return true;
            }

            result = default(SimpleAssignmentStatement);
            return false;
        }

        public override string ToString()
        {
            return Node?.ToString() ?? base.ToString();
        }

        public bool Equals(SimpleAssignmentStatement other)
        {
            return Node == other.Node;
        }

        public override bool Equals(object obj)
        {
            return obj is SimpleAssignmentStatement
                && Equals((SimpleAssignmentStatement)obj);
        }

        public override int GetHashCode()
        {
            return Node?.GetHashCode() ?? 0;
        }

        public static bool operator ==(SimpleAssignmentStatement left, SimpleAssignmentStatement right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SimpleAssignmentStatement left, SimpleAssignmentStatement right)
        {
            return !left.Equals(right);
        }
    }
}
