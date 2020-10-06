// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.Formatting
{
    internal static class FormattingExtensions
    {
        public static bool IsAnalyzerOptionEnabled(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor analyzerOption)
        {
            return IsAnalyzerOptionEnabled(context.Compilation.Options, analyzerOption);
        }

        public static bool IsAnalyzerOptionEnabled(this CompilationOptions compilationOptions, DiagnosticDescriptor analyzerOption)
        {
            return CSharp.AnalyzerOptions.IsEnabled(compilationOptions, analyzerOption);
        }
    }
}
