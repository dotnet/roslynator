// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsynchronousMethodNameShouldEndWithAsyncAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AsynchronousMethodNameShouldEndWithAsync); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (!methodDeclaration.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                return;

            if (methodDeclaration.Identifier.ValueText.EndsWith("Async", StringComparison.Ordinal))
                return;

            IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol?.IsAsync != true)
                return;

            if (methodSymbol.Name.EndsWith("Async", StringComparison.Ordinal))
                return;

            if (methodDeclaration.Body?.Statements.Any() == false)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AsynchronousMethodNameShouldEndWithAsync, methodDeclaration.Identifier);
        }
    }
}
