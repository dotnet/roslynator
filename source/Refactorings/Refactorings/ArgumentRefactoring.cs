// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class ArgumentRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArgumentSyntax argument)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddCastExpression)
                && argument.Expression?.IsMissing == false
                && context.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(argument.Expression).ConvertedType;

                if (typeSymbol?.IsErrorType() == false)
                {
                    foreach (ITypeSymbol parameterTypeSymbol in argument.DetermineParameterTypes(semanticModel, context.CancellationToken))
                    {
                        if (!typeSymbol.Equals(parameterTypeSymbol))
                        {
                            AddCastExpressionRefactoring.RegisterRefactoring(
                                context,
                                argument.Expression,
                                parameterTypeSymbol,
                                semanticModel);
                        }
                    }
                }
            }
        }
    }
}
