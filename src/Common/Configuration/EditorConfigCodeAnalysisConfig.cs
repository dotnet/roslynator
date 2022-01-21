// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Roslynator.Configuration
{
    internal class EditorConfigCodeAnalysisConfig
    {
        public const string FileName = ".roslynatorconfig";

        internal const string FileDefaultContent = @"# Roslynator Config File

is_global = true

# Options in this file can be used to change default configuration of analyzers, refactorings and compiler diagnostic fixes.
# Default configuration is loaded once when IDE starts. Therefore, it may be necessary to restart IDE for changes to take effect.
# Full list of available options: https://github.com/josefpihrt/roslynator/docs/configuration.md

# Set severity for all analyzers that are enabled by default (https://docs.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022#set-rule-severity-of-multiple-analyzer-rules-at-once-in-an-editorconfig-file)
dotnet_analyzer_diagnostic.category-roslynator.severity = default|none|silent|suggestion|warning|error

# Enable/disable all analyzers by default.
# NOTE: This option can be used only in .roslynatorconfig file
roslynator_analyzers.enabled_by_default = true|false

## Set severity for a specific analyzer
#dotnet_diagnostic.<ANALYZER_ID>.severity = default|none|silent|suggestion|warning|error

## Enable/disable all refactorings
#roslynator_refactorings.enabled = true|false

## Enable/disable specific refactoring
#roslynator_refactoring.<REFACTORING_NAME>.enabled = true|false

## Enable/disable all compiler diagnostic fixes
#roslynator_compiler_diagnostic_fixes.enabled = true|false

## Enable/disable specific compiler diagnostic fix
#roslynator_compiler_diagnostic_fix.<COMPILER_DIAGNOSTIC_ID>.enabled = true|false
";

        internal static EditorConfigCodeAnalysisConfig Empty { get; } = new();

        public EditorConfigCodeAnalysisConfig(
            IEnumerable<KeyValuePair<string, string>> options = null,
            IEnumerable<KeyValuePair<string, ReportDiagnostic>> analyzers = null,
            IEnumerable<KeyValuePair<string, ReportDiagnostic>> analyzerCategories = null,
            IEnumerable<KeyValuePair<string, bool>> refactorings = null,
            IEnumerable<KeyValuePair<string, bool>> codeFixes = null,
            bool? analyzersEnabledByDefault = null)
        {
            Options = options?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty;
            Analyzers = analyzers?.ToImmutableDictionary(StringComparer.InvariantCultureIgnoreCase) ?? ImmutableDictionary<string, ReportDiagnostic>.Empty;
            AnalyzerCategories = analyzerCategories?.ToImmutableDictionary(StringComparer.InvariantCultureIgnoreCase) ?? ImmutableDictionary<string, ReportDiagnostic>.Empty;
            Refactorings = refactorings?.ToImmutableDictionary(StringComparer.InvariantCultureIgnoreCase) ?? ImmutableDictionary<string, bool>.Empty;
            CodeFixes = codeFixes?.ToImmutableDictionary(StringComparer.InvariantCultureIgnoreCase) ?? ImmutableDictionary<string, bool>.Empty;

            if (Options.TryGetValue(ConfigOptionKeys.MaxLineLength, out string maxLineLengthRaw)
                && int.TryParse(maxLineLengthRaw, out int maxLineLength))
            {
                MaxLineLength = maxLineLength;
            }

            if (Options.TryGetValue(ConfigOptionKeys.PrefixFieldIdentifierWithUnderscore, out string prefixFieldIdentifierWithUnderscoreRaw)
                && bool.TryParse(prefixFieldIdentifierWithUnderscoreRaw, out bool prefixFieldIdentifierWithUnderscore))
            {
                PrefixFieldIdentifierWithUnderscore = prefixFieldIdentifierWithUnderscore;
            }

            AnalyzersEnabledByDefault = analyzersEnabledByDefault;
        }

        public int? MaxLineLength { get; }

        public bool? PrefixFieldIdentifierWithUnderscore { get; }

        public ImmutableDictionary<string, string> Options { get; }

        public ImmutableDictionary<string, ReportDiagnostic> Analyzers { get; }

        public ImmutableDictionary<string, ReportDiagnostic> AnalyzerCategories { get; }

        public ImmutableDictionary<string, bool> Refactorings { get; }

        public ImmutableDictionary<string, bool> CodeFixes { get; }

        public bool? AnalyzersEnabledByDefault { get; }

        internal IReadOnlyDictionary<string, bool> GetRefactorings()
        {
            return Refactorings;
        }

        internal IReadOnlyDictionary<string, bool> GetCodeFixes()
        {
            return CodeFixes;
        }

        public static string GetDefaultConfigFilePath()
        {
            string path = typeof(EditorConfigCodeAnalysisConfig).Assembly.Location;

            if (!string.IsNullOrEmpty(path))
            {
                path = Path.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(path))
                    return Path.Combine(path, FileName);
            }

            return null;
        }

        public DiagnosticSeverity? GetDiagnosticSeverity(string id, string category, bool isEnabledByDefault)
        {
            ReportDiagnostic? reportDiagnostic = GetReportDiagnostic(id, category, isEnabledByDefault);

            switch (reportDiagnostic)
            {
                case ReportDiagnostic.Error:
                case ReportDiagnostic.Warn:
                case ReportDiagnostic.Info:
                case ReportDiagnostic.Hidden:
                    return reportDiagnostic.Value.ToDiagnosticSeverity();
            }

            return null;
        }

        public bool? IsDiagnosticEnabledByDefault(string id, string category, bool defaultValue)
        {
            switch (GetReportDiagnostic(id, category, defaultValue))
            {
                case ReportDiagnostic.Error:
                case ReportDiagnostic.Warn:
                case ReportDiagnostic.Info:
                case ReportDiagnostic.Hidden:
                    return true;
                case ReportDiagnostic.Suppress:
                    return false;
            }

            return AnalyzersEnabledByDefault ?? defaultValue;
        }

        private ReportDiagnostic? GetReportDiagnostic(string id, string category, bool isEnabledByDefault)
        {
            ReportDiagnostic? reportDiagnostic = null;

            if (isEnabledByDefault
                && AnalyzerCategories.TryGetValue(category, out ReportDiagnostic categoryReportDiagnostic))
            {
                reportDiagnostic = categoryReportDiagnostic;
            }

            if (Analyzers.TryGetValue(id, out ReportDiagnostic analyzerReportDiagnostic))
                reportDiagnostic = analyzerReportDiagnostic;

            return reportDiagnostic;
        }

        public static void Save(
            string path,
            IEnumerable<KeyValuePair<string, string>> options,
            IEnumerable<KeyValuePair<string, bool>> refactorings,
            IEnumerable<KeyValuePair<string, bool>> codeFixes)
        {
            using var writer = new EditorConfigWriter(new StringWriter());

            writer.WriteGlobalDirective();
            writer.WriteLine();
            writer.WriteEntries(options.OrderBy(f => f.Key));
            writer.WriteLineIf(options.Any());
            writer.WriteRefactorings(refactorings.OrderBy(f => f.Key));
            writer.WriteLineIf(refactorings.Any());
            writer.WriteCompilerDiagnosticFixes(codeFixes.OrderBy(f => f.Key));

            File.WriteAllText(path, writer.ToString());
        }

        internal static string CreateDefaultConfigFileIfNotExists()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "JosefPihrt",
                "Roslynator",
                FileName);

            if (!File.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                    File.WriteAllText(path, FileDefaultContent, Encoding.UTF8);
                }
                catch (Exception ex) when (ex is IOException
                    || ex is UnauthorizedAccessException)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            return path;
        }
    }
}
