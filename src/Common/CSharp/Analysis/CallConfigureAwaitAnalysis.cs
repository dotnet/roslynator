// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class CallConfigureAwaitAnalysis
    {
        public static bool IsFixable(
            AwaitExpressionSyntax awaitExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            ISymbol symbol = semanticModel.GetSymbol(awaitExpression.Expression, cancellationToken);

            if (!(GetTypeOrReturnType() is INamedTypeSymbol returnType))
                return false;

            INamedTypeSymbol originalDefinition = returnType.OriginalDefinition;

            return originalDefinition.HasMetadataName(MetadataNames.System_Threading_Tasks_ValueTask_T)
                || originalDefinition.EqualsOrInheritsFrom(MetadataNames.System_Threading_Tasks_Task);

            ITypeSymbol GetTypeOrReturnType()
            {
                switch (symbol?.Kind)
                {
                    case SymbolKind.Field:
                        return ((IFieldSymbol)symbol).Type;
                    case SymbolKind.Local:
                        return ((ILocalSymbol)symbol).Type;
                    case SymbolKind.Method:
                        return ((IMethodSymbol)symbol).ReturnType;
                    case SymbolKind.Parameter:
                        return ((IParameterSymbol)symbol).Type;
                    case SymbolKind.Property:
                        return ((IPropertySymbol)symbol).Type;
                    default:
                        return null;
                }
            }
        }
    }
}
