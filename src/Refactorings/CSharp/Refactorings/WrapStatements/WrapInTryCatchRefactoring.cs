// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.WrapStatements
{
    internal class WrapInTryCatchRefactoring : WrapStatementsRefactoring<TryStatementSyntax>
    {
        public const string Title = "Wrap in try-catch";

        private WrapInTryCatchRefactoring()
        {
        }

        public static WrapInTryCatchRefactoring Instance { get; } = new WrapInTryCatchRefactoring();

        public override TryStatementSyntax CreateStatement(ImmutableArray<StatementSyntax> statements)
        {
            statements = statements.Replace(statements[0], statements[0].WithNavigationAnnotation());

            return TryStatement(
                Block(List(statements)),
                CatchClause(
                    CatchDeclaration(
                        ParseName(MetadataNames.System_Exception).WithSimplifierAnnotation(),
                        Identifier("ex")),
                    default(CatchFilterClauseSyntax),
                    Block()));
        }
    }
}
