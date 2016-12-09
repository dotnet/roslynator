// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CompilationUnitDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.DeclareEachTypeInSeparateFile,
                    DiagnosticDescriptors.RemoveFileWithNoCode);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeCompilationUnit(f), SyntaxKind.CompilationUnit);
        }

        private void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var compilationUnit = (CompilationUnitSyntax)context.Node;

            DeclareEachTypeInSeparateFileRefactoring.Analyze(context, compilationUnit);

            if (compilationUnit.Span == compilationUnit.EndOfFileToken.Span)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveFileWithNoCode,
                    Location.Create(compilationUnit.SyntaxTree, default(TextSpan)));
            }
        }
    }
}
