// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseConstantInsteadOfFieldDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseConstantInsteadOfField); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        }

        private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            if (context.Node.SpanContainsDirectives())
                return;

            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            if (!UseConstantInsteadOfFieldRefactoring.CanRefactor(fieldDeclaration, context.SemanticModel, onlyPrivate: true, cancellationToken: context.CancellationToken))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UseConstantInsteadOfField, fieldDeclaration);
        }
    }
}
