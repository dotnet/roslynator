// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    /// <summary>
    /// Represents selected nodes in a <see cref="SyntaxList{TNode}"/>.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SyntaxListSelection<TNode> : ISelection<TNode> where TNode : SyntaxNode
    {
        private SyntaxListSelection(SyntaxList<TNode> list, TextSpan span, in SelectionResult result)
            : this(list, span, result.FirstIndex, result.LastIndex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxListSelection{TNode}"/>.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="span"></param>
        /// <param name="firstIndex"></param>
        /// <param name="lastIndex"></param>
        protected SyntaxListSelection(SyntaxList<TNode> list, TextSpan span, int firstIndex, int lastIndex)
        {
            if (firstIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(firstIndex), firstIndex, "Index of the first selected node must be greater than or equal to zero.");

            if (lastIndex < firstIndex)
                throw new ArgumentOutOfRangeException(nameof(lastIndex), lastIndex, "Index of the last selected node must be greater or equal to index of the first selected node.");

            UnderlyingList = list;
            OriginalSpan = span;
            FirstIndex = firstIndex;
            LastIndex = lastIndex;
        }

        /// <summary>
        /// Gets an underlying list that contains selected nodes.
        /// </summary>
        public SyntaxList<TNode> UnderlyingList { get; }

        /// <summary>
        /// Gets the original span that was used to determine selected nodes.
        /// </summary>
        public TextSpan OriginalSpan { get; }

        /// <summary>
        /// Gets an index of the first selected node.
        /// </summary>
        public int FirstIndex { get; }

        /// <summary>
        /// Gets an index of the last selected node.
        /// </summary>
        public int LastIndex { get; }

        /// <summary>
        /// Gets a number of selected nodes.
        /// </summary>
        public int Count
        {
            get { return LastIndex - FirstIndex + 1; }
        }

        /// <summary>
        /// Gets the selected node at the specified index.
        /// </summary>
        /// <returns>The node at the specified index.</returns>
        /// <param name="index">The zero-based index of the node to get. </param>
        public TNode this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, "");
                }

                return UnderlyingList[index + FirstIndex];
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"Count = {Count} FirstIndex = {FirstIndex} LastIndex = {LastIndex} {UnderlyingList.ToString(TextSpan.FromBounds(First().SpanStart, Last().Span.End))}"; }
        }

        /// <summary>
        /// Gets the first selected node.
        /// </summary>
        /// <returns></returns>
        public TNode First()
        {
            return UnderlyingList[FirstIndex];
        }

        /// <summary>
        /// Gets the last selected node.
        /// </summary>
        /// <returns></returns>
        public TNode Last()
        {
            return UnderlyingList[LastIndex];
        }

        /// <summary>
        /// Creates a new <see cref="SyntaxListSelection{TNode}"/> based on the specified list and span.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static SyntaxListSelection<TNode> Create(SyntaxList<TNode> list, TextSpan span)
        {
            SelectionResult result = SelectionResult.Create(list, span);

            if (!result.Success)
                throw new InvalidOperationException("No selected node found.");

            return new SyntaxListSelection<TNode>(list, span, result.FirstIndex, result.LastIndex);
        }

        /// <summary>
        /// Creates a new <see cref="SyntaxListSelection{TNode}"/> based on the specified list and span.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="span"></param>
        /// <param name="selection"></param>
        /// <returns>True if the specified span contains at least one node; otherwise, false.</returns>
        public static bool TryCreate(SyntaxList<TNode> list, TextSpan span, out SyntaxListSelection<TNode> selection)
        {
            selection = Create(list, span, 1, int.MaxValue);
            return selection != null;
        }

        internal static bool TryCreate(SyntaxList<TNode> list, TextSpan span, int minCount, out SyntaxListSelection<TNode> selection)
        {
            selection = Create(list, span, minCount, int.MaxValue);
            return selection != null;
        }

        internal static bool TryCreate(SyntaxList<TNode> list, TextSpan span, int minCount, int maxCount, out SyntaxListSelection<TNode> selection)
        {
            selection = Create(list, span, minCount, maxCount);
            return selection != null;
        }

        private static SyntaxListSelection<TNode> Create(SyntaxList<TNode> list, TextSpan span, int minCount, int maxCount)
        {
            SelectionResult result = SelectionResult.Create(list, span, minCount, maxCount);

            if (!result.Success)
                return null;

            return new SyntaxListSelection<TNode>(list, span, result);
        }

        /// <summary>
        /// Returns an enumerator that iterates through selected nodes.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator()
        {
            return new EnumeratorImpl(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new EnumeratorImpl(this);
        }

        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
        [SuppressMessage("Usage", "CA2231:Overload operator equals on overriding value type Equals", Justification = "<Pending>")]
        public struct Enumerator
        {
            private readonly SyntaxListSelection<TNode> _selection;
            private int _index;

            internal Enumerator(SyntaxListSelection<TNode> selection)
            {
                _selection = selection;
                _index = -1;
            }

            public bool MoveNext()
            {
                if (_index == -1)
                {
                    _index = _selection.FirstIndex;
                    return true;
                }
                else
                {
                    int newIndex = _index + 1;
                    if (newIndex <= _selection.LastIndex)
                    {
                        _index = newIndex;
                        return true;
                    }
                }

                return false;
            }

            public TNode Current
            {
                get { return _selection.UnderlyingList[_index]; }
            }

            public void Reset()
            {
                _index = -1;
            }

            public override bool Equals(object obj)
            {
                throw new NotSupportedException();
            }

            public override int GetHashCode()
            {
                throw new NotSupportedException();
            }
        }

        private class EnumeratorImpl : IEnumerator<TNode>
        {
            private Enumerator _en;

            internal EnumeratorImpl(SyntaxListSelection<TNode> selection)
            {
                _en = new Enumerator(selection);
            }

            public bool MoveNext()
            {
                return _en.MoveNext();
            }

            public TNode Current
            {
                get { return _en.Current; }
            }

            object IEnumerator.Current
            {
                get { return _en.Current; }
            }

            void IEnumerator.Reset()
            {
                _en.Reset();
            }

            void IDisposable.Dispose()
            {
            }
        }
    }
}
