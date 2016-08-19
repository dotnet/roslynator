// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal class BlockSpan
    {
        private bool _findExecuted;
        private int _firstIndex = -1;
        private int _lastIndex = -1;

        public BlockSpan(BlockSyntax block, TextSpan span)
        {
            Block = block;
            Span = span;
            Statements = Block.Statements;
        }

        public BlockSyntax Block { get; }
        public TextSpan Span { get; }
        public SyntaxList<StatementSyntax> Statements { get; }

        public bool HasSelectedStatement
        {
            get { return FirstSelectedStatementIndex != -1; }
        }

        public int FirstSelectedStatementIndex
        {
            get
            {
                if (!_findExecuted)
                {
                    FindSelectedStatements();
                    _findExecuted = true;
                }

                return _firstIndex;
            }
        }

        public int LastSelectedStatementIndex
        {
            get
            {
                if (!_findExecuted)
                {
                    FindSelectedStatements();
                    _findExecuted = true;
                }

                return _lastIndex;
            }
        }

        public StatementSyntax FirstSelectedStatement
        {
            get
            {
                int index = FirstSelectedStatementIndex;

                if (index != -1)
                    return Statements[index];

                return null;
            }
        }

        public StatementSyntax LastSelectedStatement
        {
            get
            {
                int index = LastSelectedStatementIndex;

                if (index != -1)
                    return Statements[index];

                return null;
            }
        }

        public IEnumerable<StatementSyntax> SelectedStatements()
        {
            int firstIndex = FirstSelectedStatementIndex;

            if (firstIndex != -1)
            {
                int lastIndex = LastSelectedStatementIndex;

                for (int i = firstIndex; i <= lastIndex; i++)
                    yield return Statements[i];
            }
        }

        private void FindSelectedStatements()
        {
            SyntaxList<StatementSyntax>.Enumerator en = Statements.GetEnumerator();

            int i = 0;

            while (en.MoveNext()
                && Span.Start >= en.Current.FullSpan.End)
            {
                i++;
            }

            if (Span.Start <= en.Current.Span.Start)
            {
                int j = i;

                while (Span.End > en.Current.FullSpan.End
                    && en.MoveNext())
                {
                    j++;
                }

                if (Span.End >= en.Current.Span.End)
                {
                    _firstIndex = i;
                    _lastIndex = j;
                }
            }
        }
    }
}
