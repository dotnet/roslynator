// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public static class CSharpExtensions
    {
        public static ISymbol GetSymbol(
            this SemanticModel semanticModel,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpExtensions
                .GetSymbolInfo(semanticModel, expression, cancellationToken)
                .Symbol;
        }

        public static ITypeSymbol GetTypeSymbol(
            this SemanticModel semanticModel,
            AttributeSyntax attribute,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpExtensions
                .GetTypeInfo(semanticModel, attribute, cancellationToken)
                .Type;
        }

        public static ITypeSymbol GetTypeSymbol(
            this SemanticModel semanticModel,
            ConstructorInitializerSyntax constructorInitializer,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpExtensions
                .GetTypeInfo(semanticModel, constructorInitializer, cancellationToken)
                .Type;
        }

        public static ITypeSymbol GetTypeSymbol(
            this SemanticModel semanticModel,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpExtensions
                .GetTypeInfo(semanticModel, expression, cancellationToken)
                .Type;
        }

        public static ITypeSymbol GetTypeSymbol(
            this SemanticModel semanticModel,
            SelectOrGroupClauseSyntax selectOrGroupClause,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpExtensions
                .GetTypeInfo(semanticModel, selectOrGroupClause, cancellationToken)
                .Type;
        }

        public static ITypeSymbol GetConvertedTypeSymbol(
            this SemanticModel semanticModel,
            AttributeSyntax attribute,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpExtensions
                .GetTypeInfo(semanticModel, attribute, cancellationToken)
                .ConvertedType;
        }

        public static ITypeSymbol GetConvertedTypeSymbol(
            this SemanticModel semanticModel,
            ConstructorInitializerSyntax constructorInitializer,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpExtensions
                .GetTypeInfo(semanticModel, constructorInitializer, cancellationToken)
                .ConvertedType;
        }

        public static ITypeSymbol GetConvertedTypeSymbol(
            this SemanticModel semanticModel,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpExtensions
                .GetTypeInfo(semanticModel, expression, cancellationToken)
                .ConvertedType;
        }

        public static ITypeSymbol GetConvertedTypeSymbol(
            this SemanticModel semanticModel,
            SelectOrGroupClauseSyntax selectOrGroupClause,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpExtensions
                .GetTypeInfo(semanticModel, selectOrGroupClause, cancellationToken)
                .ConvertedType;
        }

        public static IMethodSymbol GetMethodSymbol(
            this SemanticModel semanticModel,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpExtensions
                .GetSymbolInfo(semanticModel, expression, cancellationToken)
                .Symbol as IMethodSymbol;
        }

        public static bool IsExplicitConversion(this SemanticModel semanticModel, ExpressionSyntax expression, ITypeSymbol destinationType)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));

            if (!destinationType.IsErrorType()
                && !destinationType.IsVoid())
            {
                Conversion conversion = semanticModel.ClassifyConversion(
                    expression,
                    destinationType,
                    isExplicitInSource: false);

                if (conversion.IsExplicit)
                    return true;
            }

            return false;
        }
    }
}
