// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;
using Roslynator.Extensions;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VariableDeclarationDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UseExplicitTypeInsteadOfVar,
                    DiagnosticDescriptors.UseExplicitTypeInsteadOfVarEvenIfObvious,
                    DiagnosticDescriptors.UseVarInsteadOfExplicitType);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeVariableDeclaration(f), SyntaxKind.VariableDeclaration);
        }

        private void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
        {
            var variableDeclaration = (VariableDeclarationSyntax)context.Node;

            switch (CSharpAnalysis.AnalyzeType(variableDeclaration, context.SemanticModel, context.CancellationToken))
            {
                case TypeAnalysisResult.Explicit:
                    {
                        break;
                    }
                case TypeAnalysisResult.ExplicitButShouldBeImplicit:
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.UseVarInsteadOfExplicitType,
                            variableDeclaration.Type);

                        break;
                    }
                case TypeAnalysisResult.Implicit:
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.UseExplicitTypeInsteadOfVarEvenIfObvious,
                            variableDeclaration.Type);

                        break;
                    }
                case TypeAnalysisResult.ImplicitButShouldBeExplicit:
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.UseExplicitTypeInsteadOfVar,
                            variableDeclaration.Type);

                        break;
                    }
            }
        }
    }
}
