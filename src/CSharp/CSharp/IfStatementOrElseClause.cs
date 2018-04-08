// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    /// <summary>
    /// A wrapper for either an <see cref="IfStatementSyntax"/> or an <see cref="ElseClauseSyntax"/>.
    /// </summary>
    public readonly struct IfStatementOrElseClause : IEquatable<IfStatementOrElseClause>
    {
        private readonly IfStatementSyntax _ifStatement;
        private readonly ElseClauseSyntax _elseClause;

        internal IfStatementOrElseClause(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxKind kind = node.Kind();

            if (kind == SyntaxKind.IfStatement)
            {
                _ifStatement = (IfStatementSyntax)node;
                _elseClause = null;
            }
            else if (kind == SyntaxKind.ElseClause)
            {
                _elseClause = (ElseClauseSyntax)node;
                _ifStatement = null;
            }
            else
            {
                throw new ArgumentException("Node must be either an if statement or an else clause.", nameof(node));
            }
        }

        internal IfStatementOrElseClause(IfStatementSyntax ifStatement)
        {
            _ifStatement = ifStatement ?? throw new ArgumentNullException(nameof(ifStatement));
            _elseClause = null;
        }

        internal IfStatementOrElseClause(ElseClauseSyntax elseClause)
        {
            _elseClause = elseClause ?? throw new ArgumentNullException(nameof(elseClause));
            _ifStatement = null;
        }

        internal SyntaxNode Node
        {
            get { return _ifStatement ?? (SyntaxNode)_elseClause; }
        }

        /// <summary>
        /// Gets an underlying node kind.
        /// </summary>
        public SyntaxKind Kind
        {
            get
            {
                if (_ifStatement != null)
                    return SyntaxKind.IfStatement;

                if (_elseClause != null)
                    return SyntaxKind.ElseClause;

                return SyntaxKind.None;
            }
        }

        /// <summary>
        /// Determines whether this <see cref="IfStatementOrElseClause"/> is wrapping an if statement.
        /// </summary>
        public bool IsIf
        {
            get { return Kind == SyntaxKind.IfStatement; }
        }

        /// <summary>
        /// Determines whether this <see cref=" IfStatementOrElseClause"/> is wrapping an else clause.
        /// </summary>
        public bool IsElse
        {
            get { return Kind == SyntaxKind.ElseClause; }
        }

        /// <summary>
        /// Gets <see cref="IfStatementSyntax.Statement"/> or <see cref="ElseClauseSyntax.Statement"/>.
        /// </summary>
        public StatementSyntax Statement
        {
            get
            {
                if (_ifStatement != null)
                    return _ifStatement.Statement;

                if (_elseClause != null)
                    return _elseClause.Statement;

                return null;
            }
        }

        /// <summary>
        /// The node that contains the underlying node in its <see cref="SyntaxNode.ChildNodes"/> collection.
        /// </summary>
        public SyntaxNode Parent
        {
            get { return _ifStatement?.Parent ?? _elseClause?.Parent; }
        }

        /// <summary>
        /// The absolute span of this node in characters, not including its leading and trailing trivia.
        /// </summary>
        public TextSpan Span
        {
            get
            {
                if (_ifStatement != null)
                    return _ifStatement.Span;

                if (_elseClause != null)
                    return _elseClause.Span;

                return default(TextSpan);
            }
        }

        /// <summary>
        /// The absolute span of this node in characters, including its leading and trailing trivia.
        /// </summary>
        public TextSpan FullSpan
        {
            get
            {
                if (_ifStatement != null)
                    return _ifStatement.FullSpan;

                if (_elseClause != null)
                    return _elseClause.FullSpan;

                return default(TextSpan);
            }
        }

        /// <summary>
        /// Returns the underlying if statement if this <see cref="IfStatementOrElseClause"/> is wrapping if statement.
        /// </summary>
        /// <returns></returns>
        public IfStatementSyntax AsIf()
        {
            return _ifStatement;
        }

        /// <summary>
        /// Returns the underlying else clause if this <see cref="ElseClauseSyntax"/> is wrapping else clause.
        /// </summary>
        /// <returns></returns>
        public ElseClauseSyntax AsElse()
        {
            return _elseClause;
        }

        /// <summary>
        /// Returns the string representation of the underlying node, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Node?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is IfStatementOrElseClause other
                && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(IfStatementOrElseClause other)
        {
            return Node == other.Node;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return Node?.GetHashCode() ?? 0;
        }

#pragma warning disable CS1591
        public static bool operator ==(IfStatementOrElseClause left, IfStatementOrElseClause right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IfStatementOrElseClause left, IfStatementOrElseClause right)
        {
            return !left.Equals(right);
        }

        public static implicit operator IfStatementOrElseClause(IfStatementSyntax ifStatement)
        {
            return new IfStatementOrElseClause(ifStatement);
        }

        public static implicit operator IfStatementSyntax(IfStatementOrElseClause ifOrElse)
        {
            return ifOrElse.AsIf();
        }

        public static implicit operator IfStatementOrElseClause(ElseClauseSyntax elseClause)
        {
            return new IfStatementOrElseClause(elseClause);
        }

        public static implicit operator ElseClauseSyntax(IfStatementOrElseClause ifOrElse)
        {
            return ifOrElse.AsElse();
        }
#pragma warning restore CS1591
    }
}
