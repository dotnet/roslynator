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
    public class ForEachStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseExplicitTypeInsteadOfVarInForEach); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(f => AnalyzeForEachStatement(f), SyntaxKind.ForEachStatement);
        }

        private void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var forEachStatement = (ForEachStatementSyntax)context.Node;

            TypeAnalysisResult result = CSharpAnalysis.AnalyzeType(forEachStatement, context.SemanticModel, context.CancellationToken);

            if (result == TypeAnalysisResult.ImplicitButShouldBeExplicit)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseExplicitTypeInsteadOfVarInForEach,
                    forEachStatement.Type.GetLocation());
            }
        }
    }
}
