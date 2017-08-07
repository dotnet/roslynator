// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    internal struct SingleLocalDeclarationStatement : IEquatable<SingleLocalDeclarationStatement>
    {
        public SingleLocalDeclarationStatement(VariableDeclarationSyntax declaration, VariableDeclaratorSyntax declarator)
        {
            Declaration = declaration;
            Declarator = declarator;
        }

        public LocalDeclarationStatementSyntax Statement
        {
            get { return (LocalDeclarationStatementSyntax)Node; }
        }

        public VariableDeclarationSyntax Declaration { get; }

        public VariableDeclaratorSyntax Declarator { get; }

        public EqualsValueClauseSyntax Initializer
        {
            get { return Declarator?.Initializer; }
        }

        public SyntaxTokenList Modifiers
        {
            get { return Statement?.Modifiers ?? default(SyntaxTokenList); }
        }

        public TypeSyntax Type
        {
            get { return Declaration?.Type; }
        }

        public SyntaxToken Identifier
        {
            get { return Declarator?.Identifier ?? default(SyntaxToken); }
        }

        public string IdentifierText
        {
            get { return Declarator?.Identifier.ValueText; }
        }

        public SyntaxToken EqualsToken
        {
            get { return Initializer?.EqualsToken ?? default(SyntaxToken); }
        }

        public SyntaxToken SemicolonToken
        {
            get { return Statement?.SemicolonToken ?? default(SyntaxToken); }
        }

        private SyntaxNode Node
        {
            get { return Declaration?.Parent; }
        }

        public static SingleLocalDeclarationStatement Create(LocalDeclarationStatementSyntax localDeclarationStatement)
        {
            if (localDeclarationStatement == null)
                throw new ArgumentNullException(nameof(localDeclarationStatement));

            VariableDeclarationSyntax variableDeclaration = localDeclarationStatement.Declaration;

            if (variableDeclaration == null)
                throw new ArgumentNullException(nameof(localDeclarationStatement));

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;

            if (variables.Count != 1)
                throw new ArgumentNullException(nameof(localDeclarationStatement));

            return new SingleLocalDeclarationStatement(variableDeclaration, variables[0]);
        }

        public static bool TryCreate(SyntaxNode localDeclarationStatement, out SingleLocalDeclarationStatement result)
        {
            if (localDeclarationStatement?.IsKind(SyntaxKind.LocalDeclarationStatement) == true)
                return TryCreateCore((LocalDeclarationStatementSyntax)localDeclarationStatement, out result);

            result = default(SingleLocalDeclarationStatement);
            return false;
        }

        public static bool TryCreate(LocalDeclarationStatementSyntax localDeclarationStatement, out SingleLocalDeclarationStatement result)
        {
            if (localDeclarationStatement != null)
                return TryCreateCore(localDeclarationStatement, out result);

            result = default(SingleLocalDeclarationStatement);
            return false;
        }

        public static bool TryCreateFromValue(ExpressionSyntax expression, out SingleLocalDeclarationStatement result)
        {
            SyntaxNode parent = expression?.WalkUpParentheses().Parent;

            if (parent?.IsKind(SyntaxKind.EqualsValueClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.VariableDeclarator) == true)
                {
                    var declarator = (VariableDeclaratorSyntax)parent;

                    if (declarator.IsParentKind(SyntaxKind.VariableDeclaration))
                    {
                        var declaration = (VariableDeclarationSyntax)declarator.Parent;

                        if (declaration.Variables.Count == 1
                            && declaration.IsParentKind(SyntaxKind.LocalDeclarationStatement))
                        {
                            result = new SingleLocalDeclarationStatement(declaration, declarator);
                            return true;
                        }
                    }
                }
            }

            result = default(SingleLocalDeclarationStatement);
            return false;
        }

        private static bool TryCreateCore(LocalDeclarationStatementSyntax localDeclarationStatement, out SingleLocalDeclarationStatement result)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclarationStatement.Declaration;

            if (variableDeclaration != null)
            {
                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;

                if (variables.Count == 1)
                {
                    result = new SingleLocalDeclarationStatement(variableDeclaration, variables[0]);
                    return true;
                }
            }

            result = default(SingleLocalDeclarationStatement);
            return false;
        }

        public override string ToString()
        {
            return Node?.ToString() ?? base.ToString();
        }

        public bool Equals(SingleLocalDeclarationStatement other)
        {
            return Node == other.Node;
        }

        public override bool Equals(object obj)
        {
            return obj is SingleLocalDeclarationStatement
                && Equals((SingleLocalDeclarationStatement)obj);
        }

        public override int GetHashCode()
        {
            return Node?.GetHashCode() ?? 0;
        }

        public static bool operator ==(SingleLocalDeclarationStatement left, SingleLocalDeclarationStatement right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SingleLocalDeclarationStatement left, SingleLocalDeclarationStatement right)
        {
            return !left.Equals(right);
        }
    }
}
