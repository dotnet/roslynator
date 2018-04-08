// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantFieldInitializationRefactoring
    {
        internal static Task<Document> RefactorAsync(
            Document document,
            VariableDeclaratorSyntax variableDeclarator,
            CancellationToken cancellationToken)
        {
            EqualsValueClauseSyntax initializer = variableDeclarator.Initializer;

            var removeOptions = SyntaxRemoveOptions.KeepExteriorTrivia;

            if (initializer.GetLeadingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (initializer.GetTrailingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            VariableDeclaratorSyntax newNode = variableDeclarator
                .RemoveNode(initializer, removeOptions)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(variableDeclarator, newNode, cancellationToken);
        }
    }
}