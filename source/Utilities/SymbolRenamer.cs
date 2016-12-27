// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Rename;

namespace Roslynator
{
    public static class SymbolRenamer
    {
        public static async Task<Solution> RenameSymbolAsync(
            Document document,
            ISymbol symbol,
            string newName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (newName == null)
                throw new ArgumentNullException(nameof(newName));

            Solution solution = document.Project.Solution;

            return await Renamer.RenameSymbolAsync(
                solution,
                symbol,
                newName,
                solution.Workspace.Options,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
