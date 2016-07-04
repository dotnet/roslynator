// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Analysis
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

            if (variableDeclaration.Type == null)
                return TypeAnalysisResult.None;

            if (variableDeclaration.Variables.Count == 0)
                return TypeAnalysisResult.None;

            if (variableDeclaration.Parent?.IsKind(SyntaxKind.FieldDeclaration) == true)
                return TypeAnalysisResult.None;

            ExpressionSyntax expression = variableDeclaration.Variables[0].Initializer?.Value;

            if (expression == null)
                return TypeAnalysisResult.None;

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(variableDeclaration.Type, cancellationToken).Type;

            if (typeSymbol == null || typeSymbol.IsKind(SymbolKind.ErrorType))
                return TypeAnalysisResult.None;

            if (typeSymbol.IsAnonymousType)
                return TypeAnalysisResult.None;

            if (typeSymbol.IsKind(SymbolKind.NamedType))
            {
                if (((INamedTypeSymbol)typeSymbol).IsAnyTypeArgumentAnonymousType())
                    return TypeAnalysisResult.None;
            }
            else if (!typeSymbol.IsKind(SymbolKind.ArrayType))
            {
                return TypeAnalysisResult.None;
            }

            if (variableDeclaration.Variables.Count > 1)
            {
                if (variableDeclaration.Type.IsVar)
                    return TypeAnalysisResult.ImplicitButShouldBeExplicit;
                else
                    return TypeAnalysisResult.None;
            }

            if (IsImplicitTypeAllowed(typeSymbol, expression, semanticModel, cancellationToken))
            {
                if (variableDeclaration.Type.IsVar)
                    return TypeAnalysisResult.Implicit;
                else
                    return TypeAnalysisResult.ExplicitButShouldBeImplicit;
            }
            else
            {
                if (variableDeclaration.Type.IsVar)
                    return TypeAnalysisResult.ImplicitButShouldBeExplicit;
                else
                    return TypeAnalysisResult.Explicit;
            }
        }

        private static bool IsImplicitTypeAllowed(
            ITypeSymbol typeSymbol,
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (!expression.IsAnyKind(
                SyntaxKind.ObjectCreationExpression,
                SyntaxKind.ArrayCreationExpression,
                SyntaxKind.CastExpression,
                SyntaxKind.AsExpression,
                SyntaxKind.ThisExpression))
            {
                return false;
            }

            ITypeSymbol expressionTypeSymbol = semanticModel.GetTypeInfo(expression, cancellationToken).Type;

            if (expressionTypeSymbol == null)
                return false;

            return typeSymbol.Equals(expressionTypeSymbol);
        }
    }
}
