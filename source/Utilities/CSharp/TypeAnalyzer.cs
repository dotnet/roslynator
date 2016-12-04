// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public static class TypeAnalyzer
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
                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

                        if (typeSymbol?.SupportsExplicitDeclaration() == true)
                        {
                            if (variables.Count > 1)
                            {
                                return (type.IsVar)
                                    ? TypeAnalysisResult.ImplicitButShouldBeExplicit
                                    : TypeAnalysisResult.None;
                            }
                            else if (IsImplicitTypeAllowed(typeSymbol, expression, semanticModel, cancellationToken))
                            {
                                return (type.IsVar)
                                    ? TypeAnalysisResult.Implicit
                                    : TypeAnalysisResult.ExplicitButShouldBeImplicit;
                            }
                            else
                            {
                                return (type.IsVar)
                                    ? TypeAnalysisResult.ImplicitButShouldBeExplicit
                                    : TypeAnalysisResult.Explicit;
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
                        return typeSymbol.Equals(semanticModel.GetTypeSymbol(expression, cancellationToken));
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        if (typeSymbol.Equals(semanticModel.GetTypeSymbol(expression, cancellationToken)))
                        {
                            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

                            return symbol?.Kind == SymbolKind.Field
                                && symbol.ContainingType?.TypeKind == TypeKind.Enum;
                        }

                        break;
                    }
            }

            return false;
        }

        public static TypeAnalysisResult AnalyzeType(
            ForEachStatementSyntax forEachStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            TypeSyntax type = forEachStatement.Type;

            if (type == null)
                return TypeAnalysisResult.None;

            if (!type.IsVar)
                return TypeAnalysisResult.Explicit;

            if (semanticModel
                .GetTypeSymbol(type, cancellationToken)?
                .SupportsExplicitDeclaration() == true)
            {
                return TypeAnalysisResult.ImplicitButShouldBeExplicit;
            }

            return TypeAnalysisResult.Implicit;
        }
    }
}
