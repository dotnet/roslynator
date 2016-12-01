// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class SemanticModelExtensions
    {
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
