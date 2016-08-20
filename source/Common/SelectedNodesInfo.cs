// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis
{
    public class SelectedNodesInfo<TNode> where TNode : SyntaxNode
    {
        private bool _findExecuted;
        private int _firstIndex = -1;
        private int _lastIndex = -1;

        public SelectedNodesInfo(SyntaxList<TNode> list, TextSpan span)
        {
            Span = span;
            List = list;
        }

        public TextSpan Span { get; }
        public SyntaxList<TNode> List { get; }

        public bool IsAnySelected
        {
            get { return FirstSelectedNodeIndex != -1; }
        }

        public bool IsSingleSelected
        {
            get
            {
                int firstIndex = FirstSelectedNodeIndex;

                return firstIndex != -1
                    && LastSelectedNodeIndex == firstIndex;
            }
        }

        public bool AreManySelected
        {
            get
            {
                int firstIndex = FirstSelectedNodeIndex;

                return firstIndex != -1
                    && LastSelectedNodeIndex > firstIndex;
            }
        }

        public int SelectedCount
        {
            get
            {
                int firstIndex = FirstSelectedNodeIndex;

                if (firstIndex != -1)
                    return LastSelectedNodeIndex - firstIndex + 1;

                return 0;
            }
        }

        public int FirstSelectedNodeIndex
        {
            get
            {
                if (!_findExecuted)
                {
                    FindSelectedNodes();
                    _findExecuted = true;
                }

                return _firstIndex;
            }
        }

        public int LastSelectedNodeIndex
        {
            get
            {
                if (!_findExecuted)
                {
                    FindSelectedNodes();
                    _findExecuted = true;
                }

                return _lastIndex;
            }
        }

        public TNode FirstSelectedNode
        {
            get
            {
                int index = FirstSelectedNodeIndex;

                if (index != -1)
                    return List[index];

                return null;
            }
        }

        public TNode LastSelectedNode
        {
            get
            {
                int index = LastSelectedNodeIndex;

                if (index != -1)
                    return List[index];

                return null;
            }
        }

        public IEnumerable<TNode> SelectedNodes()
        {
            int firstIndex = FirstSelectedNodeIndex;

            if (firstIndex != -1)
            {
                int lastIndex = LastSelectedNodeIndex;

                for (int i = firstIndex; i <= lastIndex; i++)
                    yield return List[i];
            }
        }

        private void FindSelectedNodes()
        {
            SyntaxList<TNode>.Enumerator en = List.GetEnumerator();

            if (en.MoveNext())
            {
                int i = 0;

                while (Span.Start >= en.Current.FullSpan.End
                    && en.MoveNext())
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
}
