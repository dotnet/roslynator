// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SplitDeclarationIntoMultipleDeclarationsDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.SplitDeclarationIntoMultipleDeclarations);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeLocalDeclarationStatement(f), SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeFieldDeclaration(f), SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEventFieldDeclaration(f), SyntaxKind.EventFieldDeclaration);
        }

        private void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (LocalDeclarationStatementSyntax)context.Node;

            if (declaration.Declaration != null)
                AnalyzeVariableDeclaration(context, declaration.Declaration);
        }

        private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (FieldDeclarationSyntax)context.Node;

            if (declaration.Declaration != null)
                AnalyzeVariableDeclaration(context, declaration.Declaration);
        }

        private void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (EventFieldDeclarationSyntax)context.Node;

            if (declaration.Declaration != null)
                AnalyzeVariableDeclaration(context, declaration.Declaration);
        }

        private static void AnalyzeVariableDeclaration(
            SyntaxNodeAnalysisContext context,
            VariableDeclarationSyntax variableDeclaration)
        {
            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;

            if (variables.Count > 1)
            {
                TextSpan span = TextSpan.FromBounds(
                    variables[1].Span.Start,
                    variables.Last().Span.End);

                if (context.Node
                    .DescendantTrivia(span)
                    .All(f => f.IsWhitespaceOrEndOfLine()))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.SplitDeclarationIntoMultipleDeclarations,
                        Location.Create(context.Node.SyntaxTree, span));
                }
            }
        }
    }
}
