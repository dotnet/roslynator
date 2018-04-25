// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.Tests.CSharp
{
    public static class CSharpCodeFixVerifier
    {
        public static void VerifyFix(
            string source,
            string newSource,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider,
            bool allowNewCompilerDiagnostics = false)
        {
            CodeFixVerifier.VerifyFix(
                source: source,
                newSource: newSource,
                analyzer: analyzer,
                fixProvider: fixProvider,
                language: LanguageNames.CSharp,
                allowNewCompilerDiagnostics: allowNewCompilerDiagnostics);
        }

        public static void VerifyNoFix(
            string source,
            DiagnosticAnalyzer analyzer,
            CodeFixProvider fixProvider)
        {
            CodeFixVerifier.VerifyNoFix(
                source: source,
                analyzer: analyzer,
                fixProvider: fixProvider,
                language: LanguageNames.CSharp);
        }
    }
}
