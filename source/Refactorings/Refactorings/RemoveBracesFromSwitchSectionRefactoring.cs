// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class RemoveBracesFromSwitchSectionRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, SwitchSectionSyntax switchSection)
        {
            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            return statements.Count == 1
                && statements[0].IsKind(SyntaxKind.Block);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var block = (BlockSyntax)switchSection.Statements[0];

            SwitchSectionSyntax newSwitchSection = switchSection
                .WithStatements(block.Statements)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(switchSection, newSwitchSection);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
