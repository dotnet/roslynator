// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;

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
                    DiagnosticDescriptors.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious,
                    DiagnosticDescriptors.UseExplicitTypeInsteadOfVarWhenTypeIsObvious,
                    DiagnosticDescriptors.UseVarInsteadOfExplicitTypeWhenTypeIsObvious);
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

            TypeAnalysisFlags flags = CSharpAnalysis.AnalyzeType(variableDeclaration, context.SemanticModel, context.CancellationToken);

            if (flags.IsExplicit())
            {
                if (flags.SupportsImplicit()
                    && flags.IsTypeObvious())
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseVarInsteadOfExplicitTypeWhenTypeIsObvious,
                        variableDeclaration.Type);
                }
            }
            else if (flags.SupportsExplicit())
            {
                if (flags.IsTypeObvious())
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseExplicitTypeInsteadOfVarWhenTypeIsObvious,
                        variableDeclaration.Type);
                }
                else if (flags.IsValidSymbol())
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious,
                        variableDeclaration.Type);
                }
            }
        }
    }
}
