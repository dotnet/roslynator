// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ISymbol symbol = semanticModel.GetSymbol(node, cancellationToken);

            return symbol?.IsProperty() == true
                && symbol.ContainingType?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                && string.Equals(symbol.Name, name, StringComparison.Ordinal);
        }
    }
}
