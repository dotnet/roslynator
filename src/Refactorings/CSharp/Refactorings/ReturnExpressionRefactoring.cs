// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReturnExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (expression == null)
                return;

            if (expression.IsMissing)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            (ISymbol memberSymbol, ITypeSymbol memberTypeSymbol) = GetContainingSymbolAndType(expression, semanticModel, context.CancellationToken);

            if (memberSymbol == null)
                return;

            if (memberTypeSymbol?.IsErrorType() != false)
                return;

            ITypeSymbol expressionSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (expressionSymbol?.IsErrorType() != false)
                return;

            ITypeSymbol castTypeSymbol = GetCastTypeSymbol(memberSymbol, memberTypeSymbol, expressionSymbol, semanticModel);

            if (castTypeSymbol == null)
                return;

            ModifyExpressionRefactoring.ComputeRefactoring(context, expression, castTypeSymbol, semanticModel, addCastExpression: false);
        }

        private static ITypeSymbol GetCastTypeSymbol(
            ISymbol memberSymbol,
            ITypeSymbol memberTypeSymbol,
            ITypeSymbol expressionSymbol,
            SemanticModel semanticModel)
        {
            if (memberSymbol.IsAsyncMethod())
            {
                if (memberTypeSymbol.Kind == SymbolKind.NamedType)
                {
                    var namedTypeSymbol = (INamedTypeSymbol)memberTypeSymbol;

                    INamedTypeSymbol taskOfTSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T);

                    if (taskOfTSymbol != null
                        && namedTypeSymbol.ConstructedFrom.Equals(taskOfTSymbol)
                        && !namedTypeSymbol.TypeArguments[0].Equals(expressionSymbol))
                    {
                        return namedTypeSymbol.TypeArguments[0];
                    }
                }
            }
            else if (!memberTypeSymbol.Equals(expressionSymbol))
            {
                return memberTypeSymbol;
            }

            return null;
        }

        internal static (ISymbol symbol, ITypeSymbol typeSymbol) GetContainingSymbolAndType(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (semanticModel.GetEnclosingSymbol(expression.SpanStart, cancellationToken))
            {
                case IMethodSymbol methodSymbol:
                    {
                        MethodKind methodKind = methodSymbol.MethodKind;

                        if (methodKind == MethodKind.PropertyGet)
                        {
                            var propertySymbol = (IPropertySymbol)methodSymbol.AssociatedSymbol;

                            return (propertySymbol, propertySymbol.Type);
                        }

                        if (methodKind == MethodKind.Ordinary
                            && methodSymbol.PartialImplementationPart != null)
                        {
                            methodSymbol = methodSymbol.PartialImplementationPart;
                        }

                        return (methodSymbol, methodSymbol.ReturnType);
                    }
                case IFieldSymbol fieldSymbol:
                    {
                        return (fieldSymbol, fieldSymbol.Type);
                    }
                case IErrorTypeSymbol _:
                    {
                        return default((ISymbol, ITypeSymbol));
                    }
            }

            Debug.Fail(expression.ToString());

            return default((ISymbol, ITypeSymbol));
        }
    }
}
