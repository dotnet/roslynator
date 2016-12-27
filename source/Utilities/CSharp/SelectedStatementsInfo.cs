// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    public class SelectedStatementsInfo : SelectedNodesInfo<StatementSyntax>
    {
        private SelectedStatementsInfo(StatementContainer container, TextSpan span)
             : base(container.Statements, span)
        {
            Container = container;
        }

        public static SelectedStatementsInfo Create(BlockSyntax block, TextSpan span)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            var container = new BlockStatementContainer(block);

            return new SelectedStatementsInfo(container, span);
        }

        public static SelectedStatementsInfo Create(SwitchSectionSyntax switchSection, TextSpan span)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            var container = new SwitchSectionStatementContainer(switchSection);

            return new SelectedStatementsInfo(container, span);
        }

        public SyntaxNode Node
        {
            get { return Container.Node; }
        }

        public StatementContainer Container { get; }
    }
}
