// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests.CSharp
{
    public static class CSharpCodeRefactoringVerifier
    {
        public static void VerifyRefactoring(
            string source,
            string newSource,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            bool allowNewCompilerDiagnostics = false)
        {
            (string source2, List<TextSpan> spans) = TextUtility.GetMarkedSpans(source);

            VerifyRefactoring(
                source: source2,
                newSource: newSource,
                spans: spans,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                allowNewCompilerDiagnostics: allowNewCompilerDiagnostics);
        }

        public static void VerifyRefactoring(
            string sourceTemplate,
            string fixableCode,
            string fixedCode,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            bool allowNewCompilerDiagnostics = false)
        {
            (string source, string newSource, TextSpan span) = TextUtility.GetMarkedSpan(sourceTemplate, fixableCode, fixedCode);

            (string source2, List<TextSpan> spans) = TextUtility.GetMarkedSpans(source);

            if (spans != null)
            {
                source = source2;
                span = spans[0];
            }

            VerifyRefactoring(
                source: source,
                newSource: newSource,
                span: span,
                refactoringProvider: refactoringProvider,
                equivalenceKey: equivalenceKey,
                allowNewCompilerDiagnostics: allowNewCompilerDiagnostics);
        }

        public static void VerifyRefactoring(
            string source,
            string newSource,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            bool allowNewCompilerDiagnostics = false)
        {
            CodeRefactoringVerifier.VerifyRefactoring(
                source: source,
                newSource: newSource,
                span: span,
                refactoringProvider: refactoringProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey,
                allowNewCompilerDiagnostics: allowNewCompilerDiagnostics);
        }

        public static void VerifyRefactoring(
            string source,
            string newSource,
            IEnumerable<TextSpan> spans,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null,
            bool allowNewCompilerDiagnostics = false)
        {
            CodeRefactoringVerifier.VerifyRefactoring(
                source: source,
                newSource: newSource,
                spans: spans,
                refactoringProvider: refactoringProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey,
                allowNewCompilerDiagnostics: allowNewCompilerDiagnostics);
        }

        public static void VerifyNoRefactoring(
            string source,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null)
        {
            (string source2, List<TextSpan> spans) = TextUtility.GetMarkedSpans(source);

            CodeRefactoringVerifier.VerifyNoRefactoring(
                source: source2,
                spans: spans,
                refactoringProvider: refactoringProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey);
        }

        public static void VerifyNoRefactoring(
            string source,
            TextSpan span,
            CodeRefactoringProvider refactoringProvider,
            string equivalenceKey = null)
        {
            CodeRefactoringVerifier.VerifyNoRefactoring(
                source: source,
                span: span,
                refactoringProvider: refactoringProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey);
        }
    }
}
