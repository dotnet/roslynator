// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Roslynator.Tests
{
    public static class CodeFixVerifier
    {
        public static void VerifyFix(
            string source,
            string newSource,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            string language,
            bool allowNewCompilerDiagnostics = false)
        {
            Assert.True(fixProvider.CanFixAny(analyzer.SupportedDiagnostics), $"Code fix provider '{fixProvider.GetType().Name}' cannot fix any diagnostic supported by analyzer '{analyzer}'.");

            Document document = WorkspaceFactory.CreateDocument(source, language);

            ImmutableArray<Diagnostic> compilerDiagnostics = document.GetCompilerDiagnostics();

            DiagnosticVerifier.VerifyNoCompilerError(compilerDiagnostics);

            Diagnostic[] analyzerDiagnostics = DiagnosticUtility.GetSortedDiagnostics(document, analyzer);

            while (analyzerDiagnostics.Length > 0)
            {
                Diagnostic diagnostic = null;

                foreach (Diagnostic analyzerDiagnostic in analyzerDiagnostics)
                {
                    if (fixProvider.FixableDiagnosticIds.Contains(analyzerDiagnostic.Id))
                    {
                        diagnostic = analyzerDiagnostic;
                        break;
                    }
                }

                if (diagnostic == null)
                    break;

                List<CodeAction> actions = null;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, _) => (actions ?? (actions = new List<CodeAction>())).Add(a),
                    CancellationToken.None);

                fixProvider.RegisterCodeFixesAsync(context).Wait();

                if (actions == null)
                    break;

                document = document.ApplyCodeAction(actions[0]);

                if (!allowNewCompilerDiagnostics)
                    DiagnosticVerifier.VerifyNoNewCompilerDiagnostics(document, compilerDiagnostics);

                analyzerDiagnostics = DiagnosticUtility.GetSortedDiagnostics(document, analyzer);
            }

            string actual = document.ToSimplifiedAndFormattedFullString();

            Assert.Equal(newSource, actual);
        }

        public static void VerifyNoFix(
            string source,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            string language)
        {
            Document document = WorkspaceFactory.CreateDocument(source, language);

            DiagnosticVerifier.VerifyNoCompilerError(document);

            foreach (Diagnostic diagnostic in DiagnosticUtility.GetSortedDiagnostics(document, analyzer))
            {
                List<CodeAction> actions = null;

                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (a, _) => (actions ?? (actions = new List<CodeAction>())).Add(a),
                    CancellationToken.None);

                fixProvider.RegisterCodeFixesAsync(context).Wait();

                Assert.True(actions == null, $"Expected no code fix, actual: {actions.Count}.");
            }
        }
    }
}
