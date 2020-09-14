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
    public class AwaitExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.CallConfigureAwait); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (startContext.Compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1") == null)
                    return;

                startContext.RegisterSyntaxNodeAction(f => AnalyzeAwaitExpression(f), SyntaxKind.AwaitExpression);
            });
        }

        private static void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
        {
            var awaitExpression = (AwaitExpressionSyntax)context.Node;

            if (CallConfigureAwaitAnalysis.IsFixable(awaitExpression, context.SemanticModel, context.CancellationToken))
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.CallConfigureAwait, awaitExpression.Expression);
            }
        }
    }
}
