// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    internal struct BlockStatementContainer : IStatementContainer
    {
        public BlockStatementContainer(BlockSyntax block)
        {
            Block = block;
            Statements = block.Statements;
        }

        SyntaxNode IStatementContainer.Node
        {
            get { return Block; }
        }

        public BlockSyntax Block { get; }

        public SyntaxList<StatementSyntax> Statements { get; }

        public SyntaxNode NodeWithStatements(IEnumerable<StatementSyntax> statements)
        {
            return NodeWithStatements(List(statements));
        }

        public SyntaxNode NodeWithStatements(SyntaxList<StatementSyntax> statements)
        {
            return Block.WithStatements(statements);
        }

        public IStatementContainer WithStatements(IEnumerable<StatementSyntax> statements)
        {
            return WithStatements(List(statements));
        }

        public IStatementContainer WithStatements(SyntaxList<StatementSyntax> statements)
        {
            return new BlockStatementContainer(Block.WithStatements(statements));
        }
    }
}
