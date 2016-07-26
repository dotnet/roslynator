// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReplaceSwitchSectionStatementsWithBlockRefactoring
    {
        public static bool CanRefactor(SwitchSectionSyntax switchSection)
        {
            return switchSection.Statements.All(f => !f.IsKind(SyntaxKind.Block));
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SwitchSectionSyntax newNode = switchSection
                .WithStatements(List<StatementSyntax>(SingletonList(Block(switchSection.Statements))))
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(switchSection, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
