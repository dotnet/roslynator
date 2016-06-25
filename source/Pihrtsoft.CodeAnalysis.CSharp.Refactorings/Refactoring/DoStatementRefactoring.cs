// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class DoStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, DoStatementSyntax doStatement)
        {
            if (doStatement.Condition != null
                && doStatement.Condition.Span.Contains(context.Span)
                && context.SupportsSemanticModel)
            {
                await AddBooleanComparisonRefactoring.ComputeRefactoringAsync(context, doStatement.Condition);
            }
        }
    }
}
