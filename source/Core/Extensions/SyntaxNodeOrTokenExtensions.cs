// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Extensions
{
    public static class SyntaxNodeOrTokenExtensions
    {
        public static SyntaxNodeOrToken WithoutLeadingTrivia(this SyntaxNodeOrToken nodeOrToken)
        {
            if (nodeOrToken.IsNode)
            {
                return nodeOrToken.AsNode().WithoutLeadingTrivia();
            }
            else
            {
                return nodeOrToken.AsToken().WithoutLeadingTrivia();
            }
        }

        public static SyntaxNodeOrToken WithoutTrailingTrivia(this SyntaxNodeOrToken nodeOrToken)
        {
            if (nodeOrToken.IsNode)
            {
                return nodeOrToken.AsNode().WithoutTrailingTrivia();
            }
            else
            {
                return nodeOrToken.AsToken().WithoutTrailingTrivia();
            }
        }
    }
}
