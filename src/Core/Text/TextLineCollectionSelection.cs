// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Text
{
    /// <summary>
    /// Represents selected lines in a <see cref="TextLineCollection"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class TextLineCollectionSelection : ISelection<TextLine>
    {
        private TextLineCollectionSelection(TextLineCollection lines, TextSpan span, in SelectionResult result)
            : this(lines, span, result.FirstIndex, result.LastIndex)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="TextLineCollectionSelection"/>.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="span"></param>
        /// <param name="firstIndex"></param>
        /// <param name="lastIndex"></param>
        protected TextLineCollectionSelection(TextLineCollection lines, TextSpan span, int firstIndex, int lastIndex)
        {
            if (firstIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(firstIndex), firstIndex, "Index of the first selected line must be greater than or equal to zero.");

            if (lastIndex < firstIndex)
                throw new ArgumentOutOfRangeException(nameof(lastIndex), lastIndex, "Index of the last selected line must be greater or equal to index of the first selected line.");

            UnderlyingLines = lines;
            OriginalSpan = span;
            FirstIndex = firstIndex;
            LastIndex = lastIndex;
        }

        /// <summary>
        /// Gets an underlying collection that contains selected lines.
        /// </summary>
        public TextLineCollection UnderlyingLines { get; }

        /// <summary>
        /// Gets the original span that was used to determine selected lines.
        /// </summary>
        public TextSpan OriginalSpan { get; }

        /// <summary>
        /// Gets an index of the first selected line.
        /// </summary>
        public int FirstIndex { get; }

        /// <summary>
        /// Gets an index of the last selected line.
        /// </summary>
        public int LastIndex { get; }

        /// <summary>
        /// Gets a number of selected lines.
        /// </summary>
        public int Count
        {
            get { return LastIndex - FirstIndex + 1; }
        }

        /// <summary>
        /// Gets the selected line at the specified index.
        /// </summary>
        /// <returns>The line at the specified index.</returns>
        /// <param name="index">The zero-based index of the line to get. </param>
        public TextLine this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, "");
                }

                return UnderlyingLines[index + FirstIndex];
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return $"Count = {Count} FirstIndex = {FirstIndex} LastIndex = {LastIndex}"; }
        }

        /// <summary>
        /// Gets the first selected line.
        /// </summary>
        /// <returns></returns>
        public TextLine First()
        {
            return UnderlyingLines[FirstIndex];
        }

        /// <summary>
        /// Gets the last selected line.
        /// </summary>
        /// <returns></returns>
        public TextLine Last()
        {
            return UnderlyingLines[LastIndex];
        }

        /// <summary>
        /// Creates a new <see cref="TextLineCollectionSelection"/> based on the specified list and span.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static TextLineCollectionSelection Create(TextLineCollection lines, TextSpan span)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            SelectionResult result = SelectionResult.Create(lines, span);

            return new TextLineCollectionSelection(lines, span, result);
        }

        /// <summary>
        /// Creates a new <see cref="TextLineCollectionSelection"/> based on the specified list and span.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="span"></param>
        /// <param name="selectedLines"></param>
        /// <returns>True if the specified span contains at least one line; otherwise, false.</returns>
        public static bool TryCreate(TextLineCollection lines, TextSpan span, out TextLineCollectionSelection selectedLines)
        {
            selectedLines = Create(lines, span, 1, int.MaxValue);
            return selectedLines != null;
        }

        internal static bool TryCreate(TextLineCollection lines, TextSpan span, int minCount, out TextLineCollectionSelection selectedLines)
        {
            selectedLines = Create(lines, span, minCount, int.MaxValue);
            return selectedLines != null;
        }

        internal static bool TryCreate(TextLineCollection lines, TextSpan span, int minCount, int maxCount, out TextLineCollectionSelection selectedLines)
        {
            selectedLines = Create(lines, span, minCount, maxCount);
            return selectedLines != null;
        }

        private static TextLineCollectionSelection Create(TextLineCollection lines, TextSpan span, int minCount, int maxCount)
        {
            if (lines == null)
                return null;

            SelectionResult result = SelectionResult.Create(lines, span, minCount, maxCount);

            if (!result.Success)
                return null;

            return new TextLineCollectionSelection(lines, span, result);
        }

        /// <summary>
        /// Returns an enumerator that iterates through selected lines.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<TextLine> IEnumerable<TextLine>.GetEnumerator()
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
        [SuppressMessage("Usage", "RCS1223:Mark publicly visible type with DebuggerDisplay attribute.", Justification = "<Pending>")]
        public struct Enumerator
        {
            private readonly TextLineCollectionSelection _selection;
            private int _index;

            internal Enumerator(TextLineCollectionSelection selection)
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

            public TextLine Current
            {
                get { return _selection.UnderlyingLines[_index]; }
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

        private class EnumeratorImpl : IEnumerator<TextLine>
        {
            private Enumerator _en;

            internal EnumeratorImpl(TextLineCollectionSelection selection)
            {
                _en = new Enumerator(selection);
            }

            public bool MoveNext()
            {
                return _en.MoveNext();
            }

            public TextLine Current
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
