// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RemoveAllRegionsRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            CompilationUnitSyntax newRoot = RegionRemover.RemoveFrom((CompilationUnitSyntax)oldRoot);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
