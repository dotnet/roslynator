// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwitchStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateSwitchSections)
                && await GenerateSwitchSectionsRefactoring.CanRefactorAsync(context, switchStatement).ConfigureAwait(false))
            {
                context.RegisterRefactoring(
                    "Generate sections",
                    cancellationToken =>
                    {
                        return GenerateSwitchSectionsRefactoring.RefactorAsync(
                            context.Document,
                            switchStatement,
                            cancellationToken);
                    });
            }

            SelectedSwitchSectionsRefactoring.ComputeRefactorings(context, switchStatement);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceSwitchWithIfElse)
                && (switchStatement.SwitchKeyword.Span.Contains(context.Span) || context.Span.IsBetweenSpans(switchStatement))
                && switchStatement
                    .Sections
                    .Any(section => !section.Labels.Contains(SyntaxKind.DefaultSwitchLabel)))
            {
                context.RegisterRefactoring(
                    "Replace switch with if-else",
                    cancellationToken => ReplaceSwitchWithIfElseRefactoring.RefactorAsync(context.Document, switchStatement, cancellationToken));
            }
        }
    }
}