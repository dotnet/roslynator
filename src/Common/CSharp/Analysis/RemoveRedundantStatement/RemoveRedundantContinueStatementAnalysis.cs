// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.RemoveRedundantStatement
{
    internal sealed class RemoveRedundantContinueStatementAnalysis : RemoveRedundantStatementAnalysis<ContinueStatementSyntax>
    {
        public static RemoveRedundantContinueStatementAnalysis Instance { get; } = new RemoveRedundantContinueStatementAnalysis();

        private RemoveRedundantContinueStatementAnalysis()
        {
        }

        protected override bool IsFixable(StatementSyntax statement, BlockSyntax block, SyntaxKind parentKind)
        {
            return CSharpFacts.IsIterationStatement(parentKind);
        }
    }
}
