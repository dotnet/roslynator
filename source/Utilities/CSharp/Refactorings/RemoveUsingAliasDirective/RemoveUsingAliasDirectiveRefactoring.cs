// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.RemoveUsingAliasDirective
{
    public static class RemoveUsingAliasDirectiveRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            UsingDirectiveSyntax usingDirective,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RemoveUsingAliasDirectiveSyntaxRewriter.VisitAsync(document, usingDirective, cancellationToken).ConfigureAwait(false);
        }
    }
}
