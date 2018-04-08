// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SelectedSwitchSectionsRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            bool fRemoveStatements = context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveStatementsFromSwitchSections);
            bool fAddBraces = context.IsRefactoringEnabled(RefactoringIdentifiers.AddBracesToSwitchSections);
            bool fRemoveBraces = context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveBracesFromSwitchSections);

            if (!fRemoveStatements && !fAddBraces && !fRemoveBraces)
                return;

            if (!SyntaxListSelection<SwitchSectionSyntax>.TryCreate(switchStatement.Sections, context.Span, out SyntaxListSelection<SwitchSectionSyntax> selectedSections))
                return;

            if (fAddBraces || fRemoveBraces)
            {
                var addBraces = new List<SwitchSectionSyntax>();
                var removeBraces = new List<SwitchSectionSyntax>();

                foreach (SwitchSectionSyntax section in selectedSections)
                {
                    if (addBraces.Count > 0
                        && removeBraces.Count > 0)
                    {
                        break;
                    }

                    BracesAnalysis analysis = BracesAnalysis.AnalyzeBraces(section);

                    if (analysis.AddBraces)
                    {
                        addBraces.Add(section);
                    }
                    else if (analysis.RemoveBraces)
                    {
                        removeBraces.Add(section);
                    }
                }

                if (fAddBraces && addBraces.Count > 0)
                {
                    string title = AddBracesToSwitchSectionRefactoring.Title;

                    if (addBraces.Count > 1)
                        title += "s";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken =>
                        {
                            return AddBracesToSwitchSectionsRefactoring.RefactorAsync(
                                context.Document,
                                switchStatement,
                                addBraces.ToArray(),
                                cancellationToken);
                        });
                }

                if (fRemoveBraces
                    && removeBraces.Count > 0)
                {
                    string title = RemoveBracesFromSwitchSectionRefactoring.Title;

                    if (removeBraces.Count > 1)
                        title += "s";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken =>
                        {
                            return RemoveBracesFromSwitchSectionsRefactoring.RefactorAsync(
                                context.Document,
                                switchStatement,
                                removeBraces.ToArray(),
                                cancellationToken);
                        });
                }
            }

            if (fRemoveStatements)
            {
                string title = "Remove statements from section";

                if (selectedSections.Count > 1)
                    title += "s";

                context.RegisterRefactoring(
                    title,
                    cancellationToken =>
                    {
                        return RemoveStatementsFromSwitchSectionsRefactoring.RefactorAsync(
                            context.Document,
                            switchStatement,
                            selectedSections.ToImmutableArray(),
                            cancellationToken);
                    });
            }
        }
    }
}