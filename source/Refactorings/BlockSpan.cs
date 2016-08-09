// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal class BlockSpan
    {
        private readonly SyntaxList<StatementSyntax> _statements;
        private bool _findExecuted;
        private int _firstIndex = -1;

        public BlockSpan(BlockSyntax block, TextSpan span)
        {
            Block = block;
            Span = span;

            _statements = Block.Statements;
        }

        public BlockSyntax Block { get; }

        public TextSpan Span { get; }

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
                    FindFirstSelectedStatement();
                    _findExecuted = true;
                }

                return _firstIndex;
            }
        }

        public StatementSyntax FirstSelectedStatement
        {
            get
            {
                int index = FirstSelectedStatementIndex;

                if (index != -1)
                    return _statements[index];

                return null;
            }
        }

        public IEnumerable<StatementSyntax> SelectedStatements()
        {
            int firstIndex = FirstSelectedStatementIndex;

            if (firstIndex != -1)
            {
                yield return _statements[firstIndex];

                for (int i = firstIndex + 1; i < _statements.Count; i++)
                {
                    StatementSyntax statement = _statements[i];

                    if (Span.End >= statement.Span.End)
                    {
                        yield return statement;
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
        }

        private void FindFirstSelectedStatement()
        {
            int i = 0;
            foreach (StatementSyntax statement in _statements)
            {
                TextSpan span = statement.Span;

                if (Span.Start <= span.Start)
                {
                    if (Span.End >= span.End)
                        _firstIndex = i;

                    break;
                }

                i++;
            }
        }
    }
}
