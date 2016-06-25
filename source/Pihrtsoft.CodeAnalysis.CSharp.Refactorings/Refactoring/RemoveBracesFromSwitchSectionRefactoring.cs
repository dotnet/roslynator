// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RemoveBracesFromSwitchSectionRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, SwitchSectionSyntax switchSection)
        {
            if (switchSection.Statements.Count == 1
                && switchSection.Statements[0].IsKind(SyntaxKind.Block))
            {
#if DEBUG
                var block = (BlockSyntax)switchSection.Statements[0];

                return block.OpenBraceToken.Span.Contains(context.Span)
                    || block.CloseBraceToken.Span.Contains(context.Span);
#else
                return true;
#endif
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var block = (BlockSyntax)switchSection.Statements[0];

            SwitchSectionSyntax newSwitchSection = switchSection
                .WithStatements(block.Statements)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(switchSection, newSwitchSection);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
