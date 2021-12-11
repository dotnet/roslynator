// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Roslynator.Configuration
{
    internal static class EditorConfigCodeAnalysisConfigLoader
    {
        public static EditorConfigCodeAnalysisConfig Load(IEnumerable<string> paths)
        {
            return LoadAndCatchIfThrows(
                paths,
                ex => Debug.Fail(ex.ToString()))
                ?? EditorConfigCodeAnalysisConfig.Empty;
        }

        internal static EditorConfigCodeAnalysisConfig LoadAndCatchIfThrows(IEnumerable<string> paths, Action<Exception> exceptionHandler)
        {
            try
            {
                return LoadInternal(paths);
            }
            catch (Exception ex) when (ex is IOException
                || ex is UnauthorizedAccessException)
            {
                exceptionHandler(ex);
                return null;
            }
        }

        private static EditorConfigCodeAnalysisConfig LoadInternal(IEnumerable<string> paths)
        {
            using (IEnumerator<string> en = paths.GetEnumerator())
            {
                if (!en.MoveNext())
                    return null;

                EditorConfigData config = LoadFile(en.Current);

                var options = new Dictionary<string, string>();
                var categories = new Dictionary<string, ReportDiagnostic>();
                var refactorings = new Dictionary<string, bool>();
                var codeFixes = new Dictionary<string, bool>();

                ImmutableDictionary<string, string> allOptions = config.Options;
                ImmutableDictionary<string, ReportDiagnostic> analyzerOptions = config.AnalyzerOptions;

                while (en.MoveNext())
                {
                    EditorConfigData nextConfig = LoadFile(en.Current);

                    if (nextConfig.Options != null)
                        allOptions = allOptions.SetItems(nextConfig.Options);

                    if (nextConfig.AnalyzerOptions != null)
                        analyzerOptions = analyzerOptions.SetItems(nextConfig.AnalyzerOptions);
                }

                foreach (KeyValuePair<string, string> option in allOptions)
                {
                    Match match = Regexes.RefactoringOption.Match(option.Key);

                    if (match.Success)
                    {
                        if (bool.TryParse(option.Value, out bool enabled))
                            refactorings.Add(match.Groups["id"].Value, enabled);
                    }
                    else
                    {
                        match = Regexes.CodeFixOption.Match(option.Key);

                        if (match.Success)
                        {
                            if (bool.TryParse(option.Value, out bool enabled))
                                codeFixes.Add(match.Groups["id"].Value, enabled);
                        }
                        else
                        {
                            match = Regexes.CategoryOption.Match(option.Key);

                            if (match.Success)
                            {
                                string category = match.Groups["category"].Value;
                                ReportDiagnostic? reportDiagnostic = ParseReportDiagnostic(option.Value);

                                if (reportDiagnostic != null)
                                    categories.Add(category, reportDiagnostic.Value);
                            }
                        }
                    }
                }

                return new EditorConfigCodeAnalysisConfig(
                    options,
                    analyzerOptions,
                    categories,
                    refactorings,
                    codeFixes);
            }

            static ReportDiagnostic? ParseReportDiagnostic(string value)
            {
                switch (value)
                {
                    case "default":
                        return ReportDiagnostic.Default;
                    case "none":
                        return ReportDiagnostic.Suppress;
                    case "silent":
                        return ReportDiagnostic.Hidden;
                    case "suggestion":
                        return ReportDiagnostic.Info;
                    case "warning":
                        return ReportDiagnostic.Warn;
                    case "error":
                        return ReportDiagnostic.Error;
                }

                Debug.Fail(value);
                return null;
            }
        }

        private static EditorConfigData LoadFile(string path)
        {
            if (!File.Exists(path))
                return default;

            string text = File.ReadAllText(path);

            AnalyzerConfig config = AnalyzerConfig.Parse(text, path);

            AnalyzerConfigSet configSet = AnalyzerConfigSet.Create(ImmutableArray.Create(config), out ImmutableArray<Diagnostic> diagnostics);

            foreach (Diagnostic diagnostic in diagnostics)
                Debug.WriteLine(diagnostic.GetMessage());

            return new EditorConfigData(
                configSet.GlobalConfigOptions.AnalyzerOptions,
                configSet.GlobalConfigOptions.TreeOptions);
        }

        private readonly struct EditorConfigData
        {
            public EditorConfigData(
                ImmutableDictionary<string, string> options,
                ImmutableDictionary<string, ReportDiagnostic> analyzerOptions)
            {
                Options = options;
                AnalyzerOptions = analyzerOptions;
            }

            public ImmutableDictionary<string, string> Options { get; }

            public ImmutableDictionary<string, ReportDiagnostic> AnalyzerOptions { get; }
        }

        private static class Regexes
        {
            public static readonly Regex RefactoringOption = new(
                @"
\A
"
                    + Regex.Escape(OptionKeys.RefactoringPrefix)
                    + @"
(?<id>(?i:RR){2}[0-9]{4})
.enabled
\z
",
                RegexOptions.IgnorePatternWhitespace);

            public static readonly Regex CodeFixOption = new(
                @"
\A
"
                    + Regex.Escape(OptionKeys.CompilerDiagnosticFixPrefix)
                    + @"
(?<id>(?i:CS)[0-9]{4})
.enabled
\z
",
                RegexOptions.IgnorePatternWhitespace);

            public static readonly Regex CategoryOption = new(
                @"
\A
"
                    + Regex.Escape(EditorConfigWriter.AnalyzerCategoryPrefix)
                    + @"
(?<category>[^.]+)
.severity
\z
",
                RegexOptions.IgnorePatternWhitespace);
        }
    }
}
