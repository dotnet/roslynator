// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public class StatementsSelection : SyntaxListSelection<StatementSyntax>
    {
        private StatementsSelection(StatementsInfo info, TextSpan span, int startIndex, int endIndex)
             : base(info.Statements, span, startIndex, endIndex)
        {
            Info = info;
        }

        public StatementsInfo Info { get; }

        public SyntaxList<StatementSyntax> Statements
        {
            get { return Info.Statements; }
        }

        public static StatementsSelection Create(BlockSyntax block, TextSpan span)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            var info = new StatementsInfo(block);

            (int startIndex, int endIndex) = GetIndexes(info.Statements, span);

            return new StatementsSelection(info, span, startIndex, endIndex);
        }

        public static StatementsSelection Create(SwitchSectionSyntax switchSection, TextSpan span)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            var info = new StatementsInfo(switchSection);

            (int startIndex, int endIndex) = GetIndexes(info.Statements, span);

            return new StatementsSelection(info, span, startIndex, endIndex);
        }

        public static bool TryCreate(BlockSyntax block, TextSpan span, out StatementsSelection selectedStatements)
        {
            StatementsInfo info = SyntaxInfo.StatementsInfo(block);

            if (!info.Success)
            {
                selectedStatements = null;
                return false;
            }

            return TryCreate(info, span, out selectedStatements);
        }

        public static bool TryCreate(SwitchSectionSyntax switchSection, TextSpan span, out StatementsSelection selectedStatements)
        {
            StatementsInfo info = SyntaxInfo.StatementsInfo(switchSection);

            if (!info.Success)
            {
                selectedStatements = null;
                return false;
            }

            return TryCreate(info, span, out selectedStatements);
        }

        public static bool TryCreate(StatementsInfo statementsInfo, TextSpan span, out StatementsSelection selectedStatements)
        {
            selectedStatements = null;

            if (span.IsEmpty)
                return false;

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            if (!statements.Any())
                return false;

            (int startIndex, int endIndex) = GetIndexes(statements, span);

            if (startIndex == -1)
                return false;

            selectedStatements = new StatementsSelection(statementsInfo, span, startIndex, endIndex);
            return true;
        }
    }
}
