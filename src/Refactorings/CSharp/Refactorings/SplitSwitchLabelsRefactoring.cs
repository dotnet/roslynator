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
            if (!switchSection.IsParentKind(SyntaxKind.SwitchStatement))
                return;

            SyntaxList<SwitchLabelSyntax> labels = switchSection.Labels;

            if (labels.Count <= 1)
                return;

            if (!SyntaxListSelection<SwitchLabelSyntax>.TryCreate(labels, context.Span, out SyntaxListSelection<SwitchLabelSyntax> selectedLabels))
                return;

            if (selectedLabels.Count == 1
                && (selectedLabels.First() == labels.Last()))
            {
                return;
            }

            context.RegisterRefactoring(
                "Split labels",
                cancellationToken => RefactorAsync(context.Document, switchSection, selectedLabels, cancellationToken));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax section,
            SyntaxListSelection<SwitchLabelSyntax> selectedLabels,
            CancellationToken cancellationToken)
        {
            SyntaxList<SwitchLabelSyntax> labels = section.Labels;

            int lastIndex = selectedLabels.LastIndex;

            if (selectedLabels.Last() == labels.Last())
                lastIndex--;

            var switchStatement = (SwitchStatementSyntax)section.Parent;

            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            int index = sections.IndexOf(section);

            SyntaxList<SwitchSectionSyntax> newSections = sections
                .ReplaceAt(index, section.RemoveNodes(labels.Take(lastIndex + 1), SyntaxRemoveOptions.KeepNoTrivia))
                .InsertRange(index, CreateSwitchSections(section, selectedLabels, lastIndex));

            SwitchStatementSyntax newSwitchStatement = switchStatement.WithSections(newSections);

            return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken);
        }

        private static IEnumerable<SwitchSectionSyntax> CreateSwitchSections(
            SwitchSectionSyntax section,
            SyntaxListSelection<SwitchLabelSyntax> selectedLabels,
            int lastIndex)
        {
            int firstIndex = selectedLabels.FirstIndex;

            if (firstIndex > 0)
            {
                yield return SwitchSection(section.Labels.Take(firstIndex + 1).ToSyntaxList(), BreakStatement())
                    .WithFormatterAnnotation();

                firstIndex++;
            }

            for (int i = firstIndex; i <= lastIndex; i++)
            {
                yield return SwitchSection(selectedLabels.UnderlyingList[i], BreakStatement())
                    .WithFormatterAnnotation();
            }
        }
    }
}