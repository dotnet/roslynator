// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Text
{
    public class TextLineCollectionSelection : IEnumerable, IEnumerable<TextLine>
    {
        public TextLineCollectionSelection(TextLineCollection lines, TextSpan span)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            UnderlyingLines = lines;
            Span = span;

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
                            StartIndex = i;
                            EndIndex = j;
                        }
                    }
                }
            }
        }

        public TextLineCollection UnderlyingLines { get; }

        public TextSpan Span { get; }

        public int StartIndex { get; } = -1;

        public int EndIndex { get; } = -1;

        public bool Any()
        {
            return StartIndex != -1;
        }

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

        public TextLine First()
        {
            return UnderlyingLines[StartIndex];
        }

        public TextLine FirstOrDefault()
        {
            if (Any())
            {
                return UnderlyingLines[StartIndex];
            }
            else
            {
                return default(TextLine);
            }
        }

        public TextLine Last()
        {
            return UnderlyingLines[EndIndex];
        }

        public TextLine LastOrDefault()
        {
            if (Any())
            {
                return UnderlyingLines[EndIndex];
            }
            else
            {
                return default(TextLine);
            }
        }

        public ImmutableArray<TextLine> Lines
        {
            get
            {
                if (Any())
                {
                    ImmutableArray<TextLine>.Builder builder = ImmutableArray.CreateBuilder<TextLine>(Count);

                    for (int i = StartIndex; i <= EndIndex; i++)
                        builder.Add(UnderlyingLines[i]);

                    builder.ToImmutable();
                }

                return ImmutableArray<TextLine>.Empty;
            }
        }

        private IEnumerable<TextLine> EnumerateLines()
        {
            if (Any())
            {
                for (int i = StartIndex; i <= EndIndex; i++)
                    yield return UnderlyingLines[i];
            }
        }

        public IEnumerator<TextLine> GetEnumerator()
        {
            return EnumerateLines().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
