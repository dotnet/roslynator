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
        public const string FileName = "roslynator.editorconfig";

        internal static EditorConfigCodeAnalysisConfig Empty { get; } = new EditorConfigCodeAnalysisConfig();

        public EditorConfigCodeAnalysisConfig(
            IEnumerable<KeyValuePair<string, string>> options = null,
            IEnumerable<KeyValuePair<string, ReportDiagnostic>> analyzers = null,
            IEnumerable<KeyValuePair<string, ReportDiagnostic>> analyzerCategories = null,
            IEnumerable<KeyValuePair<string, bool>> refactorings = null,
            IEnumerable<KeyValuePair<string, bool>> codeFixes = null)
        {
            Options = options?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty;
            Analyzers = analyzers?.ToImmutableDictionary() ?? ImmutableDictionary<string, ReportDiagnostic>.Empty;
            AnalyzerCategories = analyzerCategories?.ToImmutableDictionary() ?? ImmutableDictionary<string, ReportDiagnostic>.Empty;
            Refactorings = refactorings?.ToImmutableDictionary() ?? ImmutableDictionary<string, bool>.Empty;
            CodeFixes = codeFixes?.ToImmutableDictionary() ?? ImmutableDictionary<string, bool>.Empty;

            if (Options.TryGetValue(OptionKeys.MaxLineLength, out string maxLineLengthRaw)
                && int.TryParse(maxLineLengthRaw, out int maxLineLength))
            {
                MaxLineLength = maxLineLength;
            }

            if (Options.TryGetValue(OptionKeys.PrefixFieldIdentifierWithUnderscore, out string prefixFieldIdentifierWithUnderscoreRaw)
                && bool.TryParse(prefixFieldIdentifierWithUnderscoreRaw, out bool prefixFieldIdentifierWithUnderscore))
            {
                PrefixFieldIdentifierWithUnderscore = prefixFieldIdentifierWithUnderscore;
            }
        }

        public int? MaxLineLength { get; }

        public bool? PrefixFieldIdentifierWithUnderscore { get; }

        public ImmutableDictionary<string, string> Options { get; }

        public ImmutableDictionary<string, ReportDiagnostic> Analyzers { get; }

        public ImmutableDictionary<string, ReportDiagnostic> AnalyzerCategories { get; }

        public ImmutableDictionary<string, bool> Refactorings { get; }

        public ImmutableDictionary<string, bool> CodeFixes { get; }

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

        internal IEnumerable<string> GetDisabledRefactorings()
        {
            foreach (KeyValuePair<string, bool> kvp in Refactorings)
            {
                if (!kvp.Value)
                    yield return kvp.Key;
            }
        }

        internal IEnumerable<string> GetDisabledCodeFixes()
        {
            foreach (KeyValuePair<string, bool> kvp in CodeFixes)
            {
                if (!kvp.Value)
                    yield return kvp.Key;
            }
        }

        public DiagnosticSeverity? GetDiagnosticSeverity(string id, string category)
        {
            ReportDiagnostic? reportDiagnostic = GetReportDiagnostic(id, category);

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

        public bool? IsDiagnosticEnabled(string id, string category)
        {
            switch (GetReportDiagnostic(id, category))
            {
                case ReportDiagnostic.Error:
                case ReportDiagnostic.Warn:
                case ReportDiagnostic.Info:
                case ReportDiagnostic.Hidden:
                    return true;
                case ReportDiagnostic.Suppress:
                    return false;
            }

            return null;
        }

        private ReportDiagnostic? GetReportDiagnostic(string id, string category)
        {
            ReportDiagnostic? reportDiagnostic = null;

            if (AnalyzerCategories.TryGetValue(category, out ReportDiagnostic reportDiagnostic2))
                reportDiagnostic = reportDiagnostic2;

            if (Analyzers.TryGetValue(id, out ReportDiagnostic reportDiagnostic3))
                reportDiagnostic = reportDiagnostic3;

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

                    File.WriteAllText(path, CreateDefaultContent(), Encoding.UTF8);
                }
                catch (Exception ex) when (ex is IOException
                    || ex is UnauthorizedAccessException)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            return path;
        }

        private static string CreateDefaultContent()
        {
            using var writer = new EditorConfigWriter(new StringWriter());

            writer.WriteCommentChar();
            writer.WriteLine("This config file enables to change DEFAULT configuration of analyzers, refactorings and code fixes.");

            writer.WriteCommentChar();
            writer.WriteLine("Config is loaded once when the IDE starts. Therefore a restart of the IDE is required for changes to take effect.");
            writer.WriteLine();

            writer.WriteGlobalDirective();
            writer.WriteLine();
            writer.WriteEntry(OptionKeys.MaxLineLength, OptionDefaultValues.MaxLineLength.ToString());
            writer.WriteEntry(OptionKeys.PrefixFieldIdentifierWithUnderscore, OptionDefaultValues.PrefixFieldIdentifierWithUnderscore.ToString().ToLowerInvariant());
            writer.WriteLine();
            writer.WriteEntry(OptionKeys.RefactoringEnabled, true);
            writer.WriteCommentChar();
            writer.WriteRefactoring("RR0001", true);
            writer.WriteLine();
            writer.WriteEntry(OptionKeys.CompilerDiagnosticFixEnabled, true);
            writer.WriteCommentChar();
            writer.WriteCompilerDiagnosticFix("CS0001", true);
            writer.WriteLine();

            const string allSeverities = "default|none|silent|suggestion|warning|error";

            writer.WriteCommentChar();
            writer.WriteAnalyzerCategory(DiagnosticCategories.Roslynator.ToLowerInvariant(), allSeverities);
            writer.WriteLine();

            writer.WriteEntry("dotnet_diagnostic.RCS0001.severity", allSeverities);

            return writer.ToString();
        }
    }
}
