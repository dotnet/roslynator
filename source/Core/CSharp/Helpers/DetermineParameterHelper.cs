// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers
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

            if (argumentList != null)
            {
                SyntaxNode parent = argumentList.Parent;

                if (parent != null)
                {
                    SymbolInfo symbolInfo = GetSymbolInfo(parent, semanticModel, cancellationToken);

                    ISymbol symbol = symbolInfo.Symbol;

                    if (symbol == null
                        && allowCandidate)
                    {
                        symbol = symbolInfo.CandidateSymbols.FirstOrDefault();
                    }

                    if (symbol != null)
                    {
                        ImmutableArray<IParameterSymbol> parameters = GetParameters(symbol);

                        string name = argument.NameColon?.Name?.Identifier.ValueText;

                        if (name != null)
                            return parameters.FirstOrDefault(p => p.Name == name);

                        int index = argumentList.Arguments.IndexOf(argument);

                        if (index >= 0)
                        {
                            if (index < parameters.Length)
                                return parameters[index];

                            if (allowParams)
                            {
                                IParameterSymbol lastParameter = parameters.LastOrDefault();

                                if (lastParameter?.IsParams == true)
                                    return lastParameter;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static IParameterSymbol DetermineParameter(
            AttributeArgumentSyntax attributeArgument,
            SemanticModel semanticModel,
            bool allowParams = false,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (attributeArgument == null)
                throw new ArgumentNullException(nameof(attributeArgument));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (attributeArgument.NameEquals == null)
            {
                SyntaxNode parent = attributeArgument.Parent;

                if (parent?.IsKind(SyntaxKind.AttributeArgumentList) == true)
                {
                    var argumentList = (AttributeArgumentListSyntax)parent;

                    parent = argumentList.Parent;

                    if (parent?.IsKind(SyntaxKind.Attribute) == true)
                    {
                        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo((AttributeSyntax)parent, cancellationToken);

                        ISymbol symbol = symbolInfo.Symbol;

                        if (symbol == null
                            && allowCandidate)
                        {
                            symbol = symbolInfo.CandidateSymbols.FirstOrDefault();
                        }

                        if (symbol != null)
                        {
                            ImmutableArray<IParameterSymbol> parameters = GetParameters(symbol);

                            string name = attributeArgument.NameColon?.Name?.Identifier.ValueText;

                            if (name != null)
                                return parameters.FirstOrDefault(p => p.Name == name);

                            int index = argumentList.Arguments.IndexOf(attributeArgument);

                            if (index >= 0)
                            {
                                if (index < parameters.Length)
                                    return parameters[index];

                                if (allowParams)
                                {
                                    IParameterSymbol lastParameter = parameters.LastOrDefault();

                                    if (lastParameter?.IsParams == true)
                                        return lastParameter;
                                }
                            }
                        }
                    }
                }
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

        private static ImmutableArray<IParameterSymbol> GetParameters(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                    return ((IMethodSymbol)symbol).Parameters;
                case SymbolKind.Property:
                    return ((IPropertySymbol)symbol).Parameters;
                default:
                    return ImmutableArray<IParameterSymbol>.Empty;
            }
        }
    }
}
