// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis
{
    public class SelectedStatementsInfo : SelectedNodesInfo<StatementSyntax>
    {
        public SelectedStatementsInfo(BlockSyntax block, TextSpan span)
             : base(block.Statements, span)
        {
            Block = block;
        }

        public BlockSyntax Block { get; }
    }
}
