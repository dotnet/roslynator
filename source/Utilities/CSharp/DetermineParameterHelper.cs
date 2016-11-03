// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal static class DetermineParameterHelper
    {
        public static IParameterSymbol DetermineParameter(
            ArgumentSyntax argument,
            SemanticModel semanticModel,
            bool allowParams = false,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            var argumentList = argument.Parent as BaseArgumentListSyntax;

            if (argumentList == null)
                return null;

            SymbolInfo symbolInfo = GetSymbolInfo(argumentList.Parent, semanticModel, cancellationToken);

            ISymbol symbol = symbolInfo.Symbol;

            if (symbol == null
                && allowCandidate
                && symbolInfo.CandidateSymbols.Length > 0)
            {
                symbol = symbolInfo.CandidateSymbols[0];
            }

            if (symbol == null)
                return null;

            ImmutableArray<IParameterSymbol> parameters = symbol.GetParameters();

            string name = argument.NameColon?.Name?.Identifier.ValueText;

            if (name!= null)
                return parameters.FirstOrDefault(p => p.Name == name);

            int index = argumentList.Arguments.IndexOf(argument);

            if (index < 0)
                return null;

            if (index < parameters.Length)
                return parameters[index];

            if (allowParams)
            {
                IParameterSymbol lastParameter = parameters.LastOrDefault();

                if (lastParameter == null)
                    return null;

                if (lastParameter.IsParams)
                    return lastParameter;
            }

            return null;
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

        public static ImmutableArray<ITypeSymbol> DetermineParameterTypes(
            ArgumentSyntax argument,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var argumentList = argument.Parent as BaseArgumentListSyntax;

            if (argumentList != null)
            {
                var invocableExpression = argumentList.Parent as ExpressionSyntax;

                if (invocableExpression != null)
                {
                    SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(invocableExpression, cancellationToken);

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
                                if (typeSymbols == null)
                                    typeSymbols = new HashSet<ITypeSymbol>();

                                typeSymbols.Add(typeSymbol);
                            }
                        }

                        if (typeSymbols != null)
                            return typeSymbols.ToImmutableArray();
                    }
                }
            }

            return ImmutableArray<ITypeSymbol>.Empty;
        }

        private static ITypeSymbol DetermineParameterType(
            ISymbol symbol,
            ArgumentSyntax argument,
            BaseArgumentListSyntax argumentList)
        {
            IParameterSymbol parameterSymbol = DetermineParameterSymbol(symbol, argument, argumentList);

            if (parameterSymbol != null)
                return GetParameterType(parameterSymbol);

            return null;
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

        private static ITypeSymbol GetParameterType(IParameterSymbol parameterSymbol)
        {
            ITypeSymbol typeSymbol = parameterSymbol.Type;

            if (parameterSymbol.IsParams
                && typeSymbol.IsArrayType())
            {
                var arrayTypeSymbol = (IArrayTypeSymbol)typeSymbol;
                return arrayTypeSymbol.ElementType;
            }

            return typeSymbol;
        }
    }
}
