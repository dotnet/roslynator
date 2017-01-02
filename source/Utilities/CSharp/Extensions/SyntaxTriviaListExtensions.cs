// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Extensions
{
    public static class SyntaxTriviaListExtensions
    {
        public static int LastIndexOf(this SyntaxTriviaList triviaList, SyntaxKind kind)
        {
            for (int i = triviaList.Count - 1; i >= 0; i--)
            {
                if (triviaList[i].IsKind(kind))
                    return i;
            }

            return -1;
        }

        public static bool Contains(this SyntaxTriviaList list, SyntaxKind kind)
        {
            return list.IndexOf(kind) != -1;
        }

        public static SyntaxTriviaList TrimStart(this SyntaxTriviaList list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    if (i > 0)
                    {
                        return TriviaList(list.Skip(i));
                    }
                    else
                    {
                        return list;
                    }
                }
            }

            return SyntaxTriviaList.Empty;
        }

        public static SyntaxTriviaList TrimEnd(this SyntaxTriviaList list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!list[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    if (i < list.Count - 1)
                    {
                        return TriviaList(list.Take(i + 1));
                    }
                    else
                    {
                        return list;
                    }
                }
            }

            return SyntaxTriviaList.Empty;
        }

        public static SyntaxTriviaList Trim(this SyntaxTriviaList list)
        {
            int startIndex = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    startIndex = i;
                    break;
                }
            }

            int endIndex = -1;
            for (int i = list.Count - 1; i > startIndex; i--)
            {
                if (!list[i].IsWhitespaceOrEndOfLineTrivia())
                {
                    endIndex = i;
                    break;
                }
            }

            if (startIndex > 0 || endIndex >= 0)
            {
                return TriviaList(list.Skip(startIndex).Take(endIndex + 1 - startIndex));
            }
            else
            {
                return list;
            }
        }
    }
}
