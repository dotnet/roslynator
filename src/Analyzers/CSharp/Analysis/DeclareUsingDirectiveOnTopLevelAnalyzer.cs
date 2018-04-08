// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DeclareUsingDirectiveOnTopLevelAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.DeclareUsingDirectiveOnTopLevel); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
        }

        public static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            SyntaxList<UsingDirectiveSyntax> usings = namespaceDeclaration.Usings;

            if (!usings.Any())
                return;

            int count = usings.Count;

            for (int i = 0; i < count; i++)
            {
                if (usings[i].ContainsDiagnostics)
                    return;

                if (i == 0)
                {
                    if (usings[i].SpanOrTrailingTriviaContainsDirectives())
                        return;
                }
                else if (i == count - 1)
                {
                    if (usings[i].SpanOrLeadingTriviaContainsDirectives())
                        return;
                }
                else if (usings[i].ContainsDirectives)
                {
                    return;
                }
            }

            context.ReportDiagnostic(
                DiagnosticDescriptors.DeclareUsingDirectiveOnTopLevel,
                Location.Create(namespaceDeclaration.SyntaxTree, usings.Span));
        }
    }
}
