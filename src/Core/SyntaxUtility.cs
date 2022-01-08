// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class SyntaxUtility
    {
        public static bool IsPropertyOfNullableOfT(
            SyntaxNode node,
            string name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            ISymbol symbol = semanticModel.GetSymbol(node, cancellationToken);

            return SymbolUtility.IsPropertyOfNullableOfT(symbol, name);
        }

        public static bool IsCompositeEnumValue(
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            var enumTypeSymbol = (INamedTypeSymbol)semanticModel.GetTypeSymbol(node, cancellationToken);

            if (enumTypeSymbol.EnumUnderlyingType != null)
            {
                Optional<object> constantValue = semanticModel.GetConstantValue(node, cancellationToken);

                if (constantValue.HasValue)
                {
                    ulong value = SymbolUtility.GetEnumValueAsUInt64(constantValue.Value, enumTypeSymbol);

                    return FlagsUtility<ulong>.Instance.IsComposite(value);
                }
            }

            return false;
        }
    }
}
