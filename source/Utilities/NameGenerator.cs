// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            return EnsureUniqueName(baseName, containingType.GetMembers());
        }

        public static string GenerateUniqueLocalName(
            string baseName,
            int position,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
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

        public static IEnumerable<ILocalSymbol> GetLocalSymbols(
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
            HashSet<string> reservedNames = new HashSet<string>(OrdinalComparer);

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
            int suffix = InitialNameSuffix;

            string name = baseName;

            while (symbols.Any(symbol => string.Equals(symbol.Name, name, stringComparison)))
            {
                name = baseName + suffix.ToString();
                suffix++;
            }

            return name;
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
