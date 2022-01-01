// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    internal static class CommonWorkspaceExtensions
    {
        public static bool TryGetAnalyzerOptionValue(
            this Document document,
            SyntaxNode node,
            string optionKey,
            out string value)
        {
            return TryGetAnalyzerOptionValue(document, node.SyntaxTree, optionKey, out value);
        }

        public static bool TryGetAnalyzerOptionValue(
            this Document document,
            SyntaxToken token,
            string optionKey,
            out string value)
        {
            return TryGetAnalyzerOptionValue(document, token.SyntaxTree, optionKey, out value);
        }

        public static bool TryGetAnalyzerOptionValue(
            this Document document,
            SyntaxTree syntaxTree,
            string optionKey,
            out string value)
        {
            if (document
                .Project
                .AnalyzerOptions
                .AnalyzerConfigOptionsProvider
                .GetOptions(syntaxTree)
                .TryGetValue(optionKey, out value))
            {
                return true;
            }

            value = null;
            return false;
        }

        internal static AnalyzerConfigOptions GetConfigOptions(this Document document, SyntaxTree syntaxTree)
        {
            return document.Project.AnalyzerOptions.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);
        }
    }
}
