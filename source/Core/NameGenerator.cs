// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Roslynator.Helpers;
using Roslynator.Utilities;

namespace Roslynator
{
    public abstract class NameGenerator
    {
        internal static StringComparer OrdinalComparer { get; } = StringComparer.Ordinal;
        internal static StringComparer OrdinalIgnoreCaseComparer { get; } = StringComparer.OrdinalIgnoreCase;

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
    }
}
