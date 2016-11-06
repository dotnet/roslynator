// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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

            context.RegisterSyntaxNodeAction(f => AnalyzerNamespaceDeclaration(f), SyntaxKind.NamespaceDeclaration);
        }

        private void AnalyzerNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (NamespaceDeclarationSyntax)context.Node;

            if (declaration.Members.Count == 0
                && !declaration.OpenBraceToken.IsMissing
                && !declaration.CloseBraceToken.IsMissing
                && declaration.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && declaration.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveEmptyNamespaceDeclaration,
                    declaration.GetLocation());
            }

            SyntaxList<UsingDirectiveSyntax> usings = declaration.Usings;

            if (usings.Any())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.DeclareUsingDirectiveOnTopLevel,
                    Location.Create(declaration.SyntaxTree, usings.Span));
            }
        }
    }
}
