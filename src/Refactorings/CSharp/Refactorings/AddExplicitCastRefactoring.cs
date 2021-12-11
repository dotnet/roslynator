// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddExplicitCastRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel,
            bool addCastExpression = true)
        {
            if (semanticModel.IsExplicitConversion(expression, destinationType)
                && context.IsRefactoringEnabled(RefactoringIdentifiers.AddExplicitCast)
                && addCastExpression)
            {
                RegisterAddExplicitCastRefactoring(context, expression, destinationType, semanticModel);
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

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddExplicitCast))
            {
                foreach (ITypeSymbol destinationType in convertibleDestinationTypes)
                    RegisterAddExplicitCastRefactoring(context, expression, destinationType, semanticModel);
            }
        }

        private static void RegisterAddExplicitCastRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol destinationType,
            SemanticModel semanticModel)
        {
            CodeAction codeAction = CodeActionFactory.AddExplicitCast(
                context.Document,
                expression,
                destinationType,
                semanticModel,
                equivalenceKey: EquivalenceKey.Join(RefactoringIdentifiers.AddExplicitCast, SymbolDisplay.ToDisplayString(destinationType)));

            context.RegisterRefactoring(codeAction);
        }
    }
}
