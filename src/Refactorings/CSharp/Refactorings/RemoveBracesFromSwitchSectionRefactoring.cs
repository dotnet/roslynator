// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveBracesFromSwitchSectionRefactoring
    {
        public const string Title = "Remove braces from section";

        public static bool CanRemoveBraces(SwitchSectionSyntax section)
        {
            return section.Statements.SingleOrDefault(shouldThrow: false)?.Kind() == SyntaxKind.Block;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var block = (BlockSyntax)switchSection.Statements[0];

            SwitchSectionSyntax newSwitchSection = switchSection
                .WithStatements(block.Statements)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(switchSection, newSwitchSection, cancellationToken);
        }
    }
}
