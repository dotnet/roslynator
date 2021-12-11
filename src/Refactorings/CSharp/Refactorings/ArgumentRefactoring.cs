// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ArgumentRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArgumentSyntax argument)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddExplicitCast))
            {
                ExpressionSyntax expression = argument.Expression;

                if (expression?.IsMissing == false)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expression).ConvertedType;

                    if (typeSymbol?.IsErrorType() == false)
                    {
                        IEnumerable<ITypeSymbol> newTypes = DetermineParameterTypeHelper.DetermineParameterTypes(argument, semanticModel, context.CancellationToken)
                            .Where(f => !SymbolEqualityComparer.Default.Equals(typeSymbol, f));

                        AddExplicitCastRefactoring.ComputeRefactoring(context, expression, newTypes, semanticModel);
                    }
                }
            }
        }
    }
}
