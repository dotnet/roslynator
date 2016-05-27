// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class YieldStatementSyntaxExtensions
    {
        public static bool IsYieldReturn(this YieldStatementSyntax yieldStatement)
        {
            if (yieldStatement == null)
                throw new ArgumentNullException(nameof(yieldStatement));

            return yieldStatement.ReturnOrBreakKeyword.IsKind(SyntaxKind.ReturnKeyword);
        }

        public static bool IsYieldBreak(this YieldStatementSyntax yieldStatement)
        {
            if (yieldStatement == null)
                throw new ArgumentNullException(nameof(yieldStatement));

            return yieldStatement.ReturnOrBreakKeyword.IsKind(SyntaxKind.BreakKeyword);
        }
    }
}
