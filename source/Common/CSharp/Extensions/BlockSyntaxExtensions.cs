// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class BlockSyntaxExtensions
    {
        public static BlockSyntax WithoutStatements(this BlockSyntax block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            return block.WithStatements(List<StatementSyntax>());
        }
    }
}
