// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator;

internal static class SyntaxUtility
{
    public static bool CanConvertFromCollectionExpression(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        ITypeSymbol? typeSymbol = semanticModel.GetTypeSymbol(node, cancellationToken);

        if (typeSymbol?.IsErrorType() != false)
            return false;

        if (typeSymbol is IArrayTypeSymbol arrayType)
            return arrayType.Rank == 1;

        if (typeSymbol.HasMetadataName(MetadataNames.System_Span_T)
            || typeSymbol.HasMetadataName(MetadataNames.System_ReadOnlySpan_T)
            || typeSymbol.HasAttribute(MetadataNames.System_Runtime_CompilerServices_CollectionBuilderAttribute))
        {
            return true;
        }

        if (typeSymbol is INamedTypeSymbol namedType
            && namedType.ImplementsAny(
                SpecialType.System_Collections_IEnumerable,
                SpecialType.System_Collections_Generic_IEnumerable_T,
                allInterfaces: true))
        {
            IMethodSymbol? constructor = namedType
                .InstanceConstructors
                .SingleOrDefault(f => !f.Parameters.Any(), shouldThrow: false);

            return constructor is not null
                && semanticModel.IsAccessible(node.SpanStart, constructor);
        }

        return false;
    }

    public static bool IsPropertyOfNullableOfT(
        SyntaxNode node,
        string name,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        ISymbol? symbol = semanticModel.GetSymbol(node, cancellationToken);

        return SymbolUtility.IsPropertyOfNullableOfT(symbol, name);
    }

    public static bool IsCompositeEnumValue(
        SyntaxNode node,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        var enumTypeSymbol = (INamedTypeSymbol?)semanticModel.GetTypeSymbol(node, cancellationToken);

        if (enumTypeSymbol?.EnumUnderlyingType is not null)
        {
            Optional<object?> constantValue = semanticModel.GetConstantValue(node, cancellationToken);

            if (constantValue.HasValue)
            {
                ulong value = SymbolUtility.GetEnumValueAsUInt64(constantValue.Value!, enumTypeSymbol);

                return FlagsUtility<ulong>.Instance.IsComposite(value);
            }
        }

        return false;
    }
}
