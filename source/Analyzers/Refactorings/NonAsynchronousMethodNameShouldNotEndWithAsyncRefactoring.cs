// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class NonAsynchronousMethodNameShouldNotEndWithAsyncRefactoring
    {
        private const string AsyncSuffix = "Async";

        public static void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclaration)
        {
            IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol?.IsAsync == false
                && !methodSymbol.IsAbstract
                && methodSymbol.Name.EndsWith(AsyncSuffix, StringComparison.Ordinal)
                && !methodSymbol.ReturnType.IsTaskOrInheritsFromTask(context.SemanticModel))
            {
                SyntaxToken identifier = methodDeclaration.Identifier;

                context.ReportDiagnostic(
                    DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsync,
                    identifier);

                context.ReportDiagnostic(
                    DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsyncFadeOut,
                    Location.Create(identifier.SyntaxTree, TextSpan.FromBounds(identifier.Span.End - AsyncSuffix.Length, identifier.Span.End)));
            }
        }
    }
}
