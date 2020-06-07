// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;
using Roslynator.Configuration;

namespace Roslynator
{
    internal class AnalyzerRules
    {
        public static AnalyzerRules Default { get; } = Create();

        public ReportDiagnostic GeneralDiagnosticOption { get; }

        public ImmutableDictionary<string, ReportDiagnostic> SpecificDiagnosticOptions { get; }

        public AnalyzerRules(
            ReportDiagnostic generalDiagnosticOption,
            ImmutableDictionary<string, ReportDiagnostic> specificDiagnosticOptions)
        {
            GeneralDiagnosticOption = generalDiagnosticOption;
            SpecificDiagnosticOptions = specificDiagnosticOptions;
        }

        public DiagnosticSeverity GetDiagnosticSeverityOrDefault(string id, DiagnosticSeverity defaultValue)
        {
            if (!SpecificDiagnosticOptions.TryGetValue(id, out ReportDiagnostic reportDiagnostic))
                reportDiagnostic = GeneralDiagnosticOption;

            if (reportDiagnostic != ReportDiagnostic.Default
                && reportDiagnostic != ReportDiagnostic.Suppress)
            {
                return reportDiagnostic.ToDiagnosticSeverity();
            }

            return defaultValue;
        }

        public bool IsDiagnosticEnabledOrDefault(string id, bool defaultValue)
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

            return defaultValue;
        }

        private static AnalyzerRules Create()
        {
            string path = typeof(AnalyzerRules).Assembly.Location;

            if (!string.IsNullOrEmpty(path))
                path = Path.Combine(Path.GetDirectoryName(path), RuleSetUtility.DefaultRuleSetName);

            RuleSet ruleSet = RuleSetUtility.Load(path, CodeAnalysisConfiguration.Current.RuleSets) ?? RuleSetUtility.EmptyRuleSet;

            return new AnalyzerRules(
                ruleSet.GeneralDiagnosticOption,
                ruleSet.SpecificDiagnosticOptions);
        }
    }
}
