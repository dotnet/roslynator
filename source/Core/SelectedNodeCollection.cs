// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public class SelectedNodeCollection<TNode> : IEnumerable, IEnumerable<TNode> where TNode : SyntaxNode
    {
        private bool _findExecuted;
        private int _firstIndex = -1;
        private int _lastIndex = -1;

        public SelectedNodeCollection(SyntaxList<TNode> list, TextSpan span)
        {
            UnderlyingList = list;
            Span = span;
        }

        public TextSpan Span { get; }

        public SyntaxList<TNode> UnderlyingList { get; }

        public bool Any()
        {
            return FirstIndex != -1;
        }

        public bool IsSingle
        {
            get
            {
                int firstIndex = FirstIndex;

                return firstIndex != -1
                    && LastIndex == firstIndex;
            }
        }

        public bool IsMultiple
        {
            get
            {
                int firstIndex = FirstIndex;

                return firstIndex != -1
                    && LastIndex > firstIndex;
            }
        }

        public int Count
        {
            get
            {
                int firstIndex = FirstIndex;

                if (firstIndex != -1)
                {
                    return LastIndex - firstIndex + 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int FirstIndex
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

        public int LastIndex
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

        public TNode First
        {
            get
            {
                int firstIndex = FirstIndex;

                if (firstIndex != -1)
                {
                    return UnderlyingList[firstIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        public TNode Last
        {
            get
            {
                int lastIndex = LastIndex;

                if (lastIndex != -1)
                {
                    return UnderlyingList[lastIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        private IEnumerable<TNode> SelectedNodes()
        {
            int firstIndex = FirstIndex;

            if (firstIndex != -1)
            {
                int lastIndex = LastIndex;

                for (int i = firstIndex; i <= lastIndex; i++)
                    yield return UnderlyingList[i];
            }
        }

        private void FindSelectedNodes()
        {
            SyntaxList<TNode>.Enumerator en = UnderlyingList.GetEnumerator();

            if (en.MoveNext())
            {
                int i = 0;

                while (Span.Start >= en.Current.FullSpan.End
                    && en.MoveNext())
                {
                    i++;
                }

                if (Span.Start >= en.Current.FullSpan.Start
                    && Span.Start <= en.Current.Span.Start)
                {
                    int j = i;

                    while (Span.End > en.Current.FullSpan.End
                        && en.MoveNext())
                    {
                        j++;
                    }

                    if (Span.End >= en.Current.Span.End
                        && Span.End <= en.Current.FullSpan.End)
                    {
                        _firstIndex = i;
                        _lastIndex = j;
                    }
                }
            }
        }

        public IEnumerator<TNode> GetEnumerator()
        {
            return SelectedNodes().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
