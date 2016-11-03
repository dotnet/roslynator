// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal struct TokenPair
    {
        public TokenPair(BlockSyntax block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            OpenToken = block.OpenBraceToken;
            CloseToken = block.CloseBraceToken;
        }

        public TokenPair(AccessorListSyntax accessorList)
        {
            if (accessorList == null)
                throw new ArgumentNullException(nameof(accessorList));

            OpenToken = accessorList.OpenBraceToken;
            CloseToken = accessorList.CloseBraceToken;
        }

        public TokenPair(SyntaxToken openToken, SyntaxToken closeToken)
        {
            OpenToken = openToken;
            CloseToken = closeToken;
        }

        public SyntaxToken OpenToken { get; }
        public SyntaxToken CloseToken { get; }
    }
}
