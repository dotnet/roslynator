// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantBaseInterfaceRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            BaseTypeSyntax baseType,
            CancellationToken cancellationToken)
        {
            SyntaxRemoveOptions removeOptions = SyntaxRemover.DefaultRemoveOptions;

            if (baseType.GetLeadingTrivia().All(f => f.IsWhitespaceTrivia()))
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (baseType.GetTrailingTrivia().All(f => f.IsWhitespaceTrivia()))
            {
                var baseList = (BaseListSyntax)baseType.Parent;

                if (baseList.Types.IsLast(baseType)
                    && !SyntaxInfo.GenericInfo(baseList.Parent).ConstraintClauses.Any())
                {
                    removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;
                }
            }

            return document.RemoveNodeAsync(baseType, removeOptions, cancellationToken);
        }
    }
}
