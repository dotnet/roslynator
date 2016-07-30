// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SortSwitchSectionsRefactoring
    {
        private static readonly SwitchLabelSorter _switchLabelSorter = new SwitchLabelSorter();
        private static readonly SwitchSectionSorter _switchSectionSorter = new SwitchSectionSorter();

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SwitchStatementSyntax newSwitchStatement = SortSections(switchStatement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(switchStatement, newSwitchStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SwitchStatementSyntax SortSections(SwitchStatementSyntax switchStatement)
        {
            if (switchStatement == null)
                throw new ArgumentNullException(nameof(switchStatement));

            return switchStatement.WithSections(
                List(switchStatement.Sections
                    .Select(f => SortLabels(f))
                    .OrderBy(f => f, _switchSectionSorter)));
        }

        private static SwitchSectionSyntax SortLabels(SwitchSectionSyntax switchSection)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            return SwitchSection(
                List(switchSection.Labels.OrderBy(f => f, _switchLabelSorter)),
                switchSection.Statements);
        }

        private class SwitchSectionSorter : IComparer<SwitchSectionSyntax>
        {
            public int Compare(SwitchSectionSyntax x, SwitchSectionSyntax y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                if (x.Labels.Count == 0)
                {
                    if (y.Labels.Count == 0)
                        return 0;
                    else
                        return -1;
                }

                if (y.Labels.Count == 0)
                    return 1;

                if (x.Labels.Any(SyntaxKind.DefaultSwitchLabel))
                    return 1;

                if (y.Labels.Any(SyntaxKind.DefaultSwitchLabel))
                    return -1;

                int result = x.Labels.Count.CompareTo(y.Labels.Count);

                if (result != 0)
                    return result;

                return string.Compare(
                    ((CaseSwitchLabelSyntax)x.Labels[0]).Value?.ToString(),
                    ((CaseSwitchLabelSyntax)y.Labels[0]).Value?.ToString(),
                    StringComparison.CurrentCulture);
            }
        }

        private class SwitchLabelSorter : IComparer<SwitchLabelSyntax>
        {
            public int Compare(SwitchLabelSyntax x, SwitchLabelSyntax y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                if (x.IsKind(SyntaxKind.DefaultSwitchLabel))
                    return 1;

                if (y.IsKind(SyntaxKind.DefaultSwitchLabel))
                    return -1;

                return string.Compare(
                    ((CaseSwitchLabelSyntax)x).Value?.ToString(),
                    ((CaseSwitchLabelSyntax)y).Value?.ToString(),
                    StringComparison.CurrentCulture);
            }
        }
    }
}
