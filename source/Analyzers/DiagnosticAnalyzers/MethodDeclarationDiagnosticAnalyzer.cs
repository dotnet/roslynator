// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodDeclarationDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        private const string AsyncSuffix = "Async";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AsynchronousMethodNameShouldEndWithAsync,
                    DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsync,
                    DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsyncFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol != null)
            {
                SyntaxToken identifier = methodDeclaration.Identifier;

                if (methodSymbol.IsAsync)
                {
                    if (!methodSymbol.Name.EndsWith(AsyncSuffix, StringComparison.Ordinal)
                        && AsyncAnalysis.ContainsAwait(methodDeclaration))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.AsynchronousMethodNameShouldEndWithAsync,
                            identifier.GetLocation());
                    }
                }
                else if (!methodSymbol.IsAbstract
                    && methodSymbol.Name.EndsWith(AsyncSuffix, StringComparison.Ordinal)
                    && ShouldRemoveSuffix(methodSymbol, context.SemanticModel))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsync,
                        identifier.GetLocation());

                    context.ReportDiagnostic(
                        DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsyncFadeOut,
                        Location.Create(identifier.SyntaxTree, TextSpan.FromBounds(identifier.Span.End - AsyncSuffix.Length, identifier.Span.End)));
                }
            }
        }

        private static bool ShouldRemoveSuffix(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            var returnTypeSymbol = methodSymbol.ReturnType as INamedTypeSymbol;

            if (returnTypeSymbol != null)
            {
                INamedTypeSymbol taskSymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");

                if (taskSymbol?.Equals(returnTypeSymbol) == false)
                {
                    INamedTypeSymbol taskOfTSymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");

                    if (taskOfTSymbol?.Equals(returnTypeSymbol.ConstructedFrom) == false)
                        return true;
                }
            }

            return false;
        }
    }
}
