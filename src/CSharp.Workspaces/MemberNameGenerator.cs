// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Roslynator
{
    internal static class MemberNameGenerator
    {
        public static async Task<string> EnsureUniqueMemberNameAsync(
            string baseName,
            ISymbol memberSymbol,
            Solution solution,
            NameGenerator nameGenerator,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (memberSymbol == null)
                throw new ArgumentNullException(nameof(memberSymbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            if (nameGenerator == null)
                throw new ArgumentNullException(nameof(nameGenerator));

            HashSet<string> reservedNames = await GetReservedNamesAsync(memberSymbol, solution, isCaseSensitive, cancellationToken).ConfigureAwait(false);

            return nameGenerator.EnsureUniqueName(baseName, reservedNames);
        }

        public static async Task<bool> IsUniqueMemberNameAsync(
            string name,
            ISymbol memberSymbol,
            Solution solution,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (memberSymbol == null)
                throw new ArgumentNullException(nameof(memberSymbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            HashSet<string> reservedNames = await GetReservedNamesAsync(memberSymbol, solution, isCaseSensitive, cancellationToken).ConfigureAwait(false);

            return !reservedNames.Contains(name);
        }

        private static async Task<HashSet<string>> GetReservedNamesAsync(
            ISymbol memberSymbol,
            Solution solution,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HashSet<string> reservedNames = GetMemberNames(memberSymbol, isCaseSensitive);

            foreach (ReferencedSymbol referencedSymbol in await SymbolFinder.FindReferencesAsync(memberSymbol, solution, cancellationToken).ConfigureAwait(false))
            {
                foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                {
                    if (!referenceLocation.IsImplicit
                        && !referenceLocation.IsCandidateLocation)
                    {
                        SemanticModel semanticModel = await referenceLocation.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                        foreach (ISymbol symbol in semanticModel.LookupSymbols(referenceLocation.Location.SourceSpan.Start))
                        {
                            if (!memberSymbol.Equals(symbol))
                                reservedNames.Add(symbol.Name);
                        }
                    }
                }
            }

            return reservedNames;
        }

        private static HashSet<string> GetMemberNames(ISymbol memberSymbol, bool isCaseSensitive = true)
        {
            INamedTypeSymbol containingType = memberSymbol.ContainingType;

            Debug.Assert(containingType != null);

            if (containingType != null)
            {
                IEnumerable<string> memberNames = containingType
                    .GetMembers()
                    .Where(f => !memberSymbol.Equals(f))
                    .Select(f => f.Name);

                return CreateHashSet(memberNames, isCaseSensitive);
            }
            else
            {
                return CreateHashSet(isCaseSensitive);
            }
        }

        private static HashSet<string> CreateHashSet(IEnumerable<string> names, bool isCaseSensitive = true)
        {
            if (isCaseSensitive)
            {
                return new HashSet<string>(names, NameGenerator.OrdinalComparer);
            }
            else
            {
                return new HashSet<string>(names, NameGenerator.OrdinalIgnoreCaseComparer);
            }
        }

        private static HashSet<string> CreateHashSet(bool isCaseSensitive = true)
        {
            if (isCaseSensitive)
            {
                return new HashSet<string>(NameGenerator.OrdinalComparer);
            }
            else
            {
                return new HashSet<string>(NameGenerator.OrdinalIgnoreCaseComparer);
            }
        }
    }
}
