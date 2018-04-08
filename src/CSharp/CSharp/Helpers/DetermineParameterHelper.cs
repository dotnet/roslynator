// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
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
            if (!(argument.Parent is BaseArgumentListSyntax argumentList))
                return null;

            if (argumentList.Parent == null)
                return null;

            ISymbol symbol = GetSymbol(argumentList.Parent, allowCandidate, semanticModel, cancellationToken);

            if (symbol == null)
                return null;

            ImmutableArray<IParameterSymbol> parameters = symbol.ParametersOrDefault();

            Debug.Assert(!parameters.IsDefault, symbol.Kind.ToString());

            if (parameters.IsDefault)
                return null;

            return DetermineParameter(argument, argumentList.Arguments, parameters, allowParams);
        }

        internal static IParameterSymbol DetermineParameter(
            ArgumentSyntax argument,
            SeparatedSyntaxList<ArgumentSyntax> arguments,
            ImmutableArray<IParameterSymbol> parameters,
            bool allowParams = false)
        {
            string name = argument.NameColon?.Name?.Identifier.ValueText;

            if (name != null)
            {
                foreach (IParameterSymbol parameter in parameters)
                {
                    if (parameter.Name == name)
                        return parameter;
                }

                return null;
            }

            int index = arguments.IndexOf(argument);

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

            return null;
        }

        public static IParameterSymbol DetermineParameter(
            AttributeArgumentSyntax attributeArgument,
            SemanticModel semanticModel,
            bool allowParams = false,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (attributeArgument.NameEquals != null)
                return null;

            SyntaxNode parent = attributeArgument.Parent;

            if (!(parent is AttributeArgumentListSyntax argumentList))
                return null;

            if (!(argumentList.Parent is AttributeSyntax attribute))
                return null;

            ISymbol symbol = GetSymbol(attribute, allowCandidate, semanticModel, cancellationToken);

            if (symbol == null)
                return null;

            ImmutableArray<IParameterSymbol> parameters = symbol.ParametersOrDefault();

            Debug.Assert(!parameters.IsDefault, symbol.Kind.ToString());

            if (parameters.IsDefault)
                return null;

            return DetermineParameter(attributeArgument, argumentList.Arguments, parameters, allowParams);
        }

        internal static IParameterSymbol DetermineParameter(
            AttributeArgumentSyntax attributeArgument,
            SeparatedSyntaxList<AttributeArgumentSyntax> arguments,
            ImmutableArray<IParameterSymbol> parameters,
            bool allowParams = false)
        {
            string name = attributeArgument.NameColon?.Name?.Identifier.ValueText;

            if (name != null)
            {
                foreach (IParameterSymbol parameter in parameters)
                {
                    if (parameter.Name == name)
                        return parameter;
                }

                return null;
            }

            int index = arguments.IndexOf(attributeArgument);

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

            return null;
        }

        private static ISymbol GetSymbol(SyntaxNode node, bool allowCandidate, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SymbolInfo symbolInfo = GetSymbolInfo(node, semanticModel, cancellationToken);

            ISymbol symbol = symbolInfo.Symbol;

            if (symbol == null
                && allowCandidate)
            {
                return symbolInfo.CandidateSymbols.FirstOrDefault();
            }

            return symbol;
        }

        private static SymbolInfo GetSymbolInfo(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (node is ExpressionSyntax expression)
                return semanticModel.GetSymbolInfo(expression, cancellationToken);

            if (node is ConstructorInitializerSyntax constructorInitializer)
                return semanticModel.GetSymbolInfo(constructorInitializer, cancellationToken);

            return default(SymbolInfo);
        }
    }
}
