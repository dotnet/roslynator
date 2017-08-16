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
    public class ReduceIfNestingDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ReduceIfNesting); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol taskType = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task);

                startContext.RegisterSyntaxNodeAction(f => AnalyzeIfStatement(f, taskType), SyntaxKind.IfStatement);
            });
        }

        private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskType)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            if (ReduceIfNestingRefactoring.IsFixable(ifStatement, context.SemanticModel, taskType, context.CancellationToken, topLevelOnly: true))
                context.ReportDiagnostic(DiagnosticDescriptors.ReduceIfNesting, ifStatement.IfKeyword);
        }
    }
}
