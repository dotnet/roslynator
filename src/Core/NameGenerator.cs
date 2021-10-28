// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    /// <summary>
    /// Provides methods to obtain an unique identifier.
    /// </summary>
    [SuppressMessage("Usage", "RCS1223:Mark publicly visible type with DebuggerDisplay attribute.")]
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
        public abstract string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, bool isCaseSensitive = true);

        /// <summary>
        /// Returns an unique name using the specified list of symbols.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="symbols"></param>
        /// <param name="isCaseSensitive"></param>
        public abstract string EnsureUniqueName(string baseName, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true);

        /// <summary>
        /// Returns a name that will be unique at the specified position.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="semanticModel"></param>
        /// <param name="position"></param>
        /// <param name="isCaseSensitive"></param>
        public string EnsureUniqueName(
            string baseName,
            SemanticModel semanticModel,
            int position,
            bool isCaseSensitive = true)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return EnsureUniqueName(baseName, semanticModel.LookupSymbols(position), isCaseSensitive);
        }

        /// <summary>
        /// Returns unique enum member name for a specified enum type.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="enumType"></param>
        /// <param name="isCaseSensitive"></param>
        public string EnsureUniqueEnumMemberName(
            string baseName,
            INamedTypeSymbol enumType,
            bool isCaseSensitive = true)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));

            if (enumType.TypeKind != TypeKind.Enum)
                throw new ArgumentException("Symbol must be an enumeration.", nameof(enumType));

            return EnsureUniqueName(baseName, enumType.GetMembers(), isCaseSensitive);
        }

        /// <summary>
        /// Return a local name that will be unique at the specified position.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="semanticModel"></param>
        /// <param name="position"></param>
        /// <param name="isCaseSensitive"></param>
        /// <param name="cancellationToken"></param>
        public string EnsureUniqueLocalName(
            string baseName,
            SemanticModel semanticModel,
            int position,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            ImmutableArray<ISymbol> symbols = GetSymbolsForUniqueLocalName(semanticModel, position, cancellationToken);

            return EnsureUniqueName(baseName, symbols, isCaseSensitive);
        }

        /// <summary>
        /// Return a local names that will be unique at the specified position.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="semanticModel"></param>
        /// <param name="position"></param>
        /// <param name="count"></param>
        /// <param name="isCaseSensitive"></param>
        /// <param name="cancellationToken"></param>
        public ImmutableArray<string> EnsureUniqueLocalNames(
            string baseName,
            SemanticModel semanticModel,
            int position,
            int count,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (count < 1)
                throw new ArgumentOutOfRangeException(nameof(count), count, "");

            if (count == 1)
            {
                string name = EnsureUniqueLocalName(baseName, semanticModel, position, isCaseSensitive, cancellationToken);

                return ImmutableArray.Create(name);
            }

            List<string> reservedNames = GetSymbolsForUniqueLocalName(semanticModel, position, cancellationToken)
                .Select(f => f.Name)
                .ToList();

            ImmutableArray<string>.Builder names = ImmutableArray.CreateBuilder<string>(count);

            for (int i = 0; i < count; i++)
            {
                string name = EnsureUniqueName(baseName, reservedNames, isCaseSensitive);

                names.Add(name);

                reservedNames.Add(name);
            }

            return names.ToImmutable();
        }

        private static ImmutableArray<ISymbol> GetSymbolsForUniqueLocalName(
            SemanticModel semanticModel,
            int position,
            CancellationToken cancellationToken)
        {
            return semanticModel
                .GetSymbolsDeclaredInEnclosingSymbol(position, excludeAnonymousTypeProperty: true, cancellationToken: cancellationToken)
                .AddRange(semanticModel.LookupSymbols(position));
        }

        /// <summary>
        /// Return a parameter name that will be unique at the specified position.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="containingSymbol"></param>
        /// <param name="semanticModel"></param>
        /// <param name="isCaseSensitive"></param>
        /// <param name="cancellationToken"></param>
        public string EnsureUniqueParameterName(
            string baseName,
            ISymbol containingSymbol,
            SemanticModel semanticModel,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default)
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

        /// <summary>
        /// Returns true if the name is not contained in the specified list. <see cref="ISymbol.Name"/> is used to compare names.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="symbols"></param>
        /// <param name="isCaseSensitive"></param>
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
            CancellationToken cancellationToken = default)
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
            CancellationToken cancellationToken = default)
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
            CancellationToken cancellationToken = default)
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

            static bool AreDigits(string value, int start, int count)
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
