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
    public class DeclarationExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
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

            context.RegisterSyntaxNodeAction(f => AnalyzeDeclarationExpression(f), SyntaxKind.DeclarationExpression);
        }

        private void AnalyzeDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = (DeclarationExpressionSyntax)context.Node;

            TypeAnalysisFlags flags = CSharpAnalysis.AnalyzeType(declarationExpression, context.SemanticModel, context.CancellationToken);

            if (flags.IsExplicit())
            {
                if (flags.SupportsImplicit()
                    && flags.IsTypeObvious())
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseVarInsteadOfExplicitTypeWhenTypeIsObvious,
                        declarationExpression.Type);
                }
            }
            else if (flags.SupportsExplicit())
            {
                if (flags.IsTypeObvious())
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseExplicitTypeInsteadOfVarWhenTypeIsObvious,
                        declarationExpression.Type);
                }
                else if (flags.IsValidSymbol())
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious,
                        declarationExpression.Type);
                }
            }
        }
    }
}
