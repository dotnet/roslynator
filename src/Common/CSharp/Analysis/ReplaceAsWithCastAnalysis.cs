// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class ReplaceAsWithCastAnalysis
    {
        public static bool IsFixable(
            BinaryExpressionSyntax binaryExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            AsExpressionInfo info = SyntaxInfo.AsExpressionInfo(binaryExpression);

            if (!info.Success)
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(info.Type, cancellationToken);

            if (typeSymbol == null)
                return false;

            if (!semanticModel.IsExplicitConversion(info.Expression, typeSymbol))
                return false;

            return true;
        }
    }
}