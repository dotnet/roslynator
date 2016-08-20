// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis
{
    public static class IEnumerableExtensions
    {
        public static SyntaxList<TNode> ToSyntaxList<TNode>(this IEnumerable<TNode> nodes) where TNode : SyntaxNode
        {
            return List(nodes);
        }

        public static SeparatedSyntaxList<TNode> ToSeparatedSyntaxList<TNode>(this IEnumerable<TNode> nodes) where TNode : SyntaxNode
        {
            return SeparatedList(nodes);
        }

        public static bool ContainsEndOfLine(this IEnumerable<SyntaxTrivia> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (SyntaxTrivia trivia in collection)
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.MultiLineCommentTrivia:
                        {
                            if (trivia.ToString().Contains("\n"))
                                return true;

                            break;
                        }
                    case SyntaxKind.EndOfLineTrivia:
                        {
                            return true;
                        }
                }
            }

            return false;
        }
    }
}
