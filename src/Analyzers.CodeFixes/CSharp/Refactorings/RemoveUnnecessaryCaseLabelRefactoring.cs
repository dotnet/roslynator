// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveUnnecessaryCaseLabelRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            CaseSwitchLabelSyntax label,
            CancellationToken cancellationToken)
        {
            var switchSection = (SwitchSectionSyntax)label.Parent;

            SwitchSectionSyntax newNode = switchSection.RemoveNode(label, GetRemoveOptions(label))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(switchSection, newNode, cancellationToken);
        }

        private static SyntaxRemoveOptions GetRemoveOptions(CaseSwitchLabelSyntax label)
        {
            if (label.GetLeadingTrivia().IsEmptyOrWhitespace()
                && label.GetTrailingTrivia().IsEmptyOrWhitespace())
            {
                return SyntaxRemoveOptions.KeepNoTrivia;
            }

            return SyntaxRemoveOptions.KeepExteriorTrivia;
        }
    }
}
