// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator.Diagnostics
{
    public class CodeAnalyzerOptions : CodeAnalysisOptions
    {
        public static CodeAnalyzerOptions Default { get; } = new CodeAnalyzerOptions();

        public CodeAnalyzerOptions(
            bool ignoreAnalyzerReferences = false,
            bool ignoreCompilerDiagnostics = false,
            bool reportNotConfigurable = false,
            bool reportSuppressedDiagnostics = false,
            bool executionTime = false,
            DiagnosticSeverity severityLevel = DiagnosticSeverity.Info,
            IEnumerable<string> supportedDiagnosticIds = null,
            IEnumerable<string> ignoredDiagnosticIds = null,
            IEnumerable<string> projectNames = null,
            IEnumerable<string> ignoredProjectNames = null,
            string language = null) : base(severityLevel, ignoreAnalyzerReferences, supportedDiagnosticIds, ignoredDiagnosticIds, projectNames, ignoredProjectNames, language)
        {
            IgnoreCompilerDiagnostics = ignoreCompilerDiagnostics;
            ReportNotConfigurable = reportNotConfigurable;
            ReportSuppressedDiagnostics = reportSuppressedDiagnostics;
            ExecutionTime = executionTime;
        }

        public bool IgnoreCompilerDiagnostics { get; }

        public bool ReportNotConfigurable { get; }

        public bool ReportSuppressedDiagnostics { get; }

        public bool ExecutionTime { get; }
    }
}
