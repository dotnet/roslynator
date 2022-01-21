// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Roslynator.Configuration
{
    public class XmlCodeAnalysisConfig
    {
        public const string FileName = "roslynator.config";

        private static readonly IEqualityComparer<string> _keyComparer = StringComparer.OrdinalIgnoreCase;

        public static XmlCodeAnalysisConfig Empty { get; } = new();

        internal XmlCodeAnalysisConfig(
            IEnumerable<string> includes = null,
            IEnumerable<KeyValuePair<string, bool>> analyzers = null,
            IEnumerable<KeyValuePair<string, bool>> codeFixes = null,
            IEnumerable<KeyValuePair<string, bool>> refactorings = null,
            IEnumerable<string> ruleSets = null,
            bool? prefixFieldIdentifierWithUnderscore = ConfigOptionDefaultValues.PrefixFieldIdentifierWithUnderscore,
            int? maxLineLength = ConfigOptionDefaultValues.MaxLineLength)
        {
            Includes = includes?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
            Analyzers = analyzers?.ToImmutableDictionary(_keyComparer) ?? ImmutableDictionary<string, bool>.Empty;
            CodeFixes = codeFixes?.ToImmutableDictionary(_keyComparer) ?? ImmutableDictionary<string, bool>.Empty;
            Refactorings = refactorings?.ToImmutableDictionary(_keyComparer) ?? ImmutableDictionary<string, bool>.Empty;
            RuleSets = ruleSets?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
            PrefixFieldIdentifierWithUnderscore = prefixFieldIdentifierWithUnderscore;
            MaxLineLength = maxLineLength;

            string path = typeof(XmlCodeAnalysisConfig).Assembly.Location;

            if (!string.IsNullOrEmpty(path))
                path = Path.Combine(Path.GetDirectoryName(path), RuleSetLoader.DefaultRuleSetName);

            RuleSet ruleSet = RuleSetLoader.Load(path, RuleSets) ?? RuleSetLoader.EmptyRuleSet;

            GeneralDiagnosticOption = ruleSet.GeneralDiagnosticOption;
            SpecificDiagnosticOptions = ruleSet.SpecificDiagnosticOptions;
        }

        public ImmutableArray<string> Includes { get; }

        public ImmutableDictionary<string, bool> Analyzers { get; }

        public ImmutableDictionary<string, bool> CodeFixes { get; }

        public ImmutableDictionary<string, bool> Refactorings { get; }

        public ImmutableArray<string> RuleSets { get; }

        public ReportDiagnostic GeneralDiagnosticOption { get; }

        public ImmutableDictionary<string, ReportDiagnostic> SpecificDiagnosticOptions { get; }

        public bool? PrefixFieldIdentifierWithUnderscore { get; }

        public int? MaxLineLength { get; }

        public static string GetDefaultConfigFilePath()
        {
            string path = typeof(XmlCodeAnalysisConfig).Assembly.Location;

            if (!string.IsNullOrEmpty(path))
            {
                path = Path.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(path))
                    return Path.Combine(path, FileName);
            }

            return null;
        }

        public DiagnosticSeverity? GetDiagnosticSeverity(string id)
        {
            if (!SpecificDiagnosticOptions.TryGetValue(id, out ReportDiagnostic reportDiagnostic))
                reportDiagnostic = GeneralDiagnosticOption;

            if (reportDiagnostic != ReportDiagnostic.Default
                && reportDiagnostic != ReportDiagnostic.Suppress)
            {
                return reportDiagnostic.ToDiagnosticSeverity();
            }

            return null;
        }

        public bool? IsDiagnosticEnabledByDefault(string id)
        {
            if (SpecificDiagnosticOptions.TryGetValue(id, out ReportDiagnostic reportDiagnostic))
            {
                if (reportDiagnostic != ReportDiagnostic.Default)
                    return reportDiagnostic != ReportDiagnostic.Suppress;
            }
            else if (GeneralDiagnosticOption == ReportDiagnostic.Suppress)
            {
                return false;
            }

            return null;
        }
    }
}
