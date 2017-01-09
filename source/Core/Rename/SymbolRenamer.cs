// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Roslynator.Rename
{
    public static class Renamer
    {
        public static async Task<Solution> RenameSymbolAsync(
            Document document,
            ISymbol symbol,
            string newName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Solution solution = document.Project.Solution;

            return await Microsoft.CodeAnalysis.Rename.Renamer.RenameSymbolAsync(
                solution,
                symbol,
                newName,
                solution.Workspace.Options,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
