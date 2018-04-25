// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.Tests.CSharp.CSharpCodeFixVerifier;

namespace Roslynator.Tests.CSharp
{
    public static class CSharpDiagnosticVerifier
    {
        public static void VerifyDiagnosticAndFix(
            string source,
            string newSource,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            bool allowNewCompilerDiagnostics = false)
        {
            (string source2, List<Diagnostic> diagnostics) = TextUtility.GetMarkedDiagnostics(source, descriptor, FileUtility.DefaultCSharpFileName);

            VerifyDiagnostic(source2, analyzer, diagnostics.ToArray());

            VerifyFix(source2, newSource, analyzer, fixProvider, allowNewCompilerDiagnostics);
        }

        public static void VerifyDiagnosticAndFix(
            string source,
            string newSource,
            TextSpan span,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            bool allowNewCompilerDiagnostics = false)
        {
            VerifyDiagnostic(source, span, analyzer, descriptor);

            VerifyFix(source, newSource, analyzer, fixProvider, allowNewCompilerDiagnostics);
        }

        public static void VerifyDiagnosticAndFix(
            string source,
            string fixableCode,
            string fixedCode,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            bool allowNewCompilerDiagnostics = false)
        {
            (string source2, string newSource, TextSpan span) = TextUtility.GetMarkedSpan(source, fixableCode, fixedCode);

            (string source3, List<TextSpan> spans) = TextUtility.GetMarkedSpans(source2);

            if (spans != null)
            {
                source2 = source3;
                span = spans[0];
            }

            VerifyDiagnostic(source2, span, analyzer, descriptor);

            VerifyFix(source2, newSource, analyzer, fixProvider, allowNewCompilerDiagnostics);
        }

        public static void VerifyDiagnostic(
            string source,
            TextSpan span,
            DiagnosticAnalyzer analyzer,
            DiagnosticDescriptor descriptor)
        {
            Location location = Location.Create(FileUtility.DefaultCSharpFileName, span, span.ToLinePositionSpan(source));

            Diagnostic diagnostic = Diagnostic.Create(descriptor, location);

            VerifyDiagnostic(source, analyzer, diagnostic);
        }

        public static void VerifyDiagnostic(
            string source,
            DiagnosticAnalyzer analyzer,
            params Diagnostic[] expectedDiagnostics)
        {
            VerifyDiagnostic(new string[] { source }, analyzer, expectedDiagnostics);
        }

        public static void VerifyDiagnostic(
            IEnumerable<string> sources,
            DiagnosticAnalyzer analyzer,
            params Diagnostic[] expectedDiagnostics)
        {
            DiagnosticVerifier.VerifyDiagnostic(sources, analyzer, LanguageNames.CSharp, expectedDiagnostics);
        }

        public static void VerifyNoDiagnostic(
            string source,
            string fixableCode,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer)
        {
            (string source2, TextSpan span) = TextUtility.GetMarkedSpan(source, fixableCode);

            VerifyNoDiagnostic(fixableCode, descriptor, analyzer);
        }

        public static void VerifyNoDiagnostic(
            string source,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer)
        {
            VerifyNoDiagnostic(new string[] { source }, descriptor, analyzer);
        }

        public static void VerifyNoDiagnostic(
            IEnumerable<string> sources,
            DiagnosticDescriptor descriptor,
            DiagnosticAnalyzer analyzer)
        {
            DiagnosticVerifier.VerifyNoDiagnostic(sources, descriptor, analyzer, LanguageNames.CSharp);
        }
    }
}
