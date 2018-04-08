// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.WrapStatements
{
    internal class WrapInIfStatementRefactoring : WrapStatementsRefactoring<IfStatementSyntax>
    {
        private WrapInIfStatementRefactoring()
        {
        }

        public static WrapInIfStatementRefactoring Instance { get; } = new WrapInIfStatementRefactoring();

        public const string Title = "Wrap in condition";

        public override IfStatementSyntax CreateStatement(ImmutableArray<StatementSyntax> statements)
        {
            return IfStatement(
                IfKeyword(),
                OpenParenToken(),
                ParseExpression(""),
                CloseParenToken().WithNavigationAnnotation(),
                Block(statements),
                default(ElseClauseSyntax));
        }
    }
}
