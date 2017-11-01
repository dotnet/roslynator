// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    public struct LocalDeclarationStatementInfo
    {
        private static LocalDeclarationStatementInfo Default { get; } = new LocalDeclarationStatementInfo();

        private LocalDeclarationStatementInfo(
            LocalDeclarationStatementSyntax statement,
            VariableDeclarationSyntax declaration,
            TypeSyntax type)
        {
            Statement = statement;
            Declaration = declaration;
            Type = type;
        }

        public LocalDeclarationStatementSyntax Statement { get; }

        public SyntaxTokenList Modifiers
        {
            get { return Statement?.Modifiers ?? default(SyntaxTokenList); }
        }

        public TypeSyntax Type { get; }

        public VariableDeclarationSyntax Declaration { get; }

        public SeparatedSyntaxList<VariableDeclaratorSyntax> Variables
        {
            get { return Declaration?.Variables ?? default(SeparatedSyntaxList<VariableDeclaratorSyntax>); }
        }

        public SyntaxToken SemicolonToken
        {
            get { return Statement?.SemicolonToken ?? default(SyntaxToken); }
        }

        public bool Success
        {
            get { return Statement != null; }
        }

        internal static LocalDeclarationStatementInfo Create(
            LocalDeclarationStatementSyntax localDeclarationStatement,
            bool allowMissing = false)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclarationStatement?.Declaration;

            if (!Check(variableDeclaration, allowMissing))
                return Default;

            TypeSyntax type = variableDeclaration.Type;

            if (!Check(type, allowMissing))
                return Default;

            if (!variableDeclaration.Variables.Any())
                return Default;

            return new LocalDeclarationStatementInfo(localDeclarationStatement, variableDeclaration, type);
        }

        internal static LocalDeclarationStatementInfo Create(
            ExpressionSyntax value,
            bool allowMissing = false)
        {
            SyntaxNode node = value?.WalkUpParentheses().Parent;

            if (node?.Kind() != SyntaxKind.EqualsValueClause)
                return Default;

            node = node.Parent;

            if (node?.Kind() != SyntaxKind.VariableDeclarator)
                return Default;

            if (!(node?.Parent is VariableDeclarationSyntax declaration))
                return Default;

            TypeSyntax type = declaration.Type;

            if (!Check(type, allowMissing))
                return Default;

            if (!(declaration.Parent is LocalDeclarationStatementSyntax localDeclarationStatement))
                return Default;

            return new LocalDeclarationStatementInfo(localDeclarationStatement, declaration, type);
        }

        public override string ToString()
        {
            return Statement?.ToString() ?? base.ToString();
        }
    }
}
