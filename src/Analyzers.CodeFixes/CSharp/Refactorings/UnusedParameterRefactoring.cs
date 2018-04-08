// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UnusedParameterRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            TypeParameterSyntax typeParameter,
            CancellationToken cancellationToken)
        {
            SyntaxNode node = typeParameter;

            var typeParameterList = (TypeParameterListSyntax)typeParameter.Parent;

            if (typeParameterList.Parameters.Count == 1)
                node = typeParameterList;

            SyntaxRemoveOptions options = SyntaxRemover.DefaultRemoveOptions;

            if (node.GetLeadingTrivia().All(f => f.IsWhitespaceTrivia()))
                options &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (node.GetTrailingTrivia().All(f => f.IsWhitespaceTrivia()))
                options &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return document.RemoveNodeAsync(node, options, cancellationToken);
        }
    }
}
