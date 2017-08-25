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
    public class NamespaceDeclarationDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveEmptyNamespaceDeclaration,
                    DiagnosticDescriptors.DeclareUsingDirectiveOnTopLevel);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzerNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
        }

        private void AnalyzerNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (NamespaceDeclarationSyntax)context.Node;

            RemoveEmptyNamespaceDeclarationRefactoring.Analyze(context, declaration);

            DeclareUsingDirectiveOnTopLevelRefactoring.Analyze(context, declaration);
        }
    }
}
