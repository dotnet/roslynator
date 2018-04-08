// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class CallFindInsteadOfFirstOrDefaultAnalysis
    {
        public static void Analyze(
            SyntaxNodeAnalysisContext context,
            SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            IMethodSymbol methodSymbol = semanticModel.GetReducedExtensionMethodInfo(invocationInfo.InvocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol, semanticModel, "FirstOrDefault"))
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(invocationInfo.Expression, cancellationToken);

            if (typeSymbol == null)
                return;

            if (typeSymbol.Kind == SymbolKind.ArrayType
                && ((IArrayTypeSymbol)typeSymbol).Rank == 1)
            {
                context.ReportDiagnostic(DiagnosticDescriptors.CallFindInsteadOfFirstOrDefault, invocationInfo.Name);
            }
            else if (typeSymbol.OriginalDefinition.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Collections_Generic_List_T)))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.CallFindInsteadOfFirstOrDefault, invocationInfo.Name);
            }
        }
    }
}
