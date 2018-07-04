// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SortCaseLabelsRefactoring
    {
        private const string Title = "Sort labels";

        public static void ComputeRefactoring(
            RefactoringContext context,
            SyntaxListSelection<SwitchLabelSyntax> selectedLabels)
        {
            SyntaxList<SwitchLabelSyntax> labels = selectedLabels.UnderlyingList;

            int firstIndex = selectedLabels.FirstIndex;

            if (!(labels[firstIndex] is CaseSwitchLabelSyntax label))
                return;

            ExpressionSyntax value = label.Value;

            SyntaxKind kind = value.Kind();

            if (kind == SyntaxKind.StringLiteralExpression)
            {
                string valueText = ((LiteralExpressionSyntax)value).Token.ValueText;

                for (int i = firstIndex + 1; i <= selectedLabels.LastIndex; i++)
                {
                    if (!(labels[i] is CaseSwitchLabelSyntax label2))
                        return;

                    if (!label2.Value.IsKind(SyntaxKind.StringLiteralExpression))
                        return;

                    string valueText2 = ((LiteralExpressionSyntax)label2.Value).Token.ValueText;

                    if (StringComparer.CurrentCulture.Compare(valueText, valueText2) > 0)
                    {
                        context.RegisterRefactoring(
                            Title,
                            ct => RefactorAsync(context.Document, selectedLabels, StringLiteralExpressionLabelComparer.Instance, ct),
                            RefactoringIdentifiers.SortCaseLabels);

                        return;
                    }

                    valueText = valueText2;
                }
            }
            else if (kind == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccess = (MemberAccessExpressionSyntax)value;

                string containingName = (memberAccess.Expression as IdentifierNameSyntax)?.Identifier.ValueText;

                if (containingName == null)
                    return;

                string name = (memberAccess.Name as IdentifierNameSyntax)?.Identifier.ValueText;

                if (name == null)
                    return;

                for (int i = firstIndex + 1; i <= selectedLabels.LastIndex; i++)
                {
                    if (!(labels[i] is CaseSwitchLabelSyntax label2))
                        return;

                    if (!label2.Value.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                        return;

                    var memberAccess2 = (MemberAccessExpressionSyntax)label2.Value;

                    if (!StringComparer.CurrentCulture.Equals(containingName, (memberAccess2.Expression as IdentifierNameSyntax)?.Identifier.ValueText))
                        return;

                    string name2 = (memberAccess2.Name as IdentifierNameSyntax)?.Identifier.ValueText;

                    if (StringComparer.CurrentCulture.Compare(name, name2) > 0)
                    {
                        context.RegisterRefactoring(
                            Title,
                            ct => RefactorAsync(context.Document, selectedLabels, SimpleMemberAccessExpressionLabelComparer.Instance, ct),
                            RefactoringIdentifiers.SortCaseLabels);

                        return;
                    }

                    name = name2;
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            SyntaxListSelection<SwitchLabelSyntax> selectedLabels,
            IComparer<SwitchLabelSyntax> comparer,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxList<SwitchLabelSyntax> labels = selectedLabels.UnderlyingList;

            SyntaxList<SwitchLabelSyntax> newLabels = labels.ReplaceRange(
                selectedLabels.FirstIndex,
                selectedLabels.Count,
                selectedLabels.OrderBy(f => f, comparer));

            var section = (SwitchSectionSyntax)labels.First().Parent;

            SwitchSectionSyntax newSection = section.WithLabels(newLabels);

            return document.ReplaceNodeAsync(section, newSection, cancellationToken);
        }

        private sealed class StringLiteralExpressionLabelComparer : IComparer<SwitchLabelSyntax>
        {
            public static StringLiteralExpressionLabelComparer Instance { get; } = new StringLiteralExpressionLabelComparer();

            public int Compare(SwitchLabelSyntax x, SwitchLabelSyntax y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                var label1 = (CaseSwitchLabelSyntax)x;
                var label2 = ((CaseSwitchLabelSyntax)y);

                string value1 = ((LiteralExpressionSyntax)label1.Value).Token.ValueText;
                string value2 = ((LiteralExpressionSyntax)label2.Value).Token.ValueText;

                return StringComparer.CurrentCulture.Compare(value1, value2);
            }
        }

        private sealed class SimpleMemberAccessExpressionLabelComparer : IComparer<SwitchLabelSyntax>
        {
            public static SimpleMemberAccessExpressionLabelComparer Instance { get; } = new SimpleMemberAccessExpressionLabelComparer();

            public int Compare(SwitchLabelSyntax x, SwitchLabelSyntax y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                var label1 = (CaseSwitchLabelSyntax)x;
                var label2 = ((CaseSwitchLabelSyntax)y);

                SimpleNameSyntax name1 = ((MemberAccessExpressionSyntax)label1.Value).Name;
                SimpleNameSyntax name2 = ((MemberAccessExpressionSyntax)label2.Value).Name;

                return StringComparer.CurrentCulture.Compare(name1.Identifier.ValueText, name2.Identifier.ValueText);
            }
        }
    }
}