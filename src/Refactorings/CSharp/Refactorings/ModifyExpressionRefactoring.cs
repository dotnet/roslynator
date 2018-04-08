// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ModifyExpressionRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel,
            bool addCastExpression = true)
        {
            if (semanticModel.IsExplicitConversion(expression, destinationType))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.CallToMethod))
                    CallToMethod(context, expression, destinationType, semanticModel);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddCastExpression)
                    && addCastExpression)
                {
                    RegisterAddCastExpressionRefactoring(context, expression, destinationType, semanticModel);
                }
            }
            else if (destinationType.IsString())
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.CallToMethod))
                    CallToString(context, expression, destinationType);
            }
        }

        public static void ComputeRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            IEnumerable<ITypeSymbol> destinationTypes,
            SemanticModel semanticModel)
        {
            ITypeSymbol[] convertibleDestinationTypes = destinationTypes
                .Where(destinationType => semanticModel.IsExplicitConversion(expression, destinationType))
                .ToArray();

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CallToMethod))
            {
                bool fString = false;

                foreach (ITypeSymbol destinationType in convertibleDestinationTypes)
                {
                    if (CallToMethod(context, expression, destinationType, semanticModel))
                    {
                        if (destinationType.IsString())
                            fString = true;

                        break;
                    }
                }

                if (!fString)
                {
                    ITypeSymbol stringType = destinationTypes.FirstOrDefault(f => f.IsString());

                    if (stringType != null)
                        CallToString(context, expression, stringType);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddCastExpression))
            {
                foreach (ITypeSymbol destinationType in convertibleDestinationTypes)
                    RegisterAddCastExpressionRefactoring(context, expression, destinationType, semanticModel);
            }
        }

        private static bool CallToMethod(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel)
        {
            if (destinationType.IsString())
            {
                CallToString(context, expression, destinationType);
                return true;
            }

            switch (destinationType.Kind)
            {
                case SymbolKind.ArrayType:
                    {
                        CallToArray(context, expression, semanticModel);
                        return true;
                    }
                case SymbolKind.NamedType:
                    {
                        CallToList(context, expression, (INamedTypeSymbol)destinationType, semanticModel);
                        return true;
                    }
            }

            return false;
        }

        private static void CallToString(RefactoringContext context, ExpressionSyntax expression, ITypeSymbol destinationType)
        {
            if (!(expression is LiteralExpressionSyntax))
                CallToMethodRefactoring.ComputeRefactoring(context, expression, destinationType, "ToString");
        }

        private static void CallToArray(
            RefactoringContext context,
            ExpressionSyntax expression,
            SemanticModel semanticModel)
        {
            if (!(semanticModel.GetTypeSymbol(expression, context.CancellationToken) is INamedTypeSymbol typeSymbol))
                return;

            INamedTypeSymbol constructedFrom = typeSymbol.ConstructedFrom;

            if (!constructedFrom.IsOrImplements(SpecialType.System_Collections_Generic_IEnumerable_T, allInterfaces: true))
                return;

            INamedTypeSymbol enumerable = semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable);

            if (enumerable != null)
                CallToMethodRefactoring.ComputeRefactoring(context, expression, enumerable, "ToArray");
        }

        private static void CallToList(
            RefactoringContext context,
            ExpressionSyntax expression,
            INamedTypeSymbol destinationType,
            SemanticModel semanticModel)
        {
            if (!destinationType.ConstructedFrom.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Collections_Generic_List_T)))
                return;

            INamedTypeSymbol enumerable = semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable);

            if (enumerable != null)
                CallToMethodRefactoring.ComputeRefactoring(context, expression, enumerable, "ToList");
        }

        private static void RegisterAddCastExpressionRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel)
        {
            context.RegisterRefactoring(
                $"Cast to '{SymbolDisplay.ToDisplayString(destinationType, SymbolDisplayFormats.Default)}'",
                cancellationToken =>
                {
                    return AddCastExpressionRefactoring.RefactorAsync(
                        context.Document,
                        expression,
                        destinationType,
                        semanticModel,
                        cancellationToken);
                });
        }
    }
}