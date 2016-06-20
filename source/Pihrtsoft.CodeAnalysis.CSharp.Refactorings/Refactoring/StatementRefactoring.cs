// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class StatementRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, StatementSyntax statement)
        {
            AddBracesToEmbeddedStatementRefactoring.Refactor(context, statement);
            RemoveBracesFromStatementRefactoring.Refactor(context, statement);
            ExtractStatementRefactoring.Refactor(context, statement);
        }
    }
}
