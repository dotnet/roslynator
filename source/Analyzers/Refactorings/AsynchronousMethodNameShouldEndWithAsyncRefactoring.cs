// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AsynchronousMethodNameShouldEndWithAsyncRefactoring
    {
        private const string AsyncSuffix = "Async";

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol?.IsAsync == true
                && !methodSymbol.Name.EndsWith(AsyncSuffix, StringComparison.Ordinal)
                && methodDeclaration.ContainsAwait())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AsynchronousMethodNameShouldEndWithAsync,
                    methodDeclaration.Identifier);
            }
        }
    }
}
