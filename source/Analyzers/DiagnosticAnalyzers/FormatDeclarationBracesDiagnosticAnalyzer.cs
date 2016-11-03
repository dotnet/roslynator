// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FormatDeclarationBracesDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.FormatDeclarationBraces); }
        }

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

            if (!declaration.Members.Any())
                AnalyzerBraces(context, declaration, declaration.OpenBraceToken, declaration.CloseBraceToken);
        }

        private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (StructDeclarationSyntax)context.Node;

            if (!declaration.Members.Any())
                AnalyzerBraces(context, declaration, declaration.OpenBraceToken, declaration.CloseBraceToken);
        }

        private void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (InterfaceDeclarationSyntax)context.Node;

            if (!declaration.Members.Any())
                AnalyzerBraces(context, declaration, declaration.OpenBraceToken, declaration.CloseBraceToken);
        }

        private static void AnalyzerBraces(
            SyntaxNodeAnalysisContext context,
            MemberDeclarationSyntax declaration,
            SyntaxToken openBrace,
            SyntaxToken closeBrace)
        {
            if (!openBrace.IsMissing
                && !closeBrace.IsMissing
                && closeBrace.GetSpanStartLine() - openBrace.GetSpanEndLine() != 1)
            {
                TextSpan span = TextSpan.FromBounds(openBrace.Span.Start, closeBrace.Span.End);

                if (declaration
                    .DescendantTrivia(span)
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.FormatDeclarationBraces,
                        Location.Create(declaration.SyntaxTree, span));
                }
            }
        }
    }
}
