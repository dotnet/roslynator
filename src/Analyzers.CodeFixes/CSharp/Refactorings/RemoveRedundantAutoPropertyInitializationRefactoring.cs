// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantAutoPropertyInitializationRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            TextSpan span = TextSpan.FromBounds((propertyDeclaration.Initializer).FullSpan.Start, propertyDeclaration.FullSpan.End);

            PropertyDeclarationSyntax newNode = propertyDeclaration
                .WithInitializer(null)
                .WithSemicolonToken(default(SyntaxToken))
                .WithTrailingTrivia(propertyDeclaration.DescendantTrivia(span))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
        }
    }
}