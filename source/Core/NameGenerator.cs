// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.Helpers;
using Roslynator.Utilities;

namespace Roslynator
{
    public abstract class NameGenerator
    {
        private static StringComparer OrdinalComparer { get; } = StringComparer.Ordinal;
        private static StringComparer OrdinalIgnoreCaseComparer { get; } = StringComparer.OrdinalIgnoreCase;

        public abstract string EnsureUniqueName(string baseName, HashSet<string> reservedNames);
        public abstract string EnsureUniqueName(string baseName, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true);

        public static NameGenerator Default
        {
            get { return NameGenerators.NumberSuffix; }
        }

        public string EnsureUniqueMemberName(
            string baseName,
            SemanticModel semanticModel,
            int position,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol containingType = semanticModel.GetEnclosingNamedType(position, cancellationToken);

            if (containingType != null)
            {
                return EnsureUniqueMemberName(baseName, containingType, isCaseSensitive);
            }
            else
            {
                return EnsureUniqueName(baseName, semanticModel.LookupSymbols(position), isCaseSensitive);
            }
        }

        public string EnsureUniqueMemberName(
            string baseName,
            INamedTypeSymbol containingType,
            bool isCaseSensitive = true)
        {
            if (containingType == null)
                throw new ArgumentNullException(nameof(containingType));

            return EnsureUniqueName(baseName, containingType.GetMembers(), isCaseSensitive);
        }

        public string EnsureUniqueLocalName(
            string baseName,
            SemanticModel semanticModel,
            int position,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            ImmutableArray<ISymbol> symbols = semanticModel
                .GetSymbolsDeclaredInEnclosingSymbol(position, excludeAnonymousTypeProperty: true, cancellationToken: cancellationToken)
                .AddRange(semanticModel.LookupSymbols(position));

            return EnsureUniqueName(baseName, symbols, isCaseSensitive);
        }

        internal string EnsureUniqueParameterName(
            string baseName,
            ISymbol containingSymbol,
            SemanticModel semanticModel,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (containingSymbol == null)
                throw new ArgumentNullException(nameof(containingSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (containingSymbol.IsMethod())
            {
                var methodSymbol = (IMethodSymbol)containingSymbol;

                containingSymbol = methodSymbol.PartialImplementationPart ?? methodSymbol;
            }

            SyntaxNode containingNode = containingSymbol.GetSyntax(cancellationToken);

            ImmutableArray<ISymbol> symbols = semanticModel
                .GetDeclaredSymbols(containingNode, excludeAnonymousTypeProperty: true, cancellationToken: cancellationToken)
                .AddRange(semanticModel.LookupSymbols(containingNode.SpanStart));

            return EnsureUniqueName(baseName, symbols, isCaseSensitive);
        }

        internal virtual async Task<string> EnsureUniqueMemberNameAsync(
            string baseName,
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

            return EnsureUniqueName(baseName, reservedNames);
        }

        public string EnsureUniqueEnumMemberName(
            string baseName,
            INamedTypeSymbol enumSymbol,
            bool isCaseSensitive = true)
        {
            if (enumSymbol == null)
                throw new ArgumentNullException(nameof(enumSymbol));

            return EnsureUniqueName(baseName, enumSymbol.GetMembers(), isCaseSensitive);
        }

        internal static bool IsUniqueMemberName(
            string name,
            SemanticModel semanticModel,
            int position,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol containingType = semanticModel.GetEnclosingNamedType(position, cancellationToken);

            return IsUniqueMemberName(name, containingType, isCaseSensitive);
        }

        public static bool IsUniqueMemberName(
            string name,
            INamedTypeSymbol containingType,
            bool isCaseSensitive = true)
        {
            if (containingType == null)
                throw new ArgumentNullException(nameof(containingType));

            return IsUniqueName(name, containingType.GetMembers(), isCaseSensitive);
        }

        internal static async Task<bool> IsUniqueMemberNameAsync(
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

            return IsUniqueName(name, reservedNames);
        }

        internal static async Task<HashSet<string>> GetReservedNamesAsync(
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

        internal static bool IsUniqueName(string name, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true)
        {
            StringComparison comparison = GetStringComparison(isCaseSensitive);

            for (int i = 0; i < symbols.Length; i++)
            {
                if (string.Equals(name, symbols[i].Name, comparison))
                    return false;
            }

            return true;
        }

        internal static bool IsUniqueName(string name, HashSet<string> reservedNames)
        {
            return !reservedNames.Contains(name);
        }

        public static string CreateName(ITypeSymbol typeSymbol, bool firstCharToLower = false)
        {
            string name = CreateNameFromTypeSymbolHelper.CreateName(typeSymbol);

            if (name != null
                && firstCharToLower)
            {
                name = StringUtility.FirstCharToLower(name);
            }

            return name;
        }

        internal string CreateUniqueLocalName(
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            int position,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (typeSymbol != null)
            {
                string name = CreateName(typeSymbol, firstCharToLower: true);

                if (name != null)
                    return EnsureUniqueLocalName(name, semanticModel, position, isCaseSensitive, cancellationToken);
            }

            return null;
        }

        internal string CreateUniqueLocalName(
            ITypeSymbol typeSymbol,
            string oldName,
            SemanticModel semanticModel,
            int position,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string newName = CreateName(typeSymbol, firstCharToLower: true);

            if (newName != null
                && !string.Equals(oldName, newName, StringComparison.Ordinal))
            {
                string uniqueName = EnsureUniqueLocalName(newName, semanticModel, position, isCaseSensitive, cancellationToken);

                if (!IsChangeOnlyInSuffix(oldName, newName, uniqueName))
                    return uniqueName;
            }

            return null;
        }

        internal string CreateUniqueParameterName(
            string oldName,
            IParameterSymbol parameterSymbol,
            SemanticModel semanticModel,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string newName = CreateName(parameterSymbol.Type, firstCharToLower: true);

            if (newName != null
                && !string.Equals(oldName, newName, StringComparison.Ordinal))
            {
                string uniqueName = EnsureUniqueParameterName(newName, parameterSymbol.ContainingSymbol, semanticModel, isCaseSensitive, cancellationToken);

                if (!IsChangeOnlyInSuffix(oldName, newName, uniqueName))
                    return uniqueName;
            }

            return null;
        }

        private static bool IsChangeOnlyInSuffix(string oldName, string newName, string uniqueName, bool isCaseSensitive = true)
        {
            return oldName.Length > newName.Length
                && string.Compare(oldName, 0, newName, 0, newName.Length, GetStringComparison(isCaseSensitive)) == 0
                && AreDigits(oldName, newName.Length, oldName.Length - newName.Length)
                && uniqueName.Length > newName.Length
                && AreDigits(uniqueName, newName.Length, uniqueName.Length - newName.Length);
        }

        private static bool AreDigits(string value, int start, int count)
        {
            int max = start + count;

            for (int i = start; i < max; i++)
            {
                if (!char.IsDigit(value, i))
                    return false;
            }

            return true;
        }

        private static StringComparison GetStringComparison(bool isCaseSensitive)
        {
            if (isCaseSensitive)
            {
                return StringComparison.Ordinal;
            }
            else
            {
                return StringComparison.OrdinalIgnoreCase;
            }
        }

        private static HashSet<string> CreateHashSet(IEnumerable<string> names, bool isCaseSensitive = true)
        {
            if (isCaseSensitive)
            {
                return new HashSet<string>(names, OrdinalComparer);
            }
            else
            {
                return new HashSet<string>(names, OrdinalIgnoreCaseComparer);
            }
        }

        private static HashSet<string> CreateHashSet(bool isCaseSensitive = true)
        {
            if (isCaseSensitive)
            {
                return new HashSet<string>(OrdinalComparer);
            }
            else
            {
                return new HashSet<string>(OrdinalIgnoreCaseComparer);
            }
        }
    }
}
