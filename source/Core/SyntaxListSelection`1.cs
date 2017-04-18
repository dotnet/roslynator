// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public class SyntaxListSelection<TNode> : IEnumerable, IEnumerable<TNode> where TNode : SyntaxNode
    {
        public SyntaxListSelection(SyntaxList<TNode> list, TextSpan span)
        {
            UnderlyingList = list;
            Span = span;

            IndexPair indexes = GetIndexes(list, span);

            StartIndex = indexes.StartIndex;
            EndIndex = indexes.EndIndex;
        }

        protected SyntaxListSelection(SyntaxList<TNode> list, TextSpan span, int startIndex, int endIndex)
        {
            UnderlyingList = list;
            Span = span;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        protected static IndexPair GetIndexes(SyntaxList<TNode> list, TextSpan span)
        {
            SyntaxList<TNode>.Enumerator en = list.GetEnumerator();

            if (en.MoveNext())
            {
                int i = 0;

                while (span.Start >= en.Current.FullSpan.End
                    && en.MoveNext())
                {
                    i++;
                }

                if (span.Start >= en.Current.FullSpan.Start
                    && span.Start <= en.Current.Span.Start)
                {
                    int j = i;

                    while (span.End > en.Current.FullSpan.End
                        && en.MoveNext())
                    {
                        j++;
                    }

                    if (span.End >= en.Current.Span.End
                        && span.End <= en.Current.FullSpan.End)
                    {
                        return new IndexPair(i, j);
                    }
                }
            }

            return new IndexPair(-1, -1);
        }

        public static bool TryCreate(SyntaxList<TNode> list, TextSpan span, out SyntaxListSelection<TNode> selectedNodes)
        {
            if (list.Any())
            {
                IndexPair indexes = GetIndexes(list, span);

                if (indexes.IsValid)
                {
                    selectedNodes = new SyntaxListSelection<TNode>(list, span, indexes.StartIndex, indexes.EndIndex);
                    return true;
                }
            }

            selectedNodes = null;
            return false;
        }

        public TextSpan Span { get; }

        public SyntaxList<TNode> UnderlyingList { get; }

        public int StartIndex { get; } = -1;

        public int EndIndex { get; } = -1;

        public int Count
        {
            get
            {
                if (Any())
                {
                    return EndIndex - StartIndex + 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool Any()
        {
            return StartIndex != -1;
        }

        public TNode First()
        {
            return UnderlyingList[StartIndex];
        }

        public TNode FirstOrDefault()
        {
            if (Any())
            {
                return UnderlyingList[StartIndex];
            }
            else
            {
                return null;
            }
        }

        public TNode Last()
        {
            return UnderlyingList[EndIndex];
        }

        public TNode LastOrDefault()
        {
            if (Any())
            {
                return UnderlyingList[EndIndex];
            }
            else
            {
                return null;
            }
        }

        public ImmutableArray<TNode> Nodes
        {
            get
            {
                if (Any())
                {
                    ImmutableArray<TNode>.Builder builder = ImmutableArray.CreateBuilder<TNode>(Count);

                    for (int i = StartIndex; i <= EndIndex; i++)
                        builder.Add(UnderlyingList[i]);

                    return builder.ToImmutable();
                }

                return ImmutableArray<TNode>.Empty;
            }
        }

        private IEnumerable<TNode> EnumerateNodes()
        {
            if (Any())
            {
                for (int i = StartIndex; i <= EndIndex; i++)
                    yield return UnderlyingList[i];
            }
        }

        public IEnumerator<TNode> GetEnumerator()
        {
            return EnumerateNodes().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected class IndexPair
        {
            public IndexPair(int startIndex, int endIndex)
            {
                StartIndex = startIndex;
                EndIndex = endIndex;
            }

            public bool IsValid
            {
                get { return StartIndex != -1; }
            }

            public int StartIndex { get; }
            public int EndIndex { get; }
        }
    }
}
