// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class UsingStatementSyntaxExtensions
    {
        public static SyntaxNode DeclarationOrExpression(this UsingStatementSyntax usingStatement)
        {
            if (usingStatement == null)
                throw new ArgumentNullException(nameof(usingStatement));

            if (usingStatement.Declaration != null)
                return usingStatement.Declaration;
            else
                return usingStatement.Expression;
        }

        public static IEnumerable<StatementSyntax> GetStatements(this UsingStatementSyntax usingStatement)
        {
            if (usingStatement == null)
                throw new ArgumentNullException(nameof(usingStatement));

            if (usingStatement.Statement == null)
                yield break;

            if (usingStatement.Statement.IsKind(SyntaxKind.Block))
            {
                SyntaxList<StatementSyntax> statements = ((BlockSyntax)usingStatement.Statement).Statements;

                for (int i = 0; i < statements.Count; i++)
                    yield return statements[i];
            }
            else
            {
                yield return usingStatement.Statement;
            }
        }
    }
}
