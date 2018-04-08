// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NonAsynchronousMethodNameShouldNotEndWithAsyncAnalyzer : BaseDiagnosticAnalyzer
    {
        private const string AsyncSuffix = "Async";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsync,
                    DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsyncFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol taskType = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task);

                INamedTypeSymbol valueTaskType = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_ValueTask_T);

                var windowsRuntimeTypes = default(WindowsRuntimeAsyncTypes);

                INamedTypeSymbol asyncAction = startContext.Compilation.GetTypeByMetadataName("Windows.Foundation.IAsyncAction");

                if (asyncAction != null)
                {
                    windowsRuntimeTypes = new WindowsRuntimeAsyncTypes(
                        asyncAction: asyncAction,
                        asyncActionWithProgress: startContext.Compilation.GetTypeByMetadataName("Windows.Foundation.IAsyncActionWithProgress`1"),
                        asyncOperation: startContext.Compilation.GetTypeByMetadataName("Windows.Foundation.IAsyncOperation`1"),
                        asyncOperationWithProgress: startContext.Compilation.GetTypeByMetadataName("Windows.Foundation.IAsyncOperationWithProgress`2"));
                }

                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeMethodDeclaration(nodeContext, taskType, valueTaskType, windowsRuntimeTypes), SyntaxKind.MethodDeclaration);
            });
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskType, INamedTypeSymbol valueTaskType, WindowsRuntimeAsyncTypes windowsRuntimeAsyncTypes)
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
                if (typeSymbol.Kind == SymbolKind.TypeParameter)
                {
                    var typeParameterSymbol = (ITypeParameterSymbol)typeSymbol;

                    typeSymbol = typeParameterSymbol.ConstraintTypes.SingleOrDefault(f => f.TypeKind == TypeKind.Class, shouldThrow: false);

                    if (typeSymbol == null)
                        return false;
                }

                if (typeSymbol.IsTupleType)
                    return false;

                if (typeSymbol.SpecialType != SpecialType.None)
                    return false;

                if (!typeSymbol.TypeKind.Is(TypeKind.Class, TypeKind.Struct, TypeKind.Interface))
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
                        return true;

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

        private readonly struct WindowsRuntimeAsyncTypes
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
