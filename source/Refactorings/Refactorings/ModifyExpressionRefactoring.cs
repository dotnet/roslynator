// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ModifyExpressionRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel)
        {
            if (semanticModel.IsExplicitConversion(expression, destinationType))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddToMethodInvocation))
                    AddToMethodInvocation(context, expression, destinationType, semanticModel);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddCastExpression))
                    AddCastExpressionRefactoring.RegisterRefactoring(context, expression, destinationType);
            }
        }

        public static void ComputeRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            IEnumerable<ITypeSymbol> destinationTypes,
            SemanticModel semanticModel)
        {
            destinationTypes = destinationTypes
                .Where(destinationType => semanticModel.IsExplicitConversion(expression, destinationType))
                .ToArray();

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddToMethodInvocation))
            {
                foreach (ITypeSymbol destinationType in destinationTypes)
                {
                    if (AddToMethodInvocation(context, expression, destinationType, semanticModel))
                        break;
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddCastExpression))
            {
                foreach (ITypeSymbol destinationType in destinationTypes)
                    AddCastExpressionRefactoring.RegisterRefactoring(context, expression, destinationType);
            }
        }

        private static bool AddToMethodInvocation(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel)
        {
            if (destinationType.IsString())
            {
                AddToMethodInvocationRefactoring.ComputeRefactoring(context, expression, destinationType, "ToString");
                return true;
            }
            else if (destinationType.IsArrayType())
            {
                AddToArray(context, expression, (IArrayTypeSymbol)destinationType, semanticModel);
                return true;
            }
            else if (destinationType.IsNamedType())
            {
                AddToList(context, expression, (INamedTypeSymbol)destinationType, semanticModel);
                return true;
            }

            return false;
        }

        private static void AddToArray(
            RefactoringContext context,
            ExpressionSyntax expression,
            IArrayTypeSymbol arrayType,
            SemanticModel semanticModel)
        {
            ITypeSymbol expressionType = semanticModel.GetTypeInfo(expression, context.CancellationToken).Type;

            if (expressionType?.IsNamedType() == true)
            {
                INamedTypeSymbol constructedFrom = ((INamedTypeSymbol)expressionType).ConstructedFrom;

                if (constructedFrom.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T
                    || constructedFrom.Implements(SpecialType.System_Collections_Generic_IEnumerable_T))
                {
                    INamedTypeSymbol enumerable = semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable");

                    if (enumerable != null)
                        AddToMethodInvocationRefactoring.ComputeRefactoring(context, expression, enumerable, "ToArray");
                }
            }
        }

        private static void AddToList(
            RefactoringContext context,
            ExpressionSyntax expression,
            INamedTypeSymbol destinationType,
            SemanticModel semanticModel)
        {
            INamedTypeSymbol list = semanticModel.Compilation.GetTypeByMetadataName("System.Collections.Generic.List`1");

            if (list != null && destinationType.ConstructedFrom == list)
            {
                INamedTypeSymbol enumerable = semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable");

                if (enumerable != null)
                    AddToMethodInvocationRefactoring.ComputeRefactoring(context, expression, enumerable, "ToList");
            }
        }
    }
}