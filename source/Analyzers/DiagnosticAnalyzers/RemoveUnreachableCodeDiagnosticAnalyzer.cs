// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Diagnostics.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveUnreachableCodeDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveUnreachableCode); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeBreakStatement(f), SyntaxKind.BreakStatement);
        }

        private void AnalyzeBreakStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var breakStatement = (BreakStatementSyntax)context.Node;

            SyntaxNode node = breakStatement.Parent;

            while (node?.IsKind(SyntaxKind.Block) == true)
                node = node.Parent;

            if (node?.IsKind(SyntaxKind.SwitchSection) == true
                && context.SemanticModel.ContainsCompilerDiagnostic(CSharpErrorCodes.UnreachableCodeDetected, breakStatement.Span, context.CancellationToken))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.RemoveUnreachableCode, breakStatement);
            }
        }
    }
}
