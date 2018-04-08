// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantBaseConstructorCallAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantBaseConstructorCall); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
        }

        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructor = (ConstructorDeclarationSyntax)context.Node;

            ConstructorInitializerSyntax initializer = constructor.Initializer;

            if (initializer?.Kind() == SyntaxKind.BaseConstructorInitializer
                && initializer.ArgumentList?.Arguments.Count == 0
                && initializer
                    .DescendantTrivia(initializer.Span)
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveRedundantBaseConstructorCall,
                    initializer);
            }
        }
    }
}
