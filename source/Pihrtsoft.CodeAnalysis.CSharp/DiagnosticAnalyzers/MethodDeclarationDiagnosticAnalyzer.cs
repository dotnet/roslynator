// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
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
                    DiagnosticDescriptors.AsyncMethodShouldHaveAsyncSuffix,
                    DiagnosticDescriptors.NonAsyncMethodShouldNotHaveAsyncSuffix,
                    DiagnosticDescriptors.NonAsyncMethodShouldNotHaveAsyncSuffixFadeOut);
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

            if (methodSymbol == null || methodSymbol.IsImplicitlyDeclared)
                return;

            if (methodSymbol.IsAsync)
            {
                if (!methodSymbol.Name.EndsWith(AsyncSuffix, StringComparison.Ordinal))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AsyncMethodShouldHaveAsyncSuffix,
                        methodDeclaration.Identifier.GetLocation());
                }
            }
            else if (methodSymbol.Name.EndsWith(AsyncSuffix, StringComparison.Ordinal))
            {
                SyntaxToken identifier = methodDeclaration.Identifier;

                if (identifier.ValueText.EndsWith(AsyncSuffix, StringComparison.Ordinal))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.NonAsyncMethodShouldNotHaveAsyncSuffix,
                        identifier.GetLocation());

                    Location location = Location.Create(
                        identifier.SyntaxTree,
                        TextSpan.FromBounds(identifier.Span.End - AsyncSuffix.Length, identifier.Span.End));

                    context.ReportDiagnostic(
                        DiagnosticDescriptors.NonAsyncMethodShouldNotHaveAsyncSuffixFadeOut,
                        location);
                }
            }
        }
    }
}
