// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FormatDeclarationBracesDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.FormatDeclarationBraces);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeClassDeclaration(f), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeStructDeclaration(f), SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeInterfaceDeclaration(f), SyntaxKind.InterfaceDeclaration);
        }

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (ClassDeclarationSyntax)context.Node;

            if (declaration.Members.Count == 0
                && ShouldBeFormatted(declaration.OpenBraceToken, declaration.CloseBraceToken))
            {
                TextSpan span = TextSpan.FromBounds(
                    declaration.OpenBraceToken.Span.Start,
                    declaration.CloseBraceToken.Span.End);

                context.ReportDiagnostic(
                    DiagnosticDescriptors.FormatDeclarationBraces,
                    Location.Create(context.Node.SyntaxTree, span));
            }
        }

        private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (StructDeclarationSyntax)context.Node;

            if (declaration.Members.Count == 0
                && ShouldBeFormatted(declaration.OpenBraceToken, declaration.CloseBraceToken))
            {
                TextSpan span = TextSpan.FromBounds(
                    declaration.OpenBraceToken.Span.Start,
                    declaration.CloseBraceToken.Span.End);

                context.ReportDiagnostic(
                    DiagnosticDescriptors.FormatDeclarationBraces,
                    Location.Create(context.Node.SyntaxTree, span));
            }
        }

        private void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (InterfaceDeclarationSyntax)context.Node;

            if (declaration.Members.Count == 0
                && ShouldBeFormatted(declaration.OpenBraceToken, declaration.CloseBraceToken))
            {
                TextSpan span = TextSpan.FromBounds(
                    declaration.OpenBraceToken.Span.Start,
                    declaration.CloseBraceToken.Span.End);

                context.ReportDiagnostic(
                    DiagnosticDescriptors.FormatDeclarationBraces,
                    Location.Create(context.Node.SyntaxTree, span));
            }
        }

        private static bool ShouldBeFormatted(SyntaxToken openBrace, SyntaxToken closeBrace)
        {
            return !openBrace.IsMissing
                && !closeBrace.IsMissing
                && closeBrace.GetSpanStartLine() - openBrace.GetSpanEndLine() != 1
                && openBrace.TrailingTrivia.IsWhitespaceOrEndOfLine()
                && closeBrace.LeadingTrivia.IsWhitespaceOrEndOfLine();
        }
    }
}
