// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class AttributeArgumentSyntaxExtensions
    {
        public static IParameterSymbol DetermineParameter(
            this AttributeArgumentSyntax argument,
            SemanticModel semanticModel,
            bool allowParams = false,
            bool allowCandidate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (argument.NameEquals != null)
                return null;

            if (argument.Parent?.IsKind(SyntaxKind.AttributeArgumentList) != true)
                return null;

            var argumentList = (AttributeArgumentListSyntax)argument.Parent;

            if (argumentList.Parent?.IsKind(SyntaxKind.Attribute) != true)
                return null;

            var attribute = (AttributeSyntax)argument.Parent.Parent;

            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(attribute, cancellationToken);

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

            if (argument.NameColon != null && !argument.NameColon.IsMissing)
            {
                string name = argument.NameColon.Name.Identifier.ValueText;

                return parameters.FirstOrDefault(p => p.Name == name);
            }

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

        public static AttributeArgumentSyntax WithoutNameColon(this AttributeArgumentSyntax attributeArgument)
        {
            if (attributeArgument == null)
                throw new ArgumentNullException(nameof(attributeArgument));

            return attributeArgument.WithNameColon(null);
        }
    }
}
