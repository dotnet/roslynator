// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ArgumentRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArgumentSyntax argument)
        {
            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.AddToMethodInvocation)
                && argument.Expression?.IsMissing == false
                && context.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(argument.Expression).ConvertedType;

                if (typeSymbol?.IsErrorType() == false)
                {
                    IEnumerable<ITypeSymbol> newTypes = argument
                        .DetermineParameterTypes(semanticModel, context.CancellationToken)
                        .Where(f => !typeSymbol.Equals(f));

                    ModifyExpressionRefactoring.ComputeRefactoring(context, argument.Expression, newTypes, semanticModel);
                }
            }
        }
    }
}
