// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about local declaration statement.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct LocalDeclarationStatementInfo : IEquatable<LocalDeclarationStatementInfo>
    {
        private LocalDeclarationStatementInfo(LocalDeclarationStatementSyntax statement)
        {
            Statement = statement;
        }

        /// <summary>
        /// The local declaration statement.
        /// </summary>
        public LocalDeclarationStatementSyntax Statement { get; }

        /// <summary>
        /// The modifier list.
        /// </summary>
        public SyntaxTokenList Modifiers
        {
            get { return Statement?.Modifiers ?? default(SyntaxTokenList); }
        }

        /// <summary>
        /// The type of the declaration.
        /// </summary>
        public TypeSyntax Type
        {
            get { return Statement?.Declaration.Type; }
        }

        /// <summary>
        /// The variable declaration.
        /// </summary>
        public VariableDeclarationSyntax Declaration
        {
            get { return Statement?.Declaration; }
        }

        /// <summary>
        /// A list of variables.
        /// </summary>
        public SeparatedSyntaxList<VariableDeclaratorSyntax> Variables
        {
            get { return Statement?.Declaration.Variables ?? default(SeparatedSyntaxList<VariableDeclaratorSyntax>); }
        }

        /// <summary>
        /// The semicolon token.
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return ToDebugString(Success, this, Statement); }
        }

        internal static LocalDeclarationStatementInfo Create(
            LocalDeclarationStatementSyntax localDeclarationStatement,
            bool allowMissing = false)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclarationStatement?.Declaration;

            if (!Check(variableDeclaration, allowMissing))
                return default;

            TypeSyntax type = variableDeclaration.Type;

            if (!Check(type, allowMissing))
                return default;

            if (!variableDeclaration.Variables.Any())
                return default;

            return new LocalDeclarationStatementInfo(localDeclarationStatement);
        }

        internal static LocalDeclarationStatementInfo Create(
            ExpressionSyntax value,
            bool allowMissing = false)
        {
            SyntaxNode node = value?.WalkUpParentheses().Parent;

            if (node?.Kind() != SyntaxKind.EqualsValueClause)
                return default;

            node = node.Parent;

            if (node?.Kind() != SyntaxKind.VariableDeclarator)
                return default;

            if (!(node?.Parent is VariableDeclarationSyntax declaration))
                return default;

            TypeSyntax type = declaration.Type;

            if (!Check(type, allowMissing))
                return default;

            if (!(declaration.Parent is LocalDeclarationStatementSyntax localDeclarationStatement))
                return default;

            return new LocalDeclarationStatementInfo(localDeclarationStatement);
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
            return obj is LocalDeclarationStatementInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(LocalDeclarationStatementInfo other)
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

        public static bool operator ==(in LocalDeclarationStatementInfo info1, in LocalDeclarationStatementInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in LocalDeclarationStatementInfo info1, in LocalDeclarationStatementInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
