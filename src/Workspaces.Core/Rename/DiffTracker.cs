// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Rename
{
    internal class DiffTracker
    {
        private readonly Dictionary<DocumentId, List<DiffSpan>> _dic = new();

        public int Count => _dic.Count;

        public bool TryGetValue(DocumentId documentId, out List<DiffSpan> spans)
        {
            return _dic.TryGetValue(documentId, out spans);
        }

        internal static TextSpan GetCurrentSpan(TextSpan span, DocumentId documentId, DiffTracker diffTracker)
        {
            return diffTracker?.GetCurrentSpan(span, documentId) ?? span;
        }

        public TextSpan GetCurrentSpan(TextSpan span, DocumentId documentId)
        {
            if (!TryGetValue(documentId, out List<DiffSpan> spans))
                return span;

            int start = span.Start;
            int end = span.End;

            int i = 0;
            while (i < spans.Count)
            {
                if (spans[i].Start < start)
                {
                    start += spans[i].Diff;
                    end += spans[i].Diff;
                    i++;
                }
                else
                {
                    while (i < spans.Count
                        && spans[i].Start < end)
                    {
                        end += spans[i].Diff;
                        i++;
                    }

                    break;
                }
            }

            return (start != span.Start || end != span.End)
                ? TextSpan.FromBounds(start, end)
                : span;
        }

        public bool SpanExists(TextSpan span, DocumentId documentId)
        {
            if (TryGetValue(documentId, out List<DiffSpan> spans))
            {
                int index = spans.BinarySearch(new DiffSpan(span, 0), DiffSpanComparer.Instance);

                if (index >= 0
                    && index < spans.Count)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddLocations(
            IEnumerable<Location> locations,
            int diff,
            Solution solution)
        {
            foreach (IGrouping<DocumentId, Location> grouping in locations.GroupBy(f => solution.GetDocument(f.SourceTree).Id))
            {
                DocumentId documentId = grouping.Key;

                if (!TryGetValue(documentId, out List<DiffSpan> spans))
                {
                    spans = new List<DiffSpan>();
                    _dic.Add(documentId, spans);
                }

                int offset = 0;

                foreach (TextSpan span in grouping
                    .Select(f => f.SourceSpan)
                    .OrderBy(f => f.Start))
                {
                    AddSpan(new TextSpan(span.Start + offset, span.Length), diff, documentId);
                    offset += diff;
                }
            }
        }

        public void AddLocation(Location location, int diff, DocumentId documentId)
        {
            Debug.Assert(location.IsInSource, location.ToString());

            AddSpan(location.SourceSpan, diff, documentId);
        }

        public void AddSpan(TextSpan span, int diff, DocumentId documentId)
        {
            if (!TryGetValue(documentId, out List<DiffSpan> spans))
            {
                spans = new List<DiffSpan>() { new DiffSpan(span, diff) };
                _dic.Add(documentId, spans);
            }
            else
            {
                AddSpan(span, diff, spans);
            }
        }

        private void AddSpan(TextSpan span, int diff, List<DiffSpan> spans)
        {
            Debug.Assert(span.Length > 0, span.Length.ToString());

            int position = span.Start;

            int i = 0;
            while (i < spans.Count
                && spans[i].Start < position)
            {
                i++;
            }

            int j = i;
            var insert = true;

            if (i < spans.Count)
            {
                DiffSpan diffSpan = spans[i];

                if (diffSpan.Start == span.Start)
                {
                    Debug.Fail(span.Start.ToString());

                    spans[i] = new DiffSpan(diffSpan.Span, diffSpan.Diff + diff);
                    insert = false;
                    j++;
                }
            }

            while (j < spans.Count)
            {
                spans[j] = spans[j].Offset(diff);
                j++;
            }

            if (insert)
            {
                var diffSpan = new DiffSpan(new TextSpan(position, span.Length), diff);
                spans.Insert(i, diffSpan);
            }
#if DEBUG
            DiffSpan prev = spans[0];

            for (int k = 1; k < spans.Count; k++)
            {
                Debug.Assert(prev.Start < spans[k].Span.Start, "");
                prev = spans[k];
            }
#endif
        }

        public void Add(DiffTracker other)
        {
            foreach (KeyValuePair<DocumentId, List<DiffSpan>> kvp in other._dic)
            {
                DocumentId documentId = kvp.Key;

                if (TryGetValue(documentId, out List<DiffSpan> spans))
                {
                    foreach (DiffSpan diffSpan in kvp.Value)
                        AddSpan(diffSpan.Span, diffSpan.Diff, spans);
                }
                else
                {
                    _dic.Add(documentId, kvp.Value.ToList());
                }
            }
        }

        private class DiffSpanComparer : IComparer<DiffSpan>
        {
            public static DiffSpanComparer Instance { get; } = new();

            public int Compare(DiffSpan x, DiffSpan y) => x.Span.CompareTo(y.Span);
        }
    }
}
