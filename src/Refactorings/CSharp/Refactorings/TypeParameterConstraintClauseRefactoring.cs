// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class TypeParameterConstraintClauseRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, TypeParameterConstraintClauseSyntax constraintClause)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapConstraintClauses)
                && (context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(constraintClause)))
            {
                WrapConstraintClausesRefactoring.ComputeRefactoring(context, constraintClause);
            }
        }
    }
}
