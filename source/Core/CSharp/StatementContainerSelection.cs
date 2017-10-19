// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp
{
    public class StatementContainerSelection : SyntaxListSelection<StatementSyntax>
    {
        private StatementContainerSelection(StatementContainer container, TextSpan span, int startIndex, int endIndex)
             : base(container.Statements, span, startIndex, endIndex)
        {
            Container = container;
        }

        public StatementContainer Container { get; }

        public SyntaxList<StatementSyntax> Statements
        {
            get { return Container.Statements; }
        }

        public static StatementContainerSelection Create(BlockSyntax block, TextSpan span)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            var container = new StatementContainer(block);

            (int startIndex, int endIndex) = GetIndexes(container.Statements, span);

            return new StatementContainerSelection(container, span, startIndex, endIndex);
        }

        public static StatementContainerSelection Create(SwitchSectionSyntax switchSection, TextSpan span)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            var container = new StatementContainer(switchSection);

            (int startIndex, int endIndex) = GetIndexes(container.Statements, span);

            return new StatementContainerSelection(container, span, startIndex, endIndex);
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

        public static bool TryCreate(StatementContainer container, TextSpan span, out StatementContainerSelection selection)
        {
            selection = null;

            SyntaxList<StatementSyntax> statements = container.Statements;

            if (!statements.Any())
                return false;

            if (span.IsEmpty)
                return false;

            (int startIndex, int endIndex) = GetIndexes(statements, span);

            if (startIndex == -1)
                return false;

            selection = new StatementContainerSelection(container, span, startIndex, endIndex);
            return true;
        }
    }
}
