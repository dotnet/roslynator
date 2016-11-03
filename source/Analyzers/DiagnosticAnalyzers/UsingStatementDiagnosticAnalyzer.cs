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
    public class UsingStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.SimplifyNestedUsingStatement,
                    DiagnosticDescriptors.SimplifyNestedUsingStatementFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.UsingStatement);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var usingStatement = (UsingStatementSyntax)context.Node;

            if (RemoveBracesFromUsingStatementRefactoring.CanRefactor(usingStatement))
            {
                var block = (BlockSyntax)usingStatement.Statement;

                context.ReportDiagnostic(
                    DiagnosticDescriptors.SimplifyNestedUsingStatement,
                    block.GetLocation());

                context.FadeOutBraces(DiagnosticDescriptors.SimplifyNestedUsingStatementFadeOut, block);
            }
        }
    }
}
