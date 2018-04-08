// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatEachEnumMemberOnSeparateLineRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            CancellationToken cancellationToken)
        {
            var rewriter = new SyntaxRewriter(enumDeclaration);

            SyntaxNode newNode = rewriter.Visit(enumDeclaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken);
        }

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly SyntaxToken[] _separators;

            public SyntaxRewriter(EnumDeclarationSyntax enumDeclaration)
            {
                _separators = enumDeclaration.Members.GetSeparators().ToArray();
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                if (_separators.Contains(token)
                    && !token.TrailingTrivia.Contains(SyntaxKind.EndOfLineTrivia))
                {
                    return token.TrimTrailingTrivia().AppendToTrailingTrivia(CSharpFactory.NewLine());
                }

                return base.VisitToken(token);
            }
        }
    }
}
