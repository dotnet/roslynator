// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveFileWithNoCodeAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveFileWithNoCode); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeCompilationUnit, SyntaxKind.CompilationUnit);
        }

        private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            SyntaxToken token = compilationUnit.EndOfFileToken;

            if (compilationUnit.Span == token.Span
                && !token.HasTrailingTrivia
                && token.LeadingTrivia.All(f => !f.IsDirective))
            {
                SyntaxTree syntaxTree = compilationUnit.SyntaxTree;

                Debug.Assert(!GeneratedCodeUtility.IsGeneratedCodeFile(syntaxTree.FilePath), syntaxTree.FilePath);

                if (!GeneratedCodeUtility.IsGeneratedCodeFile(syntaxTree.FilePath))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveFileWithNoCode,
                        Location.Create(syntaxTree, default(TextSpan)));
                }
            }
        }
    }
}
