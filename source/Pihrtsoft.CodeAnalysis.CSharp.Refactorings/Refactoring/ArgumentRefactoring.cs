// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ArgumentRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArgumentSyntax argument)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddCastExpression)
                && argument.Expression?.IsMissing == false
                && context.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(argument.Expression).ConvertedType;

                if (typeSymbol?.IsErrorType() == false)
                {
                    foreach (ITypeSymbol parameterTypeSymbol in DetermineParameters(argument, semanticModel, context.CancellationToken).Distinct())
                    {
                        if (parameterTypeSymbol?.IsErrorType() == false
                            && !typeSymbol.Equals(parameterTypeSymbol))
                        {
                            AddCastExpressionRefactoring.RegisterRefactoring(context, argument.Expression, parameterTypeSymbol);
                        }
                    }
                }
            }
        }

        private static IEnumerable<ITypeSymbol> DetermineParameters(
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

                    if (symbol == null)
                    {
                        foreach (ISymbol candidateSymbol in symbolInfo.CandidateSymbols)
                            yield return DetermineParameter(candidateSymbol, argument, argumentList);
                    }
                    else
                    {
                        yield return DetermineParameter(symbol, argument, argumentList);
                    }
                }
            }
        }

        private static ITypeSymbol DetermineParameter(
            ISymbol symbol,
            ArgumentSyntax argument,
            BaseArgumentListSyntax argumentList)
        {
            ImmutableArray<IParameterSymbol> parameters = symbol.GetParameters();

            if (argument.NameColon != null
                && !argument.NameColon.IsMissing)
            {
                string name = argument.NameColon.Name.Identifier.ValueText;

                foreach (IParameterSymbol parameterSymbol in parameters)
                {
                    if (parameterSymbol.Name == name)
                        return parameterSymbol.Type;
                }

                return null;
            }

            int index = argumentList.Arguments.IndexOf(argument);

            if (index >= 0
                && index < parameters.Length)
            {
                IParameterSymbol parameterSymbol = parameters[index];

                if (parameterSymbol.IsParams)
                {
                    var arrayTypeSymbol = (IArrayTypeSymbol)parameterSymbol.Type;
                    return arrayTypeSymbol.ElementType;
                }

                return parameterSymbol.Type;
            }

            return null;
        }
    }
}
