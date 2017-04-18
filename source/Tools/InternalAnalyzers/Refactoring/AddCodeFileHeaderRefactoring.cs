// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactoring
{
    internal static class AddCodeFileHeaderRefactoring
    {
        public const string Header = "// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.";

        public static SyntaxTrivia HeaderTrivia { get; } = Comment(Header);

        public static bool CanRefactor(CompilationUnitSyntax compilationUnit)
        {
            foreach (SyntaxTrivia trivia in compilationUnit.GetLeadingTrivia())
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.WhitespaceTrivia:
                    case SyntaxKind.EndOfLineTrivia:
                        continue;
                    case SyntaxKind.SingleLineCommentTrivia:
                        return !string.Equals(trivia.ToString(), Header, StringComparison.Ordinal);
                    default:
                        return true;
                }
            }

            return true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            CompilationUnitSyntax compilationUnit,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            CompilationUnitSyntax newNode = compilationUnit
                .WithLeadingTrivia(
                    compilationUnit
                        .GetLeadingTrivia()
                        .Insert(0, CSharpFactory.NewLine())
                        .Insert(0, HeaderTrivia));

            root = root.ReplaceNode(compilationUnit, newNode);

            return document.WithSyntaxRoot(root);
        }
    }
}
