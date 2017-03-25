// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public interface IStatementContainer
    {
        SyntaxNode Node { get; }
        SyntaxList<StatementSyntax> Statements { get; }

        SyntaxNode NodeWithStatements(IEnumerable<StatementSyntax> statements);
        SyntaxNode NodeWithStatements(SyntaxList<StatementSyntax> statements);

        IStatementContainer WithStatements(IEnumerable<StatementSyntax> statements);
        IStatementContainer WithStatements(SyntaxList<StatementSyntax> statements);
    }
}