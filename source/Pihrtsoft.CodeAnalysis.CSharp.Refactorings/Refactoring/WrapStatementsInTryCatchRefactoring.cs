// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal class WrapStatementsInTryCatchRefactoring : WrapStatementsRefactoring<TryStatementSyntax>
    {
        public override TryStatementSyntax CreateStatement(ImmutableArray<StatementSyntax> statements)
        {
            return TryStatement(
                Block(statements),
                CatchClause(
                    CatchDeclaration(
                        ParseName("System.Exception").WithSimplifierAnnotation(),
                        Identifier("ex")),
                    null,
                    Block()));
        }
    }
}
