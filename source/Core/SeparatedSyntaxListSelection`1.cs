// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public class SeparatedSyntaxListSelection<TNode> : Selection<TNode> where TNode : SyntaxNode
    {
        protected SeparatedSyntaxListSelection(SeparatedSyntaxList<TNode> list, TextSpan span, int startIndex, int endIndex)
            : base(list, span, startIndex, endIndex)
        {
        }

        protected static (int startIndex, int endIndex) GetIndexes(SeparatedSyntaxList<TNode> list, TextSpan span)
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
                    && span.Start <= en.Current.Span.Start)
                {
                    int j = i;

                    while (span.End > GetEndIndex(list, en.Current, j)
                        && en.MoveNext())
                    {
                        j++;
                    }

                    if (span.End >= en.Current.Span.End
                        && span.End <= GetEndIndex(list, en.Current, j))
                    {
                        return (i, j);
                    }
                }
            }

            return (-1, -1);
        }

        private static int GetEndIndex(SeparatedSyntaxList<TNode> list, TNode node, int i)
        {
            return (i == list.Count - 1) ? node.FullSpan.End : list.GetSeparator(i).FullSpan.End;
        }

        public static bool TryCreate(SeparatedSyntaxList<TNode> list, TextSpan span, out SeparatedSyntaxListSelection<TNode> selection)
        {
            selection = null;

            if (!list.Any())
                return false;

            if (span.IsEmpty)
                return false;

            (int startIndex, int endIndex) = GetIndexes(list, span);

            if (startIndex == -1)
                return false;

            selection = new SeparatedSyntaxListSelection<TNode>(list, span, startIndex, endIndex);
            return true;
        }
    }
}
