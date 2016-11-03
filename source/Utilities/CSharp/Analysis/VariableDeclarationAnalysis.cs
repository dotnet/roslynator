// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    public static class VariableDeclarationAnalysis
    {
        public static TypeAnalysisResult AnalyzeType(
            VariableDeclarationSyntax variableDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (variableDeclaration == null)
                throw new ArgumentNullException(nameof(variableDeclaration));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            TypeSyntax type = variableDeclaration.Type;

            if (type != null)
            {
                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;

                if (variables.Count > 0
                    && variableDeclaration.Parent?.IsKind(SyntaxKind.FieldDeclaration) != true)
                {
                    ExpressionSyntax expression = variables[0].Initializer?.Value;

                    if (expression != null)
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(type, cancellationToken).Type;

                        if (typeSymbol?.SupportsExplicitDeclaration() == true)
                        {
                            bool isVar = type.IsVar;

                            if (variables.Count > 1)
                            {
                                if (isVar)
                                {
                                    return TypeAnalysisResult.ImplicitButShouldBeExplicit;
                                }
                                else
                                {
                                    return TypeAnalysisResult.None;
                                }
                            }
                            else if (IsImplicitTypeAllowed(typeSymbol, expression, semanticModel, cancellationToken))
                            {
                                if (isVar)
                                {
                                    return TypeAnalysisResult.Implicit;
                                }
                                else
                                {
                                    return TypeAnalysisResult.ExplicitButShouldBeImplicit;
                                }
                            }
                            else
                            {
                                if (isVar)
                                {
                                    return TypeAnalysisResult.ImplicitButShouldBeExplicit;
                                }
                                else
                                {
                                    return TypeAnalysisResult.Explicit;
                                }
                            }
                        }
                    }
                }
            }

            return TypeAnalysisResult.None;
        }

        private static bool IsImplicitTypeAllowed(
            ITypeSymbol typeSymbol,
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.ArrayCreationExpression:
                case SyntaxKind.CastExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.DefaultExpression:
                    {
                        ITypeSymbol expressionTypeSymbol = semanticModel.GetTypeInfo(expression, cancellationToken).Type;

                        return expressionTypeSymbol == typeSymbol;
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        ITypeSymbol expressionTypeSymbol = semanticModel.GetTypeInfo(expression, cancellationToken).Type;

                        if (expressionTypeSymbol == typeSymbol)
                        {
                            ISymbol symbol = semanticModel.GetSymbolInfo(expression, cancellationToken).Symbol;

                            if (symbol.IsEnumField())
                                return true;
                        }

                        break;
                    }
            }

            return false;
        }
    }
}
