// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    internal static class CommonExtensions
    {
        public static bool IsEnabled(
            this AnalyzerOptionDescriptor analyzerOption,
            SyntaxNodeAnalysisContext context)
        {
            return IsEnabled(
                analyzerOption,
                context.Node.SyntaxTree,
                context.Options);
        }

        public static bool? IsEnabled(
            this AnalyzerOptionDescriptor analyzerOption,
            SyntaxNodeAnalysisContext context,
            bool checkParent)
        {
            return IsEnabled(
                analyzerOption,
                context.Node.SyntaxTree,
                context.Compilation.Options,
                context.Options,
                checkParent);
        }

        public static bool IsEnabled(
            this AnalyzerOptionDescriptor analyzerOption,
            SymbolAnalysisContext context)
        {
            return IsEnabled(
                analyzerOption,
                context.Symbol.Locations[0].SourceTree,
                context.Options);
        }

        public static bool? IsEnabled(
            this AnalyzerOptionDescriptor analyzerOption,
            SyntaxTree syntaxTree,
            CompilationOptions compilationOptions,
            AnalyzerOptions analyzerOptions,
            bool checkParent)
        {
            if (checkParent && !analyzerOption.Descriptor.IsEffective(syntaxTree, compilationOptions))
                return null;

            return IsEnabled(analyzerOption, syntaxTree, analyzerOptions);
        }

        public static bool IsEnabled(
            this AnalyzerOptionDescriptor analyzerOption,
            SyntaxTree syntaxTree,
            AnalyzerOptions analyzerOptions)
        {
            if (analyzerOptions
                .AnalyzerConfigOptionsProvider
                .GetOptions(syntaxTree)
                .TryGetValue(analyzerOption.OptionKey, out string value)
                && bool.TryParse(value, out bool result))
            {
                return result;
            }

            return false;
        }

        public static bool TryGetInt32Value(
            this AnalyzerOptionDescriptor analyzerOption,
            SyntaxTree syntaxTree,
            AnalyzerOptions analyzerOptions,
            out int result)
        {
            if (analyzerOptions
                .AnalyzerConfigOptionsProvider
                .GetOptions(syntaxTree)
                .TryGetValue(analyzerOption.OptionKey, out string textValue)
                && int.TryParse(textValue, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CurrentCulture, out int value))
            {
                result = value;
                return true;
            }

            result = default;
            return false;
        }

        public static int GetInt32Value(
            this AnalyzerOptionDescriptor analyzerOption,
            SyntaxTree syntaxTree,
            AnalyzerOptions analyzerOptions,
            int defaultValue)
        {
            return (TryGetInt32Value(analyzerOption, syntaxTree, analyzerOptions, out int result))
                ? result
                : defaultValue;
        }
    }
}
