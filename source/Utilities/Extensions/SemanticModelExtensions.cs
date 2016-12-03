// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Roslynator
{
    public static class SemanticModelExtensions
    {
        internal static INamedTypeSymbol GetTypeByMetadataName(this SemanticModel semanticModel, string fullyQualifiedMetadataName)
        {
            return semanticModel
                .Compilation
                .GetTypeByMetadataName(fullyQualifiedMetadataName);
        }

        public static bool ContainsDiagnostic(
            this SemanticModel semanticModel,
            TextSpan span,
            string id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ImmutableArray<Diagnostic> diagnostics = semanticModel.GetDiagnostics(span, cancellationToken);

            for (int i = 0; i < diagnostics.Length; i++)
            {
                if (string.Equals(diagnostics[i].Id, id, StringComparison.Ordinal))
                    return true;
            }

            return false;
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
            return Microsoft.CodeAnalysis.ModelExtensions
                .GetSymbolInfo(semanticModel, node, cancellationToken)
                .Symbol;
        }

        public static IParameterSymbol GetParameterSymbol(
            this SemanticModel semanticModel,
            SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.ModelExtensions
                .GetSymbolInfo(semanticModel, node, cancellationToken)
                .Symbol as IParameterSymbol;
        }

        public static ITypeSymbol GetTypeSymbol(
            this SemanticModel semanticModel,
            SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.ModelExtensions
                .GetTypeInfo(semanticModel, node, cancellationToken)
                .Type;
        }

        public static ITypeSymbol GetConvertedTypeSymbol(
            this SemanticModel semanticModel,
            SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.ModelExtensions
                .GetTypeInfo(semanticModel, node, cancellationToken)
                .ConvertedType;
        }
    }
}
