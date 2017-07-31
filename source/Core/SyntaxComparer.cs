// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator
{
    internal static class SyntaxComparer
    {
        public static bool AreEquivalent(
            SyntaxNode node1,
            SyntaxNode node2,
            bool disregardTrivia = true,
            bool topLevel = false,
            bool requireNotNull = false)
        {
            if (disregardTrivia)
            {
                if (requireNotNull)
                {
                    if (node1 == null || node2 == null)
                    {
                        return false;
                    }
                }

                return SyntaxFactory.AreEquivalent(node1, node2, topLevel: topLevel);
            }
            else
            {
                if (requireNotNull)
                {
                    if (node1 == null || node2 == null)
                    {
                        return false;
                    }
                }

                if (node1 == null)
                {
                    return node2 == null;
                }

                return node1.IsEquivalentTo(node2, topLevel: topLevel);
            }
        }
    }
}
