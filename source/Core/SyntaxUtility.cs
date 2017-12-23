// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

            return IsPropertyOfNullableOfT(symbol, name);
        }

        public static bool IsPropertyOfNullableOfT(
            IdentifierNameSyntax identifierName,
            string name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (identifierName == null)
                return false;

            if (!string.Equals(identifierName.Identifier.ValueText, name, StringComparison.Ordinal))
                return false;

            ISymbol symbol = semanticModel.GetSymbol(identifierName, cancellationToken);

            return IsPropertyOfNullableOfT(symbol, name);
        }

        private static bool IsPropertyOfNullableOfT(ISymbol symbol, string name)
        {
            return symbol?.IsProperty() == true
                && symbol.ContainingType?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                && string.Equals(symbol.Name, name, StringComparison.Ordinal);
        }
    }
}
