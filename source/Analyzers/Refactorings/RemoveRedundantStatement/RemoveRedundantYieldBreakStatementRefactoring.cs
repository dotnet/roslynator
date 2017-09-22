// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.RemoveRedundantStatement
{
    internal class RemoveRedundantYieldBreakStatementRefactoring : RemoveRedundantStatementRefactoring<YieldStatementSyntax>
    {
        protected override bool IsFixable(StatementSyntax statement, BlockSyntax block, SyntaxKind parentKind)
        {
            if (parentKind != SyntaxKind.MethodDeclaration
                && parentKind != SyntaxKind.LocalFunctionStatement)
            {
                return false;
            }

            if (!base.IsFixable(statement, block, parentKind))
                return false;

            TextSpan span = TextSpan.FromBounds(block.SpanStart, statement.FullSpan.Start);

            return block
                .DescendantNodes(span, f => !f.IsNestedMethod())
                .Any(f => f.IsKind(SyntaxKind.YieldBreakStatement, SyntaxKind.YieldReturnStatement));
        }
    }
}
