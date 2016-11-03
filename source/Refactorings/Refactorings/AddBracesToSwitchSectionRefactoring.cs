// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddBracesToSwitchSectionRefactoring
    {
        public const string Title = "Add braces to section";

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SwitchSectionSyntax newNode = switchSection
                .WithStatements(
                    List<StatementSyntax>(
                        SingletonList(
                            Block(switchSection.Statements))))
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(switchSection, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
