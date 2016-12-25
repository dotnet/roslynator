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
        public static async Task<Solution> RenameAsync(
            Document document,
            ISymbol symbol,
            string newName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return await RenameAsync(document.Project.Solution, symbol, newName, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Solution> RenameAsync(
            Solution solution,
            ISymbol symbol,
            string newName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (newName == null)
                throw new ArgumentNullException(nameof(newName));

            return await Renamer.RenameSymbolAsync(
                solution,
                symbol,
                newName,
                solution.Workspace.Options,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
