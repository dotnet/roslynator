// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class EmptyLineBetweenUsingDirectiveAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveEmptyLineBetweenUsingDirectivesWithSameRootNamespace,
                    DiagnosticDescriptors.RemoveEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace,
                    DiagnosticDescriptors.AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeCompilationUnit, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
        }

        private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            AnalyzeUsings(context, compilationUnit.Usings);
        }

        private static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            AnalyzeUsings(context, namespaceDeclaration.Usings);
        }

        private static void AnalyzeUsings(SyntaxNodeAnalysisContext context, SyntaxList<UsingDirectiveSyntax> usings)
        {
            int count = usings.Count;

            if (count <= 1)
                return;

            UsingDirectiveSyntax usingDirective1 = usings[0];

            for (int i = 1; i < count; i++, usingDirective1 = usings[i - 1])
            {
                if (usingDirective1.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                    return;

                if (usingDirective1.Alias != null)
                    return;

                UsingDirectiveSyntax usingDirective2 = usings[i];

                if (usingDirective2.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                    return;

                if (usingDirective2.Alias != null)
                    return;

                SyntaxTriviaList trailingTrivia = usingDirective1.GetTrailingTrivia();

                if (!SyntaxTriviaAnalysis.IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(trailingTrivia))
                    continue;

                IdentifierNameSyntax rootNamespace1 = usingDirective1.GetRootNamespace();

                if (rootNamespace1 == null)
                    continue;

                IdentifierNameSyntax rootNamespace2 = usingDirective2.GetRootNamespace();

                if (rootNamespace2 == null)
                    continue;

                SyntaxTriviaList leadingTrivia = usingDirective2.GetLeadingTrivia();

                bool isEmptyLine = SyntaxTriviaAnalysis.StartsWithOptionalWhitespaceThenEndOfLineTrivia(leadingTrivia);

                if (string.Equals(rootNamespace1.Identifier.ValueText, rootNamespace2.Identifier.ValueText, StringComparison.Ordinal))
                {
                    if (isEmptyLine)
                    {
                        ReportDiagnostic(
                            context,
                            DiagnosticDescriptors.RemoveEmptyLineBetweenUsingDirectivesWithSameRootNamespace,
                            leadingTrivia[0]);
                    }
                }
                else if (isEmptyLine)
                {
                    ReportDiagnostic(
                        context,
                        DiagnosticDescriptors.RemoveEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace,
                        leadingTrivia[0]);
                }
                else
                {
                    ReportDiagnostic(
                        context,
                        DiagnosticDescriptors.AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace,
                        trailingTrivia.Last());
                }
            }
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxTrivia trivia)
        {
            if (!context.IsAnalyzerSuppressed(descriptor))
                context.ReportDiagnostic(descriptor, Location.Create(context.Node.SyntaxTree, trivia.Span.WithLength(0)));
        }
    }
}
