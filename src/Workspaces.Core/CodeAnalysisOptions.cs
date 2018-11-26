// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public abstract class CodeAnalysisOptions
    {
        protected CodeAnalysisOptions(
            DiagnosticSeverity severityLevel = DiagnosticSeverity.Info,
            bool ignoreAnalyzerReferences = false,
            IEnumerable<string> supportedDiagnosticIds = null,
            IEnumerable<string> ignoredDiagnosticIds = null,
            IEnumerable<string> projectNames = null,
            IEnumerable<string> ignoredProjectNames = null,
            string language = null)
        {
            if (supportedDiagnosticIds?.Any() == true
                && ignoredDiagnosticIds?.Any() == true)
            {
                throw new ArgumentException($"Cannot specify both '{nameof(supportedDiagnosticIds)}' and '{nameof(ignoredDiagnosticIds)}'.", nameof(ignoredDiagnosticIds));
            }

            if (projectNames?.Any() == true
                && ignoredProjectNames?.Any() == true)
            {
                throw new ArgumentException($"Cannot specify both '{nameof(projectNames)}' and '{nameof(ignoredProjectNames)}'.", nameof(ignoredProjectNames));
            }

            SeverityLevel = severityLevel;
            IgnoreAnalyzerReferences = ignoreAnalyzerReferences;
            SupportedDiagnosticIds = supportedDiagnosticIds?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            IgnoredDiagnosticIds = ignoredDiagnosticIds?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            ProjectNames = projectNames?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            IgnoredProjectNames = ignoredProjectNames?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            Language = language;
        }

        public DiagnosticSeverity SeverityLevel { get; }

        public bool IgnoreAnalyzerReferences { get; }

        public ImmutableHashSet<string> SupportedDiagnosticIds { get; }

        public ImmutableHashSet<string> IgnoredDiagnosticIds { get; }

        public ImmutableHashSet<string> ProjectNames { get; }

        public ImmutableHashSet<string> IgnoredProjectNames { get; }

        public string Language { get; }

        internal bool IsSupportedDiagnostic(Diagnostic diagnostic)
        {
            if (diagnostic.Severity >= SeverityLevel)
            {
                return (SupportedDiagnosticIds.Count > 0)
                    ? SupportedDiagnosticIds.Contains(diagnostic.Id)
                    : !IgnoredDiagnosticIds.Contains(diagnostic.Id);
            }

            return false;
        }

        internal bool IsSupportedProject(Project project)
        {
            if (SyntaxFactsService.IsSupportedLanguage(project.Language)
                && (Language == null || Language == project.Language))
            {
                return (ProjectNames.Count > 0)
                    ? ProjectNames.Contains(project.Name)
                    : !IgnoredProjectNames.Contains(project.Name);
            }

            return false;
        }
    }
}
