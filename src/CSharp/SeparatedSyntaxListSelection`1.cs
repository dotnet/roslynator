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
    /// Represents selected nodes in a <see cref="SeparatedSyntaxList{TNode}"/>.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SeparatedSyntaxListSelection<TNode> : Selection<TNode> where TNode : SyntaxNode
    {
        internal int Length;

        private SeparatedSyntaxListSelection(SeparatedSyntaxList<TNode> list, TextSpan span, SelectionResult result)
            : this(list, span, result.FirstIndex, result.LastIndex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeparatedSyntaxListSelection{TNode}"/>.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="span"></param>
        /// <param name="firstIndex"></param>
        /// <param name="lastIndex"></param>
        protected SeparatedSyntaxListSelection(SeparatedSyntaxList<TNode> list, TextSpan span, int firstIndex, int lastIndex)
            : base(span, firstIndex, lastIndex)
        {
            UnderlyingList = list;
        }

        /// <summary>
        /// Gets an underlying list that contains selected nodes.
        /// </summary>
        public SeparatedSyntaxList<TNode> UnderlyingList { get; }

        /// <summary>
        /// Gets an underlying list that contains selected nodes.
        /// </summary>
        protected override IReadOnlyList<TNode> Items => UnderlyingList;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"Count = {Count} FirstIndex = {FirstIndex} LastIndex = {LastIndex} {UnderlyingList.ToString(TextSpan.FromBounds(First().SpanStart, Last().Span.End))}"; }
        }

        /// <summary>
        /// Creates a new <see cref="SeparatedSyntaxListSelection{TNode}"/> based on the specified list and span.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static SeparatedSyntaxListSelection<TNode> Create(SeparatedSyntaxList<TNode> list, TextSpan span)
        {
            SelectionResult result = SelectionResult.Create(list, span);

            if (!result.Success)
                throw new InvalidOperationException("No selected node found.");

            return new SeparatedSyntaxListSelection<TNode>(list, span, result.FirstIndex, result.LastIndex);
        }

        /// <summary>
        /// Creates a new <see cref="SeparatedSyntaxListSelection{TNode}"/> based on the specified list and span.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="span"></param>
        /// <param name="selection"></param>
        /// <returns>True if the specified span contains at least one node; otherwise, false.</returns>
        public static bool TryCreate(SeparatedSyntaxList<TNode> list, TextSpan span, out SeparatedSyntaxListSelection<TNode> selection)
        {
            selection = Create(list, span, 1, int.MaxValue);
            return selection != null;
        }

        internal static bool TryCreate(SeparatedSyntaxList<TNode> list, TextSpan span, int minCount, out SeparatedSyntaxListSelection<TNode> selection)
        {
            selection = Create(list, span, minCount, int.MaxValue);
            return selection != null;
        }

        internal static bool TryCreate(SeparatedSyntaxList<TNode> list, TextSpan span, int minCount, int maxCount, out SeparatedSyntaxListSelection<TNode> selection)
        {
            selection = Create(list, span, minCount, maxCount);
            return selection != null;
        }

        private static SeparatedSyntaxListSelection<TNode> Create(SeparatedSyntaxList<TNode> list, TextSpan span, int minCount, int maxCount)
        {
            SelectionResult result = SelectionResult.Create(list, span, minCount, maxCount);

            if (!result.Success)
                return null;

            return new SeparatedSyntaxListSelection<TNode>(list, span, result);
        }

        /// <summary>
        /// Returns an enumerator that iterates through selected items.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        protected override IEnumerator<TNode> GetEnumeratorCore()
        {
            return new EnumeratorImpl(this);
        }

        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Pending>")]
        [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
        [SuppressMessage("Usage", "CA2231:Overload operator equals on overriding value type Equals", Justification = "<Pending>")]
        public struct Enumerator
        {
            private readonly SeparatedSyntaxListSelection<TNode> _selection;
            private int _index;

            internal Enumerator(SeparatedSyntaxListSelection<TNode> selection)
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

            internal EnumeratorImpl(SeparatedSyntaxListSelection<TNode> selection)
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
