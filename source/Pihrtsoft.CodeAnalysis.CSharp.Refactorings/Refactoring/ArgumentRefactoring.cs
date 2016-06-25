// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ArgumentRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArgumentSyntax argument)
        {
            if (argument.Expression?.IsMissing == false
                && context.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(argument.Expression).ConvertedType;

                if (typeSymbol?.IsKind(SymbolKind.ErrorType) == false)
                {
                    foreach (IParameterSymbol parameterSymbol in argument.DetermineParameters(semanticModel, context.CancellationToken))
                    {
                        if (parameterSymbol.Type != null
                            && !typeSymbol.Equals(parameterSymbol.Type))
                        {
                            AddCastRefactoring.RegisterRefactoring(context, argument.Expression, parameterSymbol.Type);
                        }
                    }
                }
            }
        }
    }
}
