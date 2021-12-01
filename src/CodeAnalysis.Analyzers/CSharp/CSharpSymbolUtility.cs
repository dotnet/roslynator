// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal static class CSharpSymbolUtility
    {
        public static bool IsKindExtensionMethod(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

            if (symbol?.Kind != SymbolKind.Method)
                return false;

            if (symbol.Name != "Kind")
                return false;

            if (!symbol.ContainingType.HasMetadataName(RoslynMetadataNames.Microsoft_CodeAnalysis_CSharp_CSharpExtensions))
                return false;

            var methodSymbol = (IMethodSymbol)symbol;

            if (methodSymbol.MethodKind != MethodKind.ReducedExtension)
                return false;

            methodSymbol = methodSymbol.ReducedFrom;

            if (!methodSymbol.ReturnType.HasMetadataName(RoslynMetadataNames.Microsoft_CodeAnalysis_CSharp_SyntaxKind))
                return false;

            return methodSymbol
                .Parameters
                .SingleOrDefault(shouldThrow: false)?
                .Type
                .HasMetadataName(MetadataNames.Microsoft_CodeAnalysis_SyntaxNode) == true;
        }

        public static bool IsIsKindExtensionMethod(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

            if (symbol?.Kind != SymbolKind.Method)
                return false;

            if (symbol.Name != "IsKind")
                return false;

            if (!symbol.ContainingType.HasMetadataName(RoslynMetadataNames.Microsoft_CodeAnalysis_CSharpExtensions))
                return false;

            var methodSymbol = (IMethodSymbol)symbol;

            if (methodSymbol.MethodKind != MethodKind.ReducedExtension)
                return false;

            methodSymbol = methodSymbol.ReducedFrom;

            if (methodSymbol.ReturnType.SpecialType != SpecialType.System_Boolean)
                return false;

            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            if (parameters.Length != 2)
                return false;

            if (!parameters[0].Type.HasMetadataName(MetadataNames.Microsoft_CodeAnalysis_SyntaxNode))
                return false;

            if (!parameters[1].Type.HasMetadataName(RoslynMetadataNames.Microsoft_CodeAnalysis_CSharp_SyntaxKind))
                return false;

            return true;
        }
    }
}
