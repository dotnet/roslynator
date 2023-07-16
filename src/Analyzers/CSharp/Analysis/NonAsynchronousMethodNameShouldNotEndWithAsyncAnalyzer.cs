// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AsyncSuffixAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(
                    ref _supportedDiagnostics,
                    DiagnosticRules.AsynchronousMethodNameShouldEndWithAsync,
                    DiagnosticRules.NonAsynchronousMethodNameShouldNotEndWithAsync,
                    DiagnosticRules.NonAsynchronousMethodNameShouldNotEndWithAsyncFadeOut);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterCompilationStartAction(startContext =>
        {
            INamedTypeSymbol asyncAction = startContext.Compilation.GetTypeByMetadataName("Windows.Foundation.IAsyncAction");

            bool shouldCheckWindowsRuntimeTypes = asyncAction is not null;

            startContext.RegisterSyntaxNodeAction(
                c =>
                {
                    if (DiagnosticHelpers.IsAnyEffective(
                        c,
                        DiagnosticRules.AsynchronousMethodNameShouldEndWithAsync,
                        DiagnosticRules.NonAsynchronousMethodNameShouldNotEndWithAsync))
                    {
                        AnalyzeMethodDeclaration(c, shouldCheckWindowsRuntimeTypes);
                    }
                },
                SyntaxKind.MethodDeclaration);
        });
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context, bool shouldCheckWindowsRuntimeTypes)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        if (methodDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword))
            return;

        if (methodDeclaration.Identifier.ValueText.EndsWith("Async", StringComparison.Ordinal))
        {
            IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol.IsAsync)
                return;

            if (!methodSymbol.Name.EndsWith("Async", StringComparison.Ordinal))
                return;

            if (SymbolUtility.IsAwaitable(methodSymbol.ReturnType, shouldCheckWindowsRuntimeTypes)
                || IsAsyncEnumerableLike(methodSymbol.ReturnType.OriginalDefinition))
            {
                return;
            }

            SyntaxToken identifier = methodDeclaration.Identifier;

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.NonAsynchronousMethodNameShouldNotEndWithAsync,
                identifier);

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.NonAsynchronousMethodNameShouldNotEndWithAsyncFadeOut,
                Location.Create(identifier.SyntaxTree, TextSpan.FromBounds(identifier.Span.End - 5, identifier.Span.End)));
        }
        else
        {
            IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol.Name.EndsWith("Async", StringComparison.Ordinal))
                return;

            if (SymbolUtility.CanBeEntryPoint(methodSymbol))
                return;

            if (!SymbolUtility.IsAwaitable(methodSymbol.ReturnType, shouldCheckWindowsRuntimeTypes)
                && !methodSymbol.ReturnType.OriginalDefinition.HasMetadataName(in MetadataNames.System_Collections_Generic_IAsyncEnumerable_T))
            {
                return;
            }

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.AsynchronousMethodNameShouldEndWithAsync, methodDeclaration.Identifier);
        }
    }

    /// <summary>
    /// Checks if the type implements <see cref="IAsyncEnumerable{T}"/> or a duck typed <c>GetAsyncEnumerator()</c>
    /// directly, through a base type, interface, or a generic constraint.
    /// </summary>
    private static bool IsAsyncEnumerableLike(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.Kind == SymbolKind.TypeParameter)
        {
            foreach (ITypeSymbol constraint in ((ITypeParameterSymbol)typeSymbol).ConstraintTypes)
            {
                if (constraint.TypeKind.Is(TypeKind.Class, TypeKind.Interface)
                    && IsAsyncEnumerable(constraint))
                {
                    return true;
                }
            }

            return false;
        }

        return typeSymbol.TypeKind.Is(TypeKind.Class, TypeKind.Struct, TypeKind.Interface)
            && IsAsyncEnumerable(typeSymbol);

        static bool IsAsyncEnumerable(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.SpecialType != SpecialType.None)
                return false;

            if (typeSymbol.IsTupleType)
                return false;

            if (typeSymbol.HasMetadataName(in MetadataNames.System_Collections_Generic_IAsyncEnumerable_T))
                return true;

            if (typeSymbol.Implements(in MetadataNames.System_Collections_Generic_IAsyncEnumerable_T, allInterfaces: true))
                return true;

            // Seek a non-static method called GetAsyncEnumerator that has no parameters or a single CancellationToken parameter.
            do
            {
                foreach (ISymbol member in typeSymbol.GetMembers("GetAsyncEnumerator"))
                {
                    if (member is IMethodSymbol { IsStatic: false } getEnumeratorMethodSymbol)
                    {
                        ImmutableArray<IParameterSymbol> parameters = getEnumeratorMethodSymbol.Parameters;

                        if (parameters.Length == 0)
                            return true;

                        if (parameters.Length == 1 && parameters[0].Type.HasMetadataName(in MetadataNames.System_Threading_CancellationToken))
                            return true;
                    }
                }
            }
            while ((typeSymbol = typeSymbol.BaseType) is not null && !typeSymbol.IsObject());

            return false;
        }
    }
}
