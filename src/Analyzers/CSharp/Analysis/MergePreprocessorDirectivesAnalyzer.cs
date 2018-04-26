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
    public class MergePreprocessorDirectivesAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.MergePreprocessorDirectives); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzePragmaWarningDirectiveTrivia, SyntaxKind.PragmaWarningDirectiveTrivia);
        }

        public static void AnalyzePragmaWarningDirectiveTrivia(SyntaxNodeAnalysisContext context)
        {
            var directive = (PragmaWarningDirectiveTriviaSyntax)context.Node;

            SyntaxTrivia trivia = directive.ParentTrivia;

            if (!trivia.TryGetContainingList(out SyntaxTriviaList list))
                return;

            int count = list.Count;

            if (count == 1)
                return;

            int index = list.IndexOf(trivia);

            if (index == count - 1)
                return;

            int i = index - 1;

            while (i >= 0
                && list[i].IsWhitespaceOrEndOfLineTrivia())
            {
                i--;
            }

            if (i >= 0
                && list[i].IsKind(SyntaxKind.PragmaWarningDirectiveTrivia))
            {
                return;
            }

            i = index + 1;

            while (i < count
                && list[i].IsWhitespaceOrEndOfLineTrivia())
            {
                i++;
            }

            if (i == count)
                return;

            if (!list[i].IsKind(SyntaxKind.PragmaWarningDirectiveTrivia))
                return;

            if (!(list[i].GetStructure() is PragmaWarningDirectiveTriviaSyntax nextDirective))
                return;

            if (directive.DisableOrRestoreKeyword.Kind() != nextDirective.DisableOrRestoreKeyword.Kind())
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.MergePreprocessorDirectives, directive);
        }
    }
}
