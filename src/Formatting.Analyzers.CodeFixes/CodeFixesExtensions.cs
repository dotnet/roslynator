// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Formatting
{
    internal static class CodeFixesExtensions
    {
        public static bool IsAnalyzerOptionEnabled(this Document document, DiagnosticDescriptor analyzerOption)
        {
            return document.Project.CompilationOptions.IsAnalyzerOptionEnabled(analyzerOption);
        }
    }
}
