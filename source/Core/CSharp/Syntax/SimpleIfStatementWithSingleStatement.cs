// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    internal struct SimpleIfStatementWithSingleStatement : IEquatable<SimpleIfStatementWithSingleStatement>
    {
        public SimpleIfStatementWithSingleStatement(ExpressionSyntax condition, StatementSyntax statement, StatementSyntax singleStatement)
        {
            Condition = condition;
            Statement = statement;
            SingleStatement = singleStatement;
        }

        public IfStatementSyntax IfStatement
        {
            get { return (IfStatementSyntax)Node; }
        }

        public ExpressionSyntax Condition { get; }

        public StatementSyntax Statement { get; }

        public StatementSyntax SingleStatement { get; }

        private SyntaxNode Node
        {
            get { return Condition?.Parent; }
        }

        public static SimpleIfStatementWithSingleStatement Create(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            if (!ifStatement.IsSimpleIf())
                throw new ArgumentException("", nameof(ifStatement));

            StatementSyntax statement = ifStatement.Statement;

            if (statement == null)
                throw new ArgumentException("", nameof(ifStatement));

            if (statement.IsKind(SyntaxKind.Block))
            {
                StatementSyntax singleStatement = ((BlockSyntax)statement).SingleStatementOrDefault();

                if (singleStatement == null)
                    throw new ArgumentException("", nameof(ifStatement));

                return new SimpleIfStatementWithSingleStatement(ifStatement.Condition, statement, singleStatement);
            }
            else
            {
                return new SimpleIfStatementWithSingleStatement(ifStatement.Condition, statement, statement);
            }
        }

        public static bool TryCreate(SyntaxNode ifStatement, out SimpleIfStatementWithSingleStatement result)
        {
            if (ifStatement?.IsKind(SyntaxKind.IfStatement) == true)
                return TryCreateCore((IfStatementSyntax)ifStatement, out result);

            result = default(SimpleIfStatementWithSingleStatement);
            return false;
        }

        public static bool TryCreate(IfStatementSyntax ifStatement, out SimpleIfStatementWithSingleStatement result)
        {
            if (ifStatement != null)
                return TryCreateCore(ifStatement, out result);

            result = default(SimpleIfStatementWithSingleStatement);
            return false;
        }

        private static bool TryCreateCore(IfStatementSyntax ifStatement, out SimpleIfStatementWithSingleStatement result)
        {
            if (ifStatement.IsSimpleIf())
            {
                StatementSyntax statement = ifStatement.Statement;

                if (statement != null)
                {
                    if (statement.IsKind(SyntaxKind.Block))
                    {
                        StatementSyntax singleStatement = ((BlockSyntax)statement).SingleStatementOrDefault();

                        if (singleStatement != null)
                        {
                            result = new SimpleIfStatementWithSingleStatement(ifStatement.Condition, statement, singleStatement);
                            return true;
                        }
                    }
                    else
                    {
                        result = new SimpleIfStatementWithSingleStatement(ifStatement.Condition, statement, statement);
                        return true;
                    }
                }
            }

            result = default(SimpleIfStatementWithSingleStatement);
            return false;
        }

        public override string ToString()
        {
            return Node?.ToString() ?? base.ToString();
        }

        public bool Equals(SimpleIfStatementWithSingleStatement other)
        {
            return Node == other.Node;
        }

        public override bool Equals(object obj)
        {
            return obj is SimpleIfStatementWithSingleStatement
                && Equals((SimpleIfStatementWithSingleStatement)obj);
        }

        public override int GetHashCode()
        {
            return Node?.GetHashCode() ?? 0;
        }

        public static bool operator ==(SimpleIfStatementWithSingleStatement left, SimpleIfStatementWithSingleStatement right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SimpleIfStatementWithSingleStatement left, SimpleIfStatementWithSingleStatement right)
        {
            return !left.Equals(right);
        }
    }
}
