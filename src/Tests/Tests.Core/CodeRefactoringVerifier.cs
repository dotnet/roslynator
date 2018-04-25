// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Roslynator.Tests
{
    public static class CodeRefactoringVerifier
    {
        public static void VerifyRefactoring(
            string source,
            string newSource,
            IEnumerable<TextSpan> spans,
            CodeRefactoringProvider refactoringProvider,
            string language,
            string equivalenceKey = null,
            bool allowNewCompilerDiagnostics = false)
        {
            Document document = WorkspaceFactory.CreateDocument(source, language);

            foreach (TextSpan span in spans.OrderByDescending(f => f.Start))
            {
                document = VerifyRefactoring(
                    document: document,
                    span: span,
                    refactoringProvider: refactoringProvider,
                    equivalenceKey: equivalenceKey,
                    allowNewCompilerDiagnostics: allowNewCompilerDiagnostics);
            }

            string actual = document.ToSimplifiedAndFormattedFullString();

            Assert.Equal(newSource, actual);
        }

        public static void VerifyRefactoring(
            string source,
            string newSource,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string language,
            string equivalenceKey = null,
            bool allowNewCompilerDiagnostics = false)
        {
            Document document = WorkspaceFactory.CreateDocument(source, language);

            document = VerifyRefactoring(
                document: document,
                span: span,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                allowNewCompilerDiagnostics: allowNewCompilerDiagnostics);

            string actual = document.ToSimplifiedAndFormattedFullString();

            Assert.Equal(newSource, actual);
        }

        private static Document VerifyRefactoring(
            Document document,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey,
            bool allowNewCompilerDiagnostics)
        {
            ImmutableArray<Diagnostic> compilerDiagnostics = document.GetCompilerDiagnostics();

            DiagnosticVerifier.VerifyNoCompilerError(compilerDiagnostics);

            List<CodeAction> actions = null;

            var context = new CodeRefactoringContext(
                document,
                span,
                a =>
                {
                    if (equivalenceKey == null
                        || string.Equals(a.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                    {
                        (actions ?? (actions = new List<CodeAction>())).Add(a);
                    }
                },
                CancellationToken.None);

            refactoringProvider.ComputeRefactoringsAsync(context).Wait();

            Assert.True(actions != null, "No code refactoring has been registered.");

            document = document.ApplyCodeAction(actions[0]);

            if (!allowNewCompilerDiagnostics)
                DiagnosticVerifier.VerifyNoNewCompilerDiagnostics(document, compilerDiagnostics);

            return document;
        }

        public static void VerifyNoRefactoring(
            string source,
            IEnumerable<TextSpan> spans,
            CodeRefactoringProvider refactoringProvider,
            string language,
            string equivalenceKey = null)
        {
            foreach (TextSpan span in spans)
            {
                VerifyNoRefactoring(
                    source: source,
                    span: span,
                    refactoringProvider: refactoringProvider,
                    language: language,
                    equivalenceKey: equivalenceKey);
            }
        }

        public static void VerifyNoRefactoring(
            string source,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string language,
            string equivalenceKey = null)
        {
            Document document = WorkspaceFactory.CreateDocument(source, language);

            DiagnosticVerifier.VerifyNoCompilerError(document);

            List<CodeAction> actions = null;

            var context = new CodeRefactoringContext(
                document,
                span,
                codeAction =>
                {
                    if (equivalenceKey == null
                        || string.Equals(codeAction.EquivalenceKey, equivalenceKey, StringComparison.Ordinal))
                    {
                        (actions ?? (actions = new List<CodeAction>())).Add(codeAction);
                    }
                },
                CancellationToken.None);

            refactoringProvider.ComputeRefactoringsAsync(context).Wait();

            Assert.True(actions == null, $"Expected no code refactoring, actual: {actions?.Count ?? 0}");
        }
    }
}
