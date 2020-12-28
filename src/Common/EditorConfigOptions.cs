// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    internal static class EditorConfigOptions
    {
        public static bool IsTrue(
            SyntaxNodeAnalysisContext context,
            SyntaxTree syntaxTree,
            string key)
        {
            return TryGetValue(context, syntaxTree, key, out bool value)
                && value;
        }

        public static bool TryGetValue(
            SyntaxNodeAnalysisContext context,
            SyntaxTree syntaxTree,
            string key,
            out bool value)
        {
            if (context
                .Options
                .AnalyzerConfigOptionsProvider
                .GetOptions(syntaxTree)
                .TryGetValue(key, out string valueText)
                && bool.TryParse(valueText, out value))
            {
                return true;
            }

            value = false;
            return false;
        }
    }
}
