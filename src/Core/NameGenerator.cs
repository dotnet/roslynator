// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Roslynator.Helpers;

namespace Roslynator
{
    /// <summary>
    /// Provides methods to obtain an unique identifier.
    /// </summary>
    public abstract class NameGenerator
    {
        internal static StringComparer OrdinalComparer { get; } = StringComparer.Ordinal;

        internal static StringComparer OrdinalIgnoreCaseComparer { get; } = StringComparer.OrdinalIgnoreCase;

        /// <summary>
        /// Default implementation of <see cref="NameGenerator"/> that adds number suffix to ensure uniqueness.
        /// </summary>
        public static NameGenerator Default
        {
            get { return NameGenerators.NumberSuffix; }
        }

        /// <summary>
        /// Returns an unique name using the specified list of reserved names.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="reservedNames"></param>
        /// <param name="isCaseSensitive"></param>
        /// <returns></returns>
        public abstract string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, bool isCaseSensitive = true);

        /// <summary>
        /// Returns an unique name using the specified list of symbols.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="symbols"></param>
        /// <param name="isCaseSensitive"></param>
        /// <returns></returns>
        public abstract string EnsureUniqueName(string baseName, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true);

        /// <summary>
        /// Returns a member name that will be unique at the specified position.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="semanticModel"></param>
        /// <param name="position"></param>
        /// <param name="isCaseSensitive"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
            INamedTypeSymbol typeSymbol,
            bool isCaseSensitive = true)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return EnsureUniqueName(baseName, typeSymbol.GetMembers(), isCaseSensitive);
        }

        /// <summary>
        /// Return a local name that will be unique at the specified position.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="semanticModel"></param>
        /// <param name="position"></param>
        /// <param name="isCaseSensitive"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

            if (containingSymbol.Kind == SymbolKind.Method)
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

            Debug.Assert(containingType != null);

            if (containingType == null)
                return true;

            return IsUniqueName(name, containingType.GetMembers(), isCaseSensitive);
        }

        /// <summary>
        /// Returns true if the name is not contained in the specified list. <see cref="ISymbol.Name"/> is used to compare names.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="symbols"></param>
        /// <param name="isCaseSensitive"></param>
        /// <returns></returns>
        public static bool IsUniqueName(string name, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true)
        {
            StringComparison comparison = GetStringComparison(isCaseSensitive);

            for (int i = 0; i < symbols.Length; i++)
            {
                if (string.Equals(name, symbols[i].Name, comparison))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if the name is not contained in the specified list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reservedNames"></param>
        /// <param name="isCaseSensitive"></param>
        /// <returns></returns>
        public static bool IsUniqueName(string name, IEnumerable<string> reservedNames, bool isCaseSensitive = true)
        {
            StringComparison comparison = GetStringComparison(isCaseSensitive);

            foreach (string reservedName in reservedNames)
            {
                if (string.Equals(name, reservedName, comparison))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a syntax identifier from the specified type symbol.
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="firstCharToLower"></param>
        /// <returns></returns>
        public static string CreateName(ITypeSymbol typeSymbol, bool firstCharToLower = false)
        {
            string name = CreateNameFromTypeSymbolHelper.CreateName(typeSymbol);

            if (name != null
                && firstCharToLower)
            {
                return StringUtility.FirstCharToLower(name);
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

            bool AreDigits(string value, int start, int count)
            {
                int max = start + count;

                for (int i = start; i < max; i++)
                {
                    if (!char.IsDigit(value, i))
                        return false;
                }

                return true;
            }
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
    }
}
