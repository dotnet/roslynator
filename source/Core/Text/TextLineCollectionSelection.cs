// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Text
{
    public class TextLineCollectionSelection : Selection<TextLine>
    {
        public TextLineCollectionSelection(TextLineCollection lines, TextSpan span, int startIndex, int endIndex)
            : base(lines, span, startIndex, endIndex)
        {
        }

        private static (int startIndex, int endIndex) GetIndexes(TextLineCollection lines, TextSpan span)
        {
            using (TextLineCollection.Enumerator en = lines.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    int i = 0;

                    while (span.Start >= en.Current.EndIncludingLineBreak
                        && en.MoveNext())
                    {
                        i++;
                    }

                    if (span.Start == en.Current.Start)
                    {
                        int j = i;

                        while (span.End > en.Current.EndIncludingLineBreak
                            && en.MoveNext())
                        {
                            j++;
                        }

                        if (span.End == en.Current.End
                            || span.End == en.Current.EndIncludingLineBreak)
                        {
                            return (i, j);
                        }
                    }
                }
            }

            return (-1, -1);
        }

        public static TextLineCollectionSelection Create(TextLineCollection lines, TextSpan span)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            (int startIndex, int endIndex) = GetIndexes(lines, span);

            return new TextLineCollectionSelection(lines, span, startIndex, endIndex);
        }

        public static bool TryCreate(TextLineCollection lines, TextSpan span, out TextLineCollectionSelection selection)
        {
            selection = null;

            if (lines.Count == 0)
                return false;

            if (span.IsEmpty)
                return false;

            (int startIndex, int endIndex) = GetIndexes(lines, span);

            if (startIndex == -1)
                return false;

            selection = new TextLineCollectionSelection(lines, span, startIndex, endIndex);
            return true;
        }
    }
}
