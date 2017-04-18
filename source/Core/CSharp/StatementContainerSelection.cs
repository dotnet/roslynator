// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    public class StatementContainerSelection : SyntaxListSelection<StatementSyntax>
    {
        private StatementContainerSelection(StatementContainer container, TextSpan span)
             : base(container.Statements, span)
        {
            Container = container;
        }

        private StatementContainerSelection(StatementContainer container, TextSpan span, int startIndex, int endIndex)
             : base(container.Statements, span, startIndex, endIndex)
        {
            Container = container;
        }

        public static StatementContainerSelection Create(BlockSyntax block, TextSpan span)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            var container = new StatementContainer(block);

            return new StatementContainerSelection(container, span);
        }

        public static StatementContainerSelection Create(SwitchSectionSyntax switchSection, TextSpan span)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            var container = new StatementContainer(switchSection);

            return new StatementContainerSelection(container, span);
        }

        public static bool TryCreate(BlockSyntax block, TextSpan span, out StatementContainerSelection selectedStatements)
        {
            StatementContainer container;
            if (StatementContainer.TryCreate(block, out container))
            {
                return TryCreate(container, span, out selectedStatements);
            }
            else
            {
                selectedStatements = null;
                return false;
            }

        }

        public static bool TryCreate(SwitchSectionSyntax switchSection, TextSpan span, out StatementContainerSelection selectedStatements)
        {
            StatementContainer container;
            if (StatementContainer.TryCreate(switchSection, out container))
            {
                return TryCreate(container, span, out selectedStatements);
            }
            else
            {
                selectedStatements = null;
                return false;
            }
        }

        public static bool TryCreate(StatementContainer container, TextSpan span, out StatementContainerSelection selectedStatements)
        {
            if (container.Statements.Any())
            {
                IndexPair indexes = GetIndexes(container.Statements, span);

                if (indexes.StartIndex != -1)
                {
                    selectedStatements = new StatementContainerSelection(container, span, indexes.StartIndex, indexes.EndIndex);
                    return true;
                }
            }

            selectedStatements = null;
            return false;
        }

        public StatementContainer Container { get; }
    }
}
