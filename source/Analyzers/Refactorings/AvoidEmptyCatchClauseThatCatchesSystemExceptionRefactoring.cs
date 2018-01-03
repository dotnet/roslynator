// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidEmptyCatchClauseThatCatchesSystemExceptionRefactoring
    {
        public static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context, ITypeSymbol exceptionSymbol)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            var catchClause = (CatchClauseSyntax)context.Node;

            if (catchClause.Filter != null)
                return;

            if (catchClause.Block?.Statements.Any() != false)
                return;

            TypeSyntax type = catchClause.Declaration?.Type;

            if (type == null)
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(type, context.CancellationToken);

            if (typeSymbol?.Equals(exceptionSymbol) != true)
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.AvoidEmptyCatchClauseThatCatchesSystemException,
                catchClause.CatchKeyword);
        }
    }
}
