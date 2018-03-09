// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.RemoveRedundantStatement
{
    internal class RemoveRedundantContinueStatementRefactoring : RemoveRedundantStatementRefactoring<ContinueStatementSyntax>
    {
        protected override bool IsFixable(StatementSyntax statement, BlockSyntax block, SyntaxKind parentKind)
        {
            return parentKind.CanContainContinueStatement();
        }
    }
}
