// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseEmptyStringLiteralInsteadOfStringEmptyAnalysis
    {
        public static bool IsFixable(
            MemberAccessExpressionSyntax memberAccess,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                return false;

            if (memberAccess.Expression == null)
                return false;

            if (memberAccess.Name?.Identifier.ValueText != "Empty")
                return false;

            var fieldSymbol = semanticModel.GetSymbol(memberAccess.Name, cancellationToken) as IFieldSymbol;

            return SymbolUtility.IsPublicStaticReadOnly(fieldSymbol)
                && fieldSymbol.ContainingType?.SpecialType == SpecialType.System_String;
        }
    }
}
