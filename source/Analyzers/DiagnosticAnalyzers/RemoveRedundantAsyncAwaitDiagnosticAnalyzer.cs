// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantAsyncAwaitDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveRedundantAsyncAwait,
                    DiagnosticDescriptors.RemoveRedundantAsyncAwaitFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeMethodDeclaration(f), SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeLambdaExpression(f), SyntaxKind.SimpleLambdaExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeLambdaExpression(f), SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAnonymousMethodExpression(f), SyntaxKind.AnonymousMethodExpression);
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            RemoveRedundantAsyncAwaitRefactoring.AnalyzeMethodDeclaration(context);
        }

        private void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            RemoveRedundantAsyncAwaitRefactoring.AnalyzeLambdaExpression(context);
        }

        private void AnalyzeAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            RemoveRedundantAsyncAwaitRefactoring.AnalyzeAnonymousMethodExpression(context);
        }
    }
}
