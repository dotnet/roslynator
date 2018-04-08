// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a simple if-else.
    /// Simple if-else is defined as follows: it is not a child of an else clause and it has an else clause and the else clause does not continue with another if statement.
    /// </summary>
    public readonly struct SimpleIfElseInfo : IEquatable<SimpleIfElseInfo>
    {
        private SimpleIfElseInfo(
            IfStatementSyntax ifStatement,
            ExpressionSyntax condition,
            StatementSyntax whenTrue,
            StatementSyntax whenFalse)
        {
            IfStatement = ifStatement;
            Condition = condition;
            WhenTrue = whenTrue;
            WhenFalse = whenFalse;
        }

        private static SimpleIfElseInfo Default { get; } = new SimpleIfElseInfo();

        /// <summary>
        /// The if statement.
        /// </summary>
        public IfStatementSyntax IfStatement { get; }

        /// <summary>
        /// The condition.
        /// </summary>
        public ExpressionSyntax Condition { get; }

        /// <summary>
        /// The statement that is executed if the condition evaluates to true.
        /// </summary>
        public StatementSyntax WhenTrue { get; }

        /// <summary>
        /// The statement that is executed if the condition evaluates to false.
        /// </summary>
        public StatementSyntax WhenFalse { get; }

        /// <summary>
        /// The else clause.
        /// </summary>
        public ElseClauseSyntax Else
        {
            get { return IfStatement?.Else; }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return IfStatement != null; }
        }

        internal static SimpleIfElseInfo Create(
            IfStatementSyntax ifStatement,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (ifStatement?.IsParentKind(SyntaxKind.ElseClause) != false)
                return Default;

            StatementSyntax whenFalse = ifStatement.Else?.Statement;

            if (!Check(whenFalse, allowMissing))
                return Default;

            if (whenFalse.IsKind(SyntaxKind.IfStatement))
                return Default;

            StatementSyntax whenTrue = ifStatement.Statement;

            if (!Check(whenTrue, allowMissing))
                return Default;

            ExpressionSyntax condition = WalkAndCheck(ifStatement.Condition, walkDownParentheses, allowMissing);

            if (condition == null)
                return Default;

            return new SimpleIfElseInfo(ifStatement, condition, whenTrue, whenFalse);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return IfStatement?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is SimpleIfElseInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(SimpleIfElseInfo other)
        {
            return EqualityComparer<IfStatementSyntax>.Default.Equals(IfStatement, other.IfStatement);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<IfStatementSyntax>.Default.GetHashCode(IfStatement);
        }

        public static bool operator ==(SimpleIfElseInfo info1, SimpleIfElseInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(SimpleIfElseInfo info1, SimpleIfElseInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
