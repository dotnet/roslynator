// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Internal.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClassDeclarationDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddDiagnosticAnalyzerSuffix,
                    DiagnosticDescriptors.AddCodeFixProviderSuffix,
                    DiagnosticDescriptors.AddCodeRefactoringProviderSuffix);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(f => AnalyzeClassDeclaration(f), SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            INamedTypeSymbol symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);

            INamedTypeSymbol diagnosticAnalyzerSymbol = context.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer");

            if (diagnosticAnalyzerSymbol != null
                && symbol.BaseTypesAndSelf().Contains(diagnosticAnalyzerSymbol)
                && !symbol.Name.EndsWith(diagnosticAnalyzerSymbol.Name, StringComparison.Ordinal))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddDiagnosticAnalyzerSuffix,
                    classDeclaration.Identifier);
            }

            INamedTypeSymbol codeFixProviderSymbol = context.GetTypeByMetadataName("Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider");

            if (codeFixProviderSymbol != null
                && symbol.BaseTypesAndSelf().Contains(codeFixProviderSymbol)
                && !symbol.Name.EndsWith(codeFixProviderSymbol.Name, StringComparison.Ordinal))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddCodeFixProviderSuffix,
                    classDeclaration.Identifier);
            }

            INamedTypeSymbol codeRefactoringProviderSymbol = context.GetTypeByMetadataName("Microsoft.CodeAnalysis.CodeRefactorings.CodeRefactoringProvider");

            if (codeRefactoringProviderSymbol != null
                && symbol.BaseTypesAndSelf().Contains(codeRefactoringProviderSymbol)
                && !symbol.Name.EndsWith(codeRefactoringProviderSymbol.Name, StringComparison.Ordinal))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddCodeRefactoringProviderSuffix,
                    classDeclaration.Identifier);
            }
        }
    }
}
