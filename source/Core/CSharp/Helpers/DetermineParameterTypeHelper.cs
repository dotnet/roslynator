// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers
{
    internal static class DetermineParameterTypeHelper
    {
        public static ImmutableArray<ITypeSymbol> DetermineParameterTypes(
            ArgumentSyntax argument,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var argumentList = argument.Parent as BaseArgumentListSyntax;

            if (argumentList != null)
            {
                SyntaxNode parent = argumentList.Parent;

                if (parent != null)
                {
                    SymbolInfo symbolInfo = GetSymbolInfo(parent, semanticModel, cancellationToken);

                    ISymbol symbol = symbolInfo.Symbol;

                    if (symbol != null)
                    {
                        ITypeSymbol typeSymbol = DetermineParameterType(symbol, argument, argumentList);

                        if (typeSymbol?.IsErrorType() == false)
                            return ImmutableArray.Create(typeSymbol);
                    }
                    else
                    {
                        HashSet<ITypeSymbol> typeSymbols = null;

                        foreach (ISymbol candidateSymbol in symbolInfo.CandidateSymbols)
                        {
                            ITypeSymbol typeSymbol = DetermineParameterType(candidateSymbol, argument, argumentList);

                            if (typeSymbol?.IsErrorType() == false)
                            {
                                (typeSymbols ?? (typeSymbols = new HashSet<ITypeSymbol>())).Add(typeSymbol);
                            }
                        }

                        if (typeSymbols != null)
                            return typeSymbols.ToImmutableArray();
                    }
                }
            }

            return ImmutableArray<ITypeSymbol>.Empty;
        }

        private static SymbolInfo GetSymbolInfo(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var expression = node as ExpressionSyntax;

            if (expression != null)
                return semanticModel.GetSymbolInfo(expression, cancellationToken);

            var constructorInitializer = node as ConstructorInitializerSyntax;

            if (constructorInitializer != null)
                return semanticModel.GetSymbolInfo(constructorInitializer, cancellationToken);

            return default(SymbolInfo);
        }

        private static ITypeSymbol DetermineParameterType(
            ISymbol symbol,
            ArgumentSyntax argument,
            BaseArgumentListSyntax argumentList)
        {
            IParameterSymbol parameterSymbol = DetermineParameterSymbol(symbol, argument, argumentList);

            if (parameterSymbol == null)
                return null;

            RefKind refKind = parameterSymbol.RefKind;

            if (refKind == RefKind.Out)
            {
                if (argument.RefOrOutKeyword.Kind() != SyntaxKind.OutKeyword)
                    return null;
            }
            else if (refKind == RefKind.Ref)
            {
                if (argument.RefOrOutKeyword.Kind() != SyntaxKind.RefKeyword)
                    return null;
            }

            ITypeSymbol typeSymbol = parameterSymbol.Type;

            if (parameterSymbol.IsParams
                && typeSymbol.IsArrayType())
            {
                return ((IArrayTypeSymbol)typeSymbol).ElementType;
            }

            return typeSymbol;
        }

        private static IParameterSymbol DetermineParameterSymbol(
            ISymbol symbol,
            ArgumentSyntax argument,
            BaseArgumentListSyntax argumentList)
        {
            ImmutableArray<IParameterSymbol> parameters = symbol.GetParameters();

            string name = argument.NameColon?.Name?.Identifier.ValueText;

            if (name != null)
                return parameters.FirstOrDefault(f => f.Name == name);

            int index = argumentList.Arguments.IndexOf(argument);

            if (index >= 0
                && index < parameters.Length)
            {
                return parameters[index];
            }

            IParameterSymbol lastParameter = parameters.LastOrDefault();

            if (lastParameter?.IsParams == true)
                return lastParameter;

            return null;
        }
    }
}
