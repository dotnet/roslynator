// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a local declaration statement with a single variable.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct SingleLocalDeclarationStatementInfo
    {
        private SingleLocalDeclarationStatementInfo(
            LocalDeclarationStatementSyntax statement,
            VariableDeclaratorSyntax declarator)
        {
            Statement = statement;
            Declarator = declarator;
        }

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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, Statement); }
        }

        internal static SingleLocalDeclarationStatementInfo Create(
            StatementSyntax statement,
            bool allowMissing = false)
        {
            if (!statement.IsKind(SyntaxKind.LocalDeclarationStatement))
                return default;

            return Create((LocalDeclarationStatementSyntax)statement, allowMissing);
        }

        internal static SingleLocalDeclarationStatementInfo Create(
            LocalDeclarationStatementSyntax localDeclarationStatement,
            bool allowMissing = false)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclarationStatement?.Declaration;

            if (!Check(variableDeclaration, allowMissing))
                return default;

            VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.SingleOrDefault(shouldThrow: false);

            if (!Check(variableDeclarator, allowMissing))
                return default;

            return new SingleLocalDeclarationStatementInfo(localDeclarationStatement, variableDeclarator);
        }

        internal static SingleLocalDeclarationStatementInfo Create(
            VariableDeclarationSyntax variableDeclaration,
            bool allowMissing = false)
        {
            if (!Check(variableDeclaration, allowMissing))
                return default;

            if (!(variableDeclaration.Parent is LocalDeclarationStatementSyntax localDeclarationStatement))
                return default;

            VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.SingleOrDefault(shouldThrow: false);

            if (!Check(variableDeclarator, allowMissing))
                return default;

            return new SingleLocalDeclarationStatementInfo(localDeclarationStatement, variableDeclarator);
        }

        internal static SingleLocalDeclarationStatementInfo Create(ExpressionSyntax value)
        {
            SyntaxNode node = value?.WalkUpParentheses().Parent;

            if (node?.Kind() != SyntaxKind.EqualsValueClause)
                return default;

            if (!(node.Parent is VariableDeclaratorSyntax declarator))
                return default;

            if (!(declarator.Parent is VariableDeclarationSyntax declaration))
                return default;

            if (declaration.Variables.Count != 1)
                return default;

            if (!(declaration.Parent is LocalDeclarationStatementSyntax localDeclarationStatement))
                return default;

            return new SingleLocalDeclarationStatementInfo(localDeclarationStatement, declarator);
        }
    }
}
