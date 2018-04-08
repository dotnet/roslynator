// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ElseClauseRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ElseClauseSyntax elseClause)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveConditionFromLastElse)
                && context.Span.IsEmptyAndContainedInSpan(elseClause.ElseKeyword))
            {
                RemoveConditionFromLastElseRefactoring.ComputeRefactorings(context, elseClause);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddBraces)
                && elseClause.Statement?.Kind() == SyntaxKind.IfStatement)
            {
                var ifStatement = (IfStatementSyntax)elseClause.Statement;

                if (ifStatement.IfKeyword.Span.Contains(context.Span)
                    || context.Span.IsBetweenSpans(ifStatement))
                {
                    AddBracesRefactoring.RegisterRefactoring(context, ifStatement);
                }
            }
        }
    }
}