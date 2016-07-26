// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Analysis
{
    public static class ForEachStatementAnalysis
    {
        public static TypeAnalysisResult AnalyzeType(
            ForEachStatementSyntax forEachStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (forEachStatement.Type == null)
                return TypeAnalysisResult.None;

            if (!forEachStatement.Type.IsVar)
                return TypeAnalysisResult.Explicit;

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(forEachStatement.Type, cancellationToken).Type;

            if (ShouldBeExplicit(typeSymbol))
                return TypeAnalysisResult.ImplicitButShouldBeExplicit;

            return TypeAnalysisResult.Implicit;
        }

        private static bool ShouldBeExplicit(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null || typeSymbol.IsErrorType())
                return false;

            if (typeSymbol.IsAnonymousType)
                return false;

            if (typeSymbol.IsNamedType())
            {
                if (((INamedTypeSymbol)typeSymbol).IsAnyTypeArgumentAnonymousType())
                    return false;
            }
            else if (!typeSymbol.IsArrayType())
            {
                return false;
            }

            return true;
        }
    }
}
