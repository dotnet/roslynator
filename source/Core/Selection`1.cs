// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public abstract class Selection<T> : IEnumerable, IEnumerable<T>
    {
        protected Selection(IReadOnlyList<T> items, TextSpan span, int startIndex, int endIndex)
        {
            Items = items;
            Span = span;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public TextSpan Span { get; }

        public IReadOnlyList<T> Items { get; }

        public int StartIndex { get; }

        public int EndIndex { get; }

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

        public ImmutableArray<T> SelectedItems
        {
            get
            {
                if (!Any())
                    return ImmutableArray<T>.Empty;

                ImmutableArray<T>.Builder builder = ImmutableArray.CreateBuilder<T>(Count);

                for (int i = StartIndex; i <= EndIndex; i++)
                    builder.Add(Items[i]);

                return builder.ToImmutable();
            }
        }

        public bool Any()
        {
            return StartIndex != -1;
        }

        public T First()
        {
            return Items[StartIndex];
        }

        public T FirstOrDefault()
        {
            if (Any())
            {
                return Items[StartIndex];
            }
            else
            {
                return default(T);
            }
        }

        public T Last()
        {
            return Items[EndIndex];
        }

        public T LastOrDefault()
        {
            if (Any())
            {
                return Items[EndIndex];
            }
            else
            {
                return default(T);
            }
        }

        private IEnumerable<T> Enumerate()
        {
            if (Any())
            {
                for (int i = StartIndex; i <= EndIndex; i++)
                    yield return Items[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
