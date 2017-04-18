// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SplitSwitchLabelsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, SwitchSectionSyntax switchSection)
        {
            if (switchSection.IsParentKind(SyntaxKind.SwitchStatement))
            {
                SyntaxList<SwitchLabelSyntax> labels = switchSection.Labels;

                if (labels.Count > 1)
                {
                    SyntaxListSelection<SwitchLabelSyntax> selection;
                    if (SyntaxListSelection<SwitchLabelSyntax>.TryCreate(labels, context.Span, out selection))
                    {
                        if (selection.Count > 1 || (selection.First() != labels.Last()))
                        {
                            SwitchLabelSyntax[] selectedLabels = selection.ToArray();

                            if (selectedLabels.Last() == labels.Last())
                                selectedLabels = selectedLabels.Take(selectedLabels.Length - 1).ToArray();

                            context.RegisterRefactoring(
                                "Split labels",
                                cancellationToken => RefactorAsync(context.Document, switchSection, selectedLabels, cancellationToken));
                        }
                    }
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            SwitchLabelSyntax[] selectedLabels,
            CancellationToken cancellationToken)
        {
            SyntaxList<SwitchLabelSyntax> labels = switchSection.Labels;

            int lastIndex = labels.IndexOf(selectedLabels.Last());

            var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

            SwitchStatementSyntax newSwitchStatement = switchStatement
                .RemoveNodes(labels.Take(lastIndex + 1), SyntaxRemoveOptions.KeepNoTrivia);

            SyntaxList<SwitchSectionSyntax> newSections = newSwitchStatement
                .Sections
                .InsertRange(0, CreateSwitchSections(switchSection, selectedLabels));

            newSwitchStatement = newSwitchStatement.WithSections(newSections);

            return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken);
        }

        private static IEnumerable<SwitchSectionSyntax> CreateSwitchSections(SwitchSectionSyntax switchSection, SwitchLabelSyntax[] selectedLabels)
        {
            SyntaxList<SwitchLabelSyntax> labels = switchSection.Labels;

            int firstIndex = labels.IndexOf(selectedLabels[0]);

            if (firstIndex > 0)
            {
                yield return SwitchSection(labels.Take(firstIndex + 1).ToSyntaxList(), SingletonList<StatementSyntax>(BreakStatement()))
                    .WithFormatterAnnotation();

                foreach (SwitchLabelSyntax label in selectedLabels.Skip(1))
                    yield return CreateSection(label);
            }
            else
            {
                foreach (SwitchLabelSyntax label in selectedLabels)
                    yield return CreateSection(label);
            }
        }

        private static SwitchSectionSyntax CreateSection(SwitchLabelSyntax label)
        {
            return SwitchSection(label, BreakStatement()).WithFormatterAnnotation();
        }
    }
}