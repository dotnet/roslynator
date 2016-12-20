// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.Internal;

namespace Roslynator
{
    public static class NameGenerator
    {
        private static StringComparer OrdinalComparer { get; } = StringComparer.Ordinal;

        private const int InitialNameSuffix = 2;

        public static string GenerateUniqueMemberName(
            string baseName,
            int position,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            INamedTypeSymbol containingType = semanticModel.GetEnclosingNamedType(position, cancellationToken);

            return GenerateUniqueMemberName(baseName, containingType, semanticModel, cancellationToken);
        }

        public static string GenerateUniqueMemberName(
            string baseName,
            INamedTypeSymbol containingType,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<string> memberNames = containingType
                .GetMembers()
                .Select(member => member.Name);

            return EnsureUniqueName(baseName, memberNames);
        }

        public static string GenerateUniqueLocalName(
            string baseName,
            int position,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<string> memberNames = semanticModel
                .LookupSymbols(position)
                .Select(symbol => symbol.Name);

            return EnsureUniqueName(baseName, memberNames);
        }

        public static async Task<string> GenerateUniqueParameterNameAsync(
            IParameterSymbol parameterSymbol,
            string baseName,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HashSet<string> reservedNames = GetParameterNames(parameterSymbol);

            foreach (ReferencedSymbol referencedSymbol in await SymbolFinder.FindReferencesAsync(parameterSymbol, solution, cancellationToken).ConfigureAwait(false))
            {
                foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                {
                    if (!referenceLocation.IsImplicit
                        && !referenceLocation.IsCandidateLocation)
                    {
                        SemanticModel semanticModel = await referenceLocation.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                        foreach (ISymbol symbol in semanticModel.LookupSymbols(referenceLocation.Location.SourceSpan.Start))
                        {
                            if (!parameterSymbol.Equals(symbol))
                                reservedNames.Add(symbol.Name);
                        }
                    }
                }
            }

            return EnsureUniqueName(baseName, reservedNames);
        }

        private static HashSet<string> GetParameterNames(IParameterSymbol parameterSymbol)
        {
            HashSet<string> reservedNames = new HashSet<string>();

            ISymbol containingSymbol = parameterSymbol.ContainingSymbol;

            Debug.Assert(containingSymbol?.IsKind(SymbolKind.Method) == true);

            if (containingSymbol?.IsKind(SymbolKind.Method) == true)
            {
                var methodSymbol = (IMethodSymbol)containingSymbol;

                foreach (IParameterSymbol symbol in methodSymbol.Parameters)
                {
                    if (!parameterSymbol.Equals(symbol))
                        reservedNames.Add(symbol.Name);
                }
            }

            return reservedNames;
        }

        public static async Task<string> GenerateUniqueAsyncMethodNameAsync(
            IMethodSymbol methodSymbol,
            string baseName,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HashSet<string> reservedNames = await GetReservedNamesAsync(methodSymbol, solution, cancellationToken).ConfigureAwait(false);

            var generator = new AsyncMethodNameGenerator();
            return generator.EnsureUniqueName(baseName, reservedNames);
        }

        public static async Task<string> GenerateUniqueMemberNameAsync(
            ISymbol memberSymbol,
            string baseName,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HashSet<string> reservedNames = await GetReservedNamesAsync(memberSymbol, solution, cancellationToken).ConfigureAwait(false);

            return EnsureUniqueName(baseName, reservedNames);
        }

        public static async Task<bool> IsUniqueMemberNameAsync(
            ISymbol memberSymbol,
            string name,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HashSet<string> reservedNames = await GetReservedNamesAsync(memberSymbol, solution, cancellationToken).ConfigureAwait(false);

            return !reservedNames.Contains(name);
        }

        private static async Task<HashSet<string>> GetReservedNamesAsync(
            ISymbol memberSymbol,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HashSet<string> reservedNames = GetMemberNames(memberSymbol);

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

        private static HashSet<string> GetMemberNames(ISymbol memberSymbol)
        {
            INamedTypeSymbol containingType = memberSymbol.ContainingType;

            Debug.Assert(containingType != null);

            if (containingType != null)
            {
                IEnumerable<string> memberNames = containingType
                    .GetMembers()
                    .Where(f => !memberSymbol.Equals(f))
                    .Select(f => f.Name);

                return new HashSet<string>(memberNames, OrdinalComparer);
            }
            else
            {
                return new HashSet<string>(OrdinalComparer);
            }
        }

        public static string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, bool isCaseSensitive = true)
        {
            StringComparison stringComparison = (isCaseSensitive)
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            return EnsureUniqueName(baseName, reservedNames, stringComparison);
        }

        public static string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, StringComparison stringComparison)
        {
            int suffix = InitialNameSuffix;

            string name = baseName;

            while (reservedNames.Any(f => string.Equals(f, name, stringComparison)))
            {
                name = baseName + suffix.ToString();
                suffix++;
            }

            return name;
        }

        public static string GenerateIdentifier(ITypeSymbol typeSymbol, bool firstCharToLower = false)
        {
            return IdentifierGenerator.GenerateIdentifier(typeSymbol, firstCharToLower);
        }
    }
}
