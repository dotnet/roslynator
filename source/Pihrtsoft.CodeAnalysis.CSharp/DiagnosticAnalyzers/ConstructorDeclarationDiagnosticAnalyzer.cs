// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConstructorDeclarationDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantBaseConstructorCall);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeConstructorDeclaration(f), SyntaxKind.ConstructorDeclaration);
        }

        private void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var constructor = (ConstructorDeclarationSyntax)context.Node;

            if (constructor.Initializer?.IsKind(SyntaxKind.BaseConstructorInitializer) == true)
            {
                ConstructorInitializerSyntax initializer = constructor.Initializer;

                if (initializer.ArgumentList?.Arguments.Count == 0
                    && initializer
                        .DescendantTrivia(initializer.Span)
                        .All(f => f.IsWhitespaceOrEndOfLine()))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveRedundantBaseConstructorCall,
                        initializer.GetLocation());
                }
            }
        }
    }
}
