// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal class BlockStatementContainer : StatementContainer
    {
        private readonly BlockSyntax _block;

        public BlockStatementContainer(BlockSyntax block)
            : base(block)
        {
            _block = block;
        }

        public override SyntaxList<StatementSyntax> Statements
        {
            get { return _block.Statements; }
        }

        public override SyntaxNode NodeWithStatements(SyntaxList<StatementSyntax> statements)
        {
            return _block.WithStatements(statements);
        }

        public override StatementContainer WithStatements(SyntaxList<StatementSyntax> statements)
        {
            return new BlockStatementContainer(_block.WithStatements(statements));
        }
    }
}
