// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class CommonWorkspaceExtensions
    {
        public static bool IsEnabled(
            this AnalyzerOptionDescriptor analyzerOption,
            Document document,
            SyntaxNode node)
        {
            return IsEnabled(analyzerOption, document, node.SyntaxTree);
        }

        public static bool IsEnabled(
            this AnalyzerOptionDescriptor analyzerOption,
            Document document,
            SyntaxToken token)
        {
            return IsEnabled(analyzerOption, document, token.SyntaxTree);
        }

        public static bool IsEnabled(
            this AnalyzerOptionDescriptor analyzerOption,
            Document document,
            SyntaxTree syntaxTree)
        {
            return analyzerOption.Descriptor.IsEffective(syntaxTree, document.Project.CompilationOptions)
                && analyzerOption.IsEnabled(
                    syntaxTree,
                    document.Project.AnalyzerOptions);
        }

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
    }
}
