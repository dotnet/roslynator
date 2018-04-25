// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests.CSharp
{
    public static class CSharpCompilerCodeFixVerifier
    {
        public static void VerifyFix(
            string sourceTemplate,
            string fixableCode,
            string fixedCode,
            string diagnosticId,
            CodeFixProvider fixProvider,
            string equivalenceKey = null)
        {
            (string source, string newSource, TextSpan span) = TextUtility.GetMarkedSpan(sourceTemplate, fixableCode, fixedCode);

            VerifyFix(
                source: source,
                newSource: newSource,
                diagnosticId: diagnosticId,
                fixProvider: fixProvider,
                equivalenceKey: equivalenceKey);
        }

        public static void VerifyFix(
            string source,
            string newSource,
            string diagnosticId,
            CodeFixProvider fixProvider,
            string equivalenceKey = null)
        {
            CompilerCodeFixVerifier.VerifyFix(
                source: source,
                newSource: newSource,
                diagnosticId: diagnosticId,
                fixProvider: fixProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey);
        }

        public static void VerifyNoFix(
            string source,
            CodeFixProvider fixProvider,
            string equivalenceKey = null)
        {
            CompilerCodeFixVerifier.VerifyNoFix(
                source: source,
                fixProvider: fixProvider,
                language: LanguageNames.CSharp,
                equivalenceKey: equivalenceKey);
        }
    }
}
