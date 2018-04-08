// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.Analysis.ReduceIfNesting
{
    internal readonly struct ReduceIfNestingAnalysisResult
    {
        public ReduceIfNestingAnalysisResult(SyntaxKind jumpKind, SyntaxNode topNode)
        {
            Debug.Assert(jumpKind == SyntaxKind.None
                || jumpKind == SyntaxKind.ReturnStatement
                || jumpKind == SyntaxKind.NullLiteralExpression
                || jumpKind == SyntaxKind.FalseLiteralExpression
                || jumpKind == SyntaxKind.TrueLiteralExpression
                || jumpKind == SyntaxKind.BreakStatement
                || jumpKind == SyntaxKind.ContinueStatement
                || jumpKind == SyntaxKind.ThrowStatement
                || jumpKind == SyntaxKind.YieldBreakStatement, jumpKind.ToString());

            JumpKind = jumpKind;
            TopNode = topNode;
        }

        public SyntaxKind JumpKind { get; }

        public SyntaxNode TopNode { get; }

        public bool Success
        {
            get { return JumpKind != SyntaxKind.None; }
        }
    }
}
