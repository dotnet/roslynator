// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// The original rule does not consider the following four return types for Universal Windows Platform:
//      Windows.Foundation.IAsyncAction
//      Windows.Foundation.IAsyncOperation<TResult>
//      Windows.Foundation.IAsyncActionWithProgress<TProgress>
//      Windows.Foundation.IAsyncOperationWithProgress<TResult, TProgress>

// Meanwhile, the rule ignores names of those abstract methods.
// In my personal viewpoint, these methods should also get included into considerations.

#define DO_NOT_PASS_ABSTRACT

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class NonAsynchronousMethodNameShouldNotEndWithAsyncRefactoring
    {
        private const string AsyncSuffix = "Async";

        private static bool ShouldAddAsyncSuffix(this ITypeSymbol type, SemanticModel model)
        {
            // skip some common types
            if (type.IsVoid()
                || type.IsString()
                || type.IsArrayType()
                || type.IsPredefinedValueType()
                || type.IsTupleType
                || type.IsEnum()
                || type.MetadataName.Length <= 0)
                return false;

            if (type.IsTaskOrInheritsFromTask(model))
                return true;

            // check if the return type implements Windows.Foundation.IAsyncAction
            INamedTypeSymbol symbol = model.GetTypeByMetadataName("Windows.Foundation.IAsyncAction");
            if (type.EqualsOrInheritsFrom(symbol))
                return true;

            // then we will check if the type is directly one of the remaining three mentioned interfaces
            // notice that, it is quite rare that we create types which implement those interfaces
            // for effciency, we will not check the derived parent types
            if (type is INamedTypeSymbol namedType
                && namedType.IsInterface()
                && namedType.IsGenericType)
            {
                if (namedType.IsConstructedFrom(model.GetTypeByMetadataName("Windows.Foundation.IAsyncOperation`1"))
                    || namedType.IsConstructedFrom(model.GetTypeByMetadataName("Windows.Foundation.IAsyncActionWithProgress`1"))
                    || namedType.IsConstructedFrom(model.GetTypeByMetadataName("Windows.Foundation.IAsyncOperationWithProgress`2")))
                    return true;
            }

            return false;
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclaration)
        {
            IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol?.IsAsync == false
#if !DO_NOT_PASS_ABSTRACT
                && !methodSymbol.IsAbstract
#endif
                && methodSymbol.Name.EndsWith(AsyncSuffix, StringComparison.Ordinal)
                && !methodSymbol.ReturnType.ShouldAddAsyncSuffix(context.SemanticModel))
            {
                SyntaxToken identifier = methodDeclaration.Identifier;

                context.ReportDiagnostic(
                    DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsync,
                    identifier);

                context.ReportDiagnostic(
                    DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsyncFadeOut,
                    Location.Create(identifier.SyntaxTree, TextSpan.FromBounds(identifier.Span.End - AsyncSuffix.Length, identifier.Span.End)));
            }
        }
    }
}
