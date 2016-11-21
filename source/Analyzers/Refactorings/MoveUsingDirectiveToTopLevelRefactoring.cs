// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MoveUsingDirectiveToTopLevelRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            NamespaceDeclarationSyntax namespaceDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var compilationUnit = (CompilationUnitSyntax)root;

            SyntaxList<UsingDirectiveSyntax> usings = namespaceDeclaration.Usings;

            CompilationUnitSyntax newCompilationUnit = compilationUnit
                .RemoveNodes(usings, SyntaxRemoveOptions.KeepUnbalancedDirectives)
                .AddUsings(
                    keepSingleLineCommentsOnTop: true,
                    usings: usings.Select(f => f.WithFormatterAnnotation()).ToArray());

            return document.WithSyntaxRoot(newCompilationUnit);
        }
    }
}
