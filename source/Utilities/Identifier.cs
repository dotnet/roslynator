// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.Extensions;
using Roslynator.Internal;

namespace Roslynator
{
    public static class Identifier
    {
        private static StringComparer OrdinalComparer { get; } = StringComparer.Ordinal;

        public static string EnsureUniqueMemberName(
            string baseName,
            int position,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol containingType = semanticModel.GetEnclosingNamedType(position, cancellationToken);

            return EnsureUniqueMemberName(baseName, containingType, cancellationToken);
        }

        public static string EnsureUniqueMemberName(
            string baseName,
            INamedTypeSymbol containingType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (containingType == null)
                throw new ArgumentNullException(nameof(containingType));

            return EnsureUniqueName(baseName, containingType.GetMembers());
        }

        public static string EnsureUniqueLocalName(
            string baseName,
            int position,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            ImmutableArray<ISymbol> symbols = semanticModel.LookupSymbols(position);

            symbols = AddLocalSymbols(symbols, position, semanticModel, cancellationToken);

            return EnsureUniqueName(baseName, symbols);
        }

        private static ImmutableArray<ISymbol> AddLocalSymbols(ImmutableArray<ISymbol> symbols, int position, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ISymbol enclosingSymbol = semanticModel.GetEnclosingSymbol(position, cancellationToken);

            if (enclosingSymbol != null)
            {
                List<ISymbol> additionalSymbols = null;

                foreach (ILocalSymbol localSymbol in GetLocalSymbols(enclosingSymbol, semanticModel, cancellationToken))
                {
                    if (!symbols.Contains(localSymbol))
                    {
                        if (additionalSymbols == null)
                            additionalSymbols = new List<ISymbol>();

                        additionalSymbols.Add(localSymbol);
                    }
                }

                if (additionalSymbols != null)
                    symbols = symbols.AddRange(additionalSymbols);
            }

            return symbols;
        }

        private static IEnumerable<ILocalSymbol> GetLocalSymbols(
            ISymbol symbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxReference syntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault();

            if (syntaxReference != null)
            {
                var localSymbols = new List<ILocalSymbol>();

                SyntaxNode node = syntaxReference.GetSyntax(cancellationToken);

                foreach (SyntaxNode descendant in node.DescendantNodes())
                {
                    if (descendant.IsKind(SyntaxKind.LocalDeclarationStatement))
                    {
                        var localDeclaration = (LocalDeclarationStatementSyntax)descendant;

                        VariableDeclarationSyntax declaration = localDeclaration.Declaration;

                        if (declaration != null)
                        {
                            foreach (VariableDeclaratorSyntax variable in declaration.Variables)
                            {
                                ISymbol localSymbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);

                                if (localSymbol?.IsLocal() == true)
                                    yield return (ILocalSymbol)localSymbol;
                            }
                        }
                    }
                }
            }
        }

        public static async Task<string> EnsureUniqueParameterNameAsync(
            IParameterSymbol parameterSymbol,
            string baseName,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (parameterSymbol == null)
                throw new ArgumentNullException(nameof(parameterSymbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

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
            var reservedNames = new HashSet<string>(OrdinalComparer);

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

        public static async Task<string> EnsureUniqueAsyncMethodNameAsync(
            IMethodSymbol methodSymbol,
            string baseName,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            HashSet<string> reservedNames = await GetReservedNamesAsync(methodSymbol, solution, cancellationToken).ConfigureAwait(false);

            var generator = new AsyncMethodNameGenerator();
            return generator.EnsureUniqueName(baseName, reservedNames);
        }

        public static async Task<string> EnsureUniqueMemberNameAsync(
            ISymbol memberSymbol,
            string baseName,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (memberSymbol == null)
                throw new ArgumentNullException(nameof(memberSymbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            HashSet<string> reservedNames = await GetReservedNamesAsync(memberSymbol, solution, cancellationToken).ConfigureAwait(false);

            return EnsureUniqueName(baseName, reservedNames);
        }

        public static bool IsUniqueMemberName(
            string name,
            int position,
            SemanticModel semanticModel,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol containingType = semanticModel.GetEnclosingNamedType(position, cancellationToken);

            return IsUniqueMemberName(name, containingType, isCaseSensitive, cancellationToken);
        }

        public static bool IsUniqueMemberName(
            string name,
            INamedTypeSymbol containingType,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (containingType == null)
                throw new ArgumentNullException(nameof(containingType));

            return IsUniqueName(name, containingType.GetMembers(), GetStringComparison(isCaseSensitive));
        }

        public static async Task<bool> IsUniqueMemberNameAsync(
            ISymbol memberSymbol,
            string name,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (memberSymbol == null)
                throw new ArgumentNullException(nameof(memberSymbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

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

        private static string EnsureUniqueName(string baseName, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true)
        {
            return EnsureUniqueName(baseName, symbols, GetStringComparison(isCaseSensitive));
        }

        public static string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, bool isCaseSensitive = true)
        {
            return EnsureUniqueName(baseName, reservedNames, GetStringComparison(isCaseSensitive));
        }

        private static string EnsureUniqueName(string baseName, IList<ISymbol> symbols, StringComparison stringComparison)
        {
            int suffix = 2;

            string name = baseName;

            while (!IsUniqueName(name, symbols, stringComparison))
            {
                name = baseName + suffix.ToString();
                suffix++;
            }

            return name;
        }

        public static string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, StringComparison stringComparison)
        {
            if (reservedNames == null)
                throw new ArgumentNullException(nameof(reservedNames));

            int suffix = 2;

            string name = baseName;

            while (!IsUniqueName(name, reservedNames, stringComparison))
            {
                name = baseName + suffix.ToString();
                suffix++;
            }

            return name;
        }

        private static bool IsUniqueName(string name, IList<ISymbol> symbols, StringComparison stringComparison)
        {
            return !symbols.Any(symbol => string.Equals(symbol.Name, name, stringComparison));
        }

        private static bool IsUniqueName(string name, IEnumerable<string> reservedNames, StringComparison stringComparison)
        {
            return !reservedNames.Any(f => string.Equals(f, name, stringComparison));
        }

        public static string CreateName(ITypeSymbol typeSymbol, bool firstCharToLower = false)
        {
            return CreateNameFromTypeSymbol.CreateName(typeSymbol, firstCharToLower);
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

        public static string ToCamelCase(string value, bool prefixWithUnderscore = false)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            string prefix = (prefixWithUnderscore) ? "_" : "";

            if (value.Length > 0)
            {
                return ToCamelCase(value, prefix);
            }
            else
            {
                return prefix;
            }
        }

        private static string ToCamelCase(string value, string prefix)
        {
            var sb = new StringBuilder(prefix, value.Length + prefix.Length);

            int i = 0;

            while (i < value.Length && value[i] == '_')
                i++;

            if (char.IsUpper(value[i]))
            {
                sb.Append(char.ToLower(value[i]));
            }
            else
            {
                sb.Append(value[i]);
            }

            i++;

            sb.Append(value, i, value.Length - i);

            return sb.ToString();
        }

        public static bool IsCamelCasePrefixedWithUnderscore(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value[0] == '_')
            {
                if (value.Length > 1)
                {
                    return value[1] != '_'
                        && !char.IsUpper(value[1]);
                }

                return true;
            }

            return false;
        }

        public static bool IsCamelCaseNotPrefixedWithUnderscore(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Length > 0
                && value[0] != '_'
                && char.IsLower(value[0]);
        }

        public static bool HasPrefix(string value, string prefix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));

            return prefix.Length > 0
                && value.Length > prefix.Length
                && value.StartsWith(prefix, comparison)
                && IsBoundary(value[prefix.Length - 1], value[prefix.Length]);
        }

        public static bool HasSuffix(string value, string suffix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (suffix == null)
                throw new ArgumentNullException(nameof(suffix));

            return suffix.Length > 0
                && value.Length > suffix.Length
                && value.EndsWith(suffix, comparison)
                && IsBoundary(value[value.Length - suffix.Length - 1], value[value.Length - suffix.Length]);
        }

        private static bool IsBoundary(char ch1, char ch2)
        {
            if (IsHyphenOrUnderscore(ch1))
            {
                return !IsHyphenOrUnderscore(ch2);
            }
            else if (char.IsDigit(ch1))
            {
                return IsHyphenOrUnderscore(ch2);
            }
            else if (char.IsLower(ch1))
            {
                return IsHyphenOrUnderscore(ch2) || char.IsUpper(ch2);
            }
            else
            {
                return IsHyphenOrUnderscore(ch2);
            }
        }

        private static bool IsHyphenOrUnderscore(char ch)
        {
            return ch == '-' || ch == '_';
        }

        public static string RemovePrefix(string value, string prefix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (HasPrefix(value, prefix, comparison))
            {
                return value.Substring(prefix.Length);
            }
            else
            {
                return value;
            }
        }

        public static string RemoveSuffix(string value, string suffix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (HasSuffix(value, suffix, comparison))
            {
                return value.Remove(value.Length - suffix.Length);
            }
            else
            {
                return value;
            }
        }
    }
}
