// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public class SyntaxListSelection<TNode> : Selection<TNode> where TNode : SyntaxNode
    {
        protected SyntaxListSelection(SyntaxList<TNode> list, TextSpan span, int startIndex, int endIndex)
            : base(list, span, startIndex, endIndex)
        {
        }

        protected static (int startIndex, int endIndex) GetIndexes(SyntaxList<TNode> list, TextSpan span)
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
                    && span.Start <= en.Current.Span.Start)
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
                        return (i, j);
                    }
                }
            }

            return (-1, -1);
        }

        public static bool TryCreate(SyntaxList<TNode> list, TextSpan span, out SyntaxListSelection<TNode> selection)
        {
            selection = null;

            if (!list.Any())
                return false;

            if (span.IsEmpty)
                return false;

            (int startIndex, int endIndex) = GetIndexes(list, span);

            if (startIndex == -1)
                return false;

            selection = new SyntaxListSelection<TNode>(list, span, startIndex, endIndex);
            return true;
        }
    }
}
