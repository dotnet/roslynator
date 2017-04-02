// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Roslynator.Extensions
{
    public static class SemanticModelExtensions
    {
        public static IEnumerable<Diagnostic> GetCompilerDiagnostics(
            this SemanticModel semanticModel,
            TextSpan? span = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return semanticModel
                .GetDiagnostics(span, cancellationToken)
                .Where(f => f.IsCompilerDiagnostic());
        }

        public static bool ContainsDiagnostic(
            this SemanticModel semanticModel,
            string id,
            TextSpan? span = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDiagnostic(semanticModel, id, span, cancellationToken) != null;
        }

        public static bool ContainsCompilerDiagnostic(
            this SemanticModel semanticModel,
            string id,
            TextSpan? span = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetCompilerDiagnostic(semanticModel, id, span, cancellationToken) != null;
        }

        public static Diagnostic GetDiagnostic(
            this SemanticModel semanticModel,
            string id,
            TextSpan? span = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            ImmutableArray<Diagnostic> diagnostics = semanticModel.GetDiagnostics(span, cancellationToken);

            for (int i = 0; i < diagnostics.Length; i++)
            {
                if (string.Equals(diagnostics[i].Id, id, StringComparison.Ordinal))
                    return diagnostics[i];
            }

            return null;
        }

        public static Diagnostic GetCompilerDiagnostic(
            this SemanticModel semanticModel,
            string id,
            TextSpan? span = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            ImmutableArray<Diagnostic> diagnostics = semanticModel.GetDiagnostics(span, cancellationToken);

            for (int i = 0; i < diagnostics.Length; i++)
            {
                if (string.Equals(diagnostics[i].Id, id, StringComparison.Ordinal)
                    && diagnostics[i].IsCompilerDiagnostic())
                {
                    return diagnostics[i];
                }
            }

            return null;
        }

        public static INamedTypeSymbol GetEnclosingNamedType(
            this SemanticModel semanticModel,
            int position,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return semanticModel.GetEnclosingSymbol<INamedTypeSymbol>(position, cancellationToken);
        }

        public static TSymbol GetEnclosingSymbol<TSymbol>(
            this SemanticModel semanticModel,
            int position,
            CancellationToken cancellationToken = default(CancellationToken)) where TSymbol : ISymbol
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            ISymbol symbol = semanticModel.GetEnclosingSymbol(position, cancellationToken);

            while (symbol != null)
            {
                if (symbol is TSymbol)
                    return (TSymbol)symbol;

                symbol = symbol.ContainingSymbol;
            }

            return default(TSymbol);
        }

        public static ISymbol GetSymbol(
            this SemanticModel semanticModel,
            SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return ModelExtensions
                .GetSymbolInfo(semanticModel, node, cancellationToken)
                .Symbol;
        }

        public static ITypeSymbol GetTypeSymbol(
            this SemanticModel semanticModel,
            SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return ModelExtensions
                .GetTypeInfo(semanticModel, node, cancellationToken)
                .Type;
        }

        public static INamedTypeSymbol GetTypeByMetadataName(this SemanticModel semanticModel, string fullyQualifiedMetadataName)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return semanticModel
                .Compilation
                .GetTypeByMetadataName(fullyQualifiedMetadataName);
        }

        public static IMethodSymbol GetSpeculativeMethodSymbol(
            this SemanticModel semanticModel,
            int position,
            SyntaxNode expression)
        {
            SymbolInfo symbolInfo = ModelExtensions.GetSpeculativeSymbolInfo(
                semanticModel,
                position,
                expression,
                SpeculativeBindingOption.BindAsExpression);

            return symbolInfo.Symbol as IMethodSymbol;
        }

        internal static ImmutableArray<ISymbol> GetSymbolsDeclaredInEnclosingSymbol(
            this SemanticModel semanticModel,
            int position,
            bool excludeAnonymousTypeProperty = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode container = GetEnclosingSymbolSyntax(semanticModel, position, cancellationToken);

            return GetDeclaredSymbols(semanticModel, container, excludeAnonymousTypeProperty, cancellationToken);
        }

        private static SyntaxNode GetEnclosingSymbolSyntax(SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            ISymbol enclosingSymbol = semanticModel.GetEnclosingSymbol(position, cancellationToken);

            ImmutableArray<SyntaxReference> syntaxReferences = enclosingSymbol.DeclaringSyntaxReferences;

            if (syntaxReferences.Length == 1)
            {
                return syntaxReferences[0].GetSyntax(cancellationToken);
            }
            else
            {
                foreach (SyntaxReference syntaxReference in syntaxReferences)
                {
                    SyntaxNode syntax = syntaxReference.GetSyntax(cancellationToken);

                    if (syntax.SyntaxTree == semanticModel.SyntaxTree)
                        return syntax;
                }
            }

            return null;
        }

        internal static ImmutableArray<ISymbol> GetDeclaredSymbols(
            this SemanticModel semanticModel,
            SyntaxNode container,
            bool excludeAnonymousTypeProperty = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HashSet<ISymbol> symbols = null;

            foreach (SyntaxNode node in container.DescendantNodesAndSelf())
            {
                ISymbol symbol = semanticModel.GetDeclaredSymbol(node, cancellationToken);

                if (symbol != null)
                {
                    if (!excludeAnonymousTypeProperty
                        || !symbol.IsAnonymousTypeProperty())
                    {
                        (symbols ?? (symbols = new HashSet<ISymbol>())).Add(symbol);
                    }
                }
            }

            return symbols?.ToImmutableArray() ?? ImmutableArray<ISymbol>.Empty;
        }
    }
}
