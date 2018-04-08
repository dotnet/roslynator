// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis.RemoveRedundantStatement
{
    internal class RemoveRedundantYieldBreakStatementAnalysis : RemoveRedundantStatementAnalysis<YieldStatementSyntax>
    {
        protected override bool IsFixable(StatementSyntax statement, BlockSyntax block, SyntaxKind parentKind)
        {
            if (!parentKind.Is(
                SyntaxKind.MethodDeclaration,
                SyntaxKind.LocalFunctionStatement))
            {
                return false;
            }

            if (object.ReferenceEquals(block.Statements.SingleOrDefault(ignoreLocalFunctions: true, shouldThrow: false), statement))
                return false;

            TextSpan span = TextSpan.FromBounds(block.SpanStart, statement.FullSpan.Start);

            return block.ContainsYield(span);
        }
    }
}
