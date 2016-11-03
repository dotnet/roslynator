// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public class SelectedLinesInfo
    {
        private bool _findExecuted;
        private int _firstIndex = -1;
        private int _lastIndex = -1;

        public SelectedLinesInfo(TextLineCollection lines, TextSpan span)
        {
            Span = span;
            Lines = lines;
        }

        public TextSpan Span { get; }
        public TextLineCollection Lines { get; }

        public bool IsAnySelected
        {
            get { return FirstSelectedLineIndex != -1; }
        }

        public bool IsSingleSelected
        {
            get
            {
                int firstIndex = FirstSelectedLineIndex;

                return firstIndex != -1
                    && LastSelectedLineIndex == firstIndex;
            }
        }

        public bool AreManySelected
        {
            get
            {
                int firstIndex = FirstSelectedLineIndex;

                return firstIndex != -1
                    && LastSelectedLineIndex > firstIndex;
            }
        }

        public int SelectedCount
        {
            get
            {
                int firstIndex = FirstSelectedLineIndex;

                if (firstIndex != -1)
                    return LastSelectedLineIndex - firstIndex + 1;

                return 0;
            }
        }

        public int FirstSelectedLineIndex
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

        public int LastSelectedLineIndex
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

        public TextLine FirstSelectedLine
        {
            get
            {
                int index = FirstSelectedLineIndex;

                if (index != -1)
                    return Lines[index];

                return default(TextLine);
            }
        }

        public TextLine LastSelectedLine
        {
            get
            {
                int index = LastSelectedLineIndex;

                if (index != -1)
                    return Lines[index];

                return default(TextLine);
            }
        }

        public IEnumerable<TextLine> SelectedLines()
        {
            int firstIndex = FirstSelectedLineIndex;

            if (firstIndex != -1)
            {
                int lastIndex = LastSelectedLineIndex;

                for (int i = firstIndex; i <= lastIndex; i++)
                    yield return Lines[i];
            }
        }

        private bool FindSelectedLines()
        {
            using (TextLineCollection.Enumerator en = Lines.GetEnumerator())
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
    }
}
