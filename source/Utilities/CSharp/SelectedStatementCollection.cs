// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    public class SelectedStatementCollection : SelectedNodeCollection<StatementSyntax>
    {
        private SelectedStatementCollection(StatementContainer container, TextSpan span)
             : base(container.Statements, span)
        {
            Container = container;
        }

        public static SelectedStatementCollection Create(BlockSyntax block, TextSpan span)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            var container = new BlockStatementContainer(block);

            return new SelectedStatementCollection(container, span);
        }

        public static SelectedStatementCollection Create(SwitchSectionSyntax switchSection, TextSpan span)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            var container = new SwitchSectionStatementContainer(switchSection);

            return new SelectedStatementCollection(container, span);
        }

        public StatementContainer Container { get; }
    }
}
