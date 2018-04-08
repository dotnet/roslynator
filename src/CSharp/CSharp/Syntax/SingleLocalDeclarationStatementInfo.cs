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
    /// Provides information about a local declaration statement with a single variable.
    /// </summary>
    public readonly struct SingleLocalDeclarationStatementInfo : IEquatable<SingleLocalDeclarationStatementInfo>
    {
        private SingleLocalDeclarationStatementInfo(
            LocalDeclarationStatementSyntax statement,
            VariableDeclaratorSyntax declarator)
        {
            Statement = statement;
            Declarator = declarator;
        }

        private static SingleLocalDeclarationStatementInfo Default { get; } = new SingleLocalDeclarationStatementInfo();

        /// <summary>
        /// The local declaration statement.
        /// </summary>
        public LocalDeclarationStatementSyntax Statement { get; }

        /// <summary>
        /// The variable declarator.
        /// </summary>
        public VariableDeclaratorSyntax Declarator { get; }

        /// <summary>
        /// The variable declaration.
        /// </summary>
        public VariableDeclarationSyntax Declaration
        {
            get { return Statement?.Declaration; }
        }

        /// <summary>
        /// The variable initializer, if any.
        /// </summary>
        public EqualsValueClauseSyntax Initializer
        {
            get { return Declarator?.Initializer; }
        }

        /// <summary>
        /// The initialized value, if any.
        /// </summary>
        public ExpressionSyntax Value
        {
            get { return Initializer?.Value; }
        }

        /// <summary>
        /// The modifier list.
        /// </summary>
        public SyntaxTokenList Modifiers
        {
            get { return Statement?.Modifiers ?? default(SyntaxTokenList); }
        }

        /// <summary>
        /// The type of a declaration.
        /// </summary>
        public TypeSyntax Type
        {
            get { return Statement?.Declaration.Type; }
        }

        /// <summary>
        /// Variable identifier.
        /// </summary>
        public SyntaxToken Identifier
        {
            get { return Declarator?.Identifier ?? default(SyntaxToken); }
        }

        /// <summary>
        /// Variable name.
        /// </summary>
        public string IdentifierText
        {
            get { return Identifier.ValueText; }
        }

        /// <summary>
        /// The equals token.
        /// </summary>
        public SyntaxToken EqualsToken
        {
            get { return Initializer?.EqualsToken ?? default(SyntaxToken); }
        }

        /// <summary>
        /// The semicolon.
        /// </summary>
        public SyntaxToken SemicolonToken
        {
            get { return Statement?.SemicolonToken ?? default(SyntaxToken); }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return Statement != null; }
        }

        internal static SingleLocalDeclarationStatementInfo Create(
            LocalDeclarationStatementSyntax localDeclarationStatement,
            bool allowMissing = false)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclarationStatement?.Declaration;

            if (!Check(variableDeclaration, allowMissing))
                return Default;

            VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.SingleOrDefault(shouldThrow: false);

            if (!Check(variableDeclarator, allowMissing))
                return Default;

            return new SingleLocalDeclarationStatementInfo(localDeclarationStatement, variableDeclarator);
        }

        internal static SingleLocalDeclarationStatementInfo Create(
            VariableDeclarationSyntax variableDeclaration,
            bool allowMissing = false)
        {
            if (!Check(variableDeclaration, allowMissing))
                return Default;

            if (!(variableDeclaration.Parent is LocalDeclarationStatementSyntax localDeclarationStatement))
                return Default;

            VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.SingleOrDefault(shouldThrow: false);

            if (!Check(variableDeclarator, allowMissing))
                return Default;

            return new SingleLocalDeclarationStatementInfo(localDeclarationStatement, variableDeclarator);
        }

        internal static SingleLocalDeclarationStatementInfo Create(ExpressionSyntax value)
        {
            SyntaxNode node = value?.WalkUpParentheses().Parent;

            if (node?.Kind() != SyntaxKind.EqualsValueClause)
                return Default;

            if (!(node.Parent is VariableDeclaratorSyntax declarator))
                return Default;

            if (!(declarator.Parent is VariableDeclarationSyntax declaration))
                return Default;

            if (declaration.Variables.Count != 1)
                return Default;

            if (!(declaration.Parent is LocalDeclarationStatementSyntax localDeclarationStatement))
                return Default;

            return new SingleLocalDeclarationStatementInfo(localDeclarationStatement, declarator);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Statement?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is SingleLocalDeclarationStatementInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(SingleLocalDeclarationStatementInfo other)
        {
            return EqualityComparer<LocalDeclarationStatementSyntax>.Default.Equals(Statement, other.Statement);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<LocalDeclarationStatementSyntax>.Default.GetHashCode(Statement);
        }

        public static bool operator ==(SingleLocalDeclarationStatementInfo info1, SingleLocalDeclarationStatementInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(SingleLocalDeclarationStatementInfo info1, SingleLocalDeclarationStatementInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
