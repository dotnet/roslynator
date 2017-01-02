// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Text
{
    public class SelectedTextLineCollection : IEnumerable, IEnumerable<TextLine>
    {
        private bool _findExecuted;
        private int _firstIndex = -1;
        private int _lastIndex = -1;

        public SelectedTextLineCollection(TextLineCollection lines, TextSpan span)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            UnderlyingLines = lines;
            Span = span;
        }

        public TextLineCollection UnderlyingLines { get; }
        public TextSpan Span { get; }

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
                    return LastIndex - firstIndex + 1;

                return 0;
            }
        }

        public int FirstIndex
        {
            get
            {
                if (!_findExecuted)
                {
                    FindSelectedLines();
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
                    FindSelectedLines();
                    _findExecuted = true;
                }

                return _lastIndex;
            }
        }

        public TextLine First
        {
            get
            {
                int firstIndex = FirstIndex;

                if (firstIndex != -1)
                {
                    return UnderlyingLines[firstIndex];
                }
                else
                {
                    return default(TextLine);
                }
            }
        }

        public TextLine Last
        {
            get
            {
                int lastIndex = LastIndex;

                if (lastIndex != -1)
                {
                    return UnderlyingLines[lastIndex];
                }
                else
                {
                    return default(TextLine);
                }
            }
        }

        private IEnumerable<TextLine> SelectedLines()
        {
            int firstIndex = FirstIndex;

            if (firstIndex != -1)
            {
                int lastIndex = LastIndex;

                for (int i = firstIndex; i <= lastIndex; i++)
                    yield return UnderlyingLines[i];
            }
        }

        private bool FindSelectedLines()
        {
            using (TextLineCollection.Enumerator en = UnderlyingLines.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    int i = 0;

                    while (Span.Start >= en.Current.EndIncludingLineBreak
                        && en.MoveNext())
                    {
                        i++;
                    }

                    if (Span.Start == en.Current.Start)
                    {
                        int j = i;

                        while (Span.End > en.Current.EndIncludingLineBreak
                            && en.MoveNext())
                        {
                            j++;
                        }

                        if (Span.End == en.Current.End
                            || Span.End == en.Current.EndIncludingLineBreak)
                        {
                            _firstIndex = i;
                            _lastIndex = j;
                        }
                    }
                }
            }

            return false;
        }

        public IEnumerator<TextLine> GetEnumerator()
        {
            return SelectedLines().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
