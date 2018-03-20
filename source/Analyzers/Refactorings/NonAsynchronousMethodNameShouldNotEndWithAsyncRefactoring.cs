// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskType, INamedTypeSymbol valueTaskType, WindowsRuntimeAsyncTypes windowsRuntimeAsyncTypes)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (!methodDeclaration.Identifier.ValueText.EndsWith(AsyncSuffix, StringComparison.Ordinal))
                return;

            IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (methodSymbol.IsAsync)
                return;

            if (!methodSymbol.Name.EndsWith(AsyncSuffix, StringComparison.Ordinal))
                return;

            ITypeSymbol typeSymbol = methodSymbol.ReturnType;

            if (CanHaveAsyncSuffix())
                return;

            SyntaxToken identifier = methodDeclaration.Identifier;

            context.ReportDiagnostic(
                DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsync,
                identifier);

            context.ReportDiagnostic(
                DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsyncFadeOut,
                Location.Create(identifier.SyntaxTree, TextSpan.FromBounds(identifier.Span.End - AsyncSuffix.Length, identifier.Span.End)));

            bool CanHaveAsyncSuffix()
            {
                if (typeSymbol.IsTupleType)
                    return false;

                if (typeSymbol.SpecialType != SpecialType.None)
                    return false;

                if (!typeSymbol.IsTypeKind(TypeKind.Class, TypeKind.Struct, TypeKind.Interface))
                    return false;

                if (!(typeSymbol is INamedTypeSymbol namedTypeSymbol))
                    return false;

                INamedTypeSymbol constructedFrom = namedTypeSymbol.ConstructedFrom;

                if (constructedFrom.Equals(valueTaskType))
                    return true;

                if (namedTypeSymbol.EqualsOrInheritsFrom(taskType))
                    return true;

                INamedTypeSymbol asyncAction = windowsRuntimeAsyncTypes.IAsyncAction;

                if (asyncAction != null)
                {
                    if (namedTypeSymbol.Equals(asyncAction))
                        return false;

                    if (namedTypeSymbol.Implements(asyncAction, allInterfaces: true))
                        return true;

                    if (namedTypeSymbol.Arity > 0
                        && namedTypeSymbol.TypeKind == TypeKind.Interface)
                    {
                        if (constructedFrom.Equals(windowsRuntimeAsyncTypes.IAsyncActionWithProgress))
                            return true;

                        if (constructedFrom.Equals(windowsRuntimeAsyncTypes.IAsyncOperation))
                            return true;

                        if (constructedFrom.Equals(windowsRuntimeAsyncTypes.IAsyncOperationWithProgress))
                            return true;
                    }
                }

                return false;
            }
        }

        public struct WindowsRuntimeAsyncTypes
        {
            public WindowsRuntimeAsyncTypes(
                INamedTypeSymbol asyncAction,
                INamedTypeSymbol asyncActionWithProgress,
                INamedTypeSymbol asyncOperation,
                INamedTypeSymbol asyncOperationWithProgress)
            {
                IAsyncAction = asyncAction;
                IAsyncActionWithProgress = asyncActionWithProgress;
                IAsyncOperation = asyncOperation;
                IAsyncOperationWithProgress = asyncOperationWithProgress;
            }

            public INamedTypeSymbol IAsyncAction { get; }
            public INamedTypeSymbol IAsyncActionWithProgress { get; }
            public INamedTypeSymbol IAsyncOperation { get; }
            public INamedTypeSymbol IAsyncOperationWithProgress { get; }
        }
    }
}
