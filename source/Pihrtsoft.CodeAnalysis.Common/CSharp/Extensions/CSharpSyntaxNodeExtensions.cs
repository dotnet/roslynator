// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class CSharpSyntaxNodeExtensions
    {
        public static SyntaxTriviaList GetLeadingAndTrailingTrivia(this CSharpSyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node
                .GetLeadingTrivia()
                .AddRange(node.GetTrailingTrivia());
        }
    }
}
