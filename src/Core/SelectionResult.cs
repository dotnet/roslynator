// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    internal readonly struct SelectionResult
    {
        public SelectionResult(int firstIndex, int lastIndex)
        {
            FirstIndex = firstIndex;
            LastIndex = lastIndex;
            Success = true;
        }

        public bool IsInRange(int minCount, int maxCount)
        {
            Debug.Assert(Success);

            int count = LastIndex - FirstIndex + 1;

            return count >= minCount
                && count <= maxCount;
        }

        public static SelectionResult Fail { get; } = new SelectionResult();

        public int FirstIndex { get; }

        public int LastIndex { get; }

        public bool Success { get; }

        public static SelectionResult Create<TNode>(SyntaxList<TNode> nodes, TextSpan span, int minCount, int maxCount) where TNode : SyntaxNode
        {
            ThrowIfNotValidRange(minCount, maxCount);

            if (!nodes.Any())
                return Fail;

            if (span.IsEmpty)
                return Fail;

            SelectionResult result = Create(nodes, span);

            if (!result.Success)
                return Fail;

            if (!result.IsInRange(minCount, maxCount))
                return Fail;

            return result;
        }

        public static SelectionResult Create<TNode>(SyntaxList<TNode> list, TextSpan span) where TNode : SyntaxNode
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
                    && span.Start <= en.Current.SpanStart)
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
                        return new SelectionResult(i, j);
                    }
                }
            }

            return Fail;
        }

        public static SelectionResult Create<TNode>(SeparatedSyntaxList<TNode> nodes, TextSpan span, int minCount, int maxCount) where TNode : SyntaxNode
        {
            ThrowIfNotValidRange(minCount, maxCount);

            if (!nodes.Any())
                return Fail;

            if (span.IsEmpty)
                return Fail;

            SelectionResult result = Create(nodes, span);

            if (!result.Success)
                return Fail;

            if (!result.IsInRange(minCount, maxCount))
                return Fail;

            return result;
        }

        public static SelectionResult Create<TNode>(SeparatedSyntaxList<TNode> list, TextSpan span) where TNode : SyntaxNode
        {
            SeparatedSyntaxList<TNode>.Enumerator en = list.GetEnumerator();

            if (en.MoveNext())
            {
                int i = 0;

                while (span.Start >= en.Current.FullSpan.End
                    && en.MoveNext())
                {
                    i++;
                }

                if (span.Start >= en.Current.FullSpan.Start
                    && span.Start <= en.Current.SpanStart)
                {
                    int j = i;

                    while (span.End > GetLastIndex(en.Current, j)
                        && en.MoveNext())
                    {
                        j++;
                    }

                    if (span.End >= en.Current.Span.End
                        && span.End <= GetLastIndex(en.Current, j))
                    {
                        return new SelectionResult(i, j);
                    }
                }
            }

            return Fail;

            int GetLastIndex(TNode node, int i)
            {
                if (i < list.Count - 1
                    || list.Count == list.SeparatorCount)
                {
                    return list.GetSeparator(i).FullSpan.End;
                }

                return node.FullSpan.End;
            }
        }

        public static SelectionResult Create(TextLineCollection lines, TextSpan span, int minCount, int maxCount)
        {
            ThrowIfNotValidRange(minCount, maxCount);

            if (lines.Count == 0)
                return Fail;

            if (span.IsEmpty)
                return Fail;

            SelectionResult result = Create(lines, span);

            if (!result.Success)
                return Fail;

            if (!result.IsInRange(minCount, maxCount))
                return Fail;

            return result;
        }

        public static SelectionResult Create(TextLineCollection lines, TextSpan span)
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
                            return new SelectionResult(i, j);
                        }
                    }
                }
            }

            return Fail;
        }

        private static void ThrowIfNotValidRange(int minCount, int maxCount)
        {
            if (minCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(minCount), minCount, $"{nameof(minCount)} must be greater than 0.");

            if (maxCount < minCount)
                throw new ArgumentOutOfRangeException(nameof(maxCount), maxCount, $"{nameof(maxCount)} must be greater than or equal to {nameof(minCount)}.");
        }
    }
}
