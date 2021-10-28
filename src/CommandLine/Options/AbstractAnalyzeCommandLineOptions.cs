// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;
using Microsoft.CodeAnalysis;

namespace Roslynator.CommandLine
{
    public abstract class AbstractAnalyzeCommandLineOptions : MSBuildCommandLineOptions
    {
        [Value(
            index: 0,
            HelpText = "Path to one or more project/solution files.",
            MetaName = "<PROJECT|SOLUTION>")]
        public IEnumerable<string> Paths { get; set; }

        [Option(
            shortName: OptionShortNames.AnalyzerAssemblies,
            longName: "analyzer-assemblies",
            HelpText = "Define one or more paths to an analyzer assembly or a directory that should be searched recursively for analyzer assemblies.",
            MetaValue = "<PATH>")]
        public IEnumerable<string> AnalyzerAssemblies { get; set; }

        [Option(
            longName: "culture",
            HelpText = "Defines culture that should be used to display diagnostic message.",
            MetaValue = "<CULTURE_ID>")]
        public string Culture { get; set; }

        [Option(
            longName: "ignore-analyzer-references",
            HelpText = "Indicates whether analyzers that are referenced in a project should be ignored.")]
        public bool IgnoreAnalyzerReferences { get; set; }

        [Option(
            longName: "ignored-diagnostics",
            HelpText = "Defines diagnostics that should not be reported.",
            MetaValue = "<DIAGNOSTIC_ID>")]
        public IEnumerable<string> IgnoredDiagnostics { get; set; }

        [Option(
            longName: OptionNames.SeverityLevel,
            HelpText = "Defines minimally required severity for a diagnostic. Allowed values are hidden, info (default), warning or error.",
            MetaValue = "<LEVEL>")]
        public string SeverityLevel { get; set; }

        [Option(
            longName: "supported-diagnostics",
            HelpText = "Defines diagnostics that should be reported.",
            MetaValue = "<DIAGNOSTIC_ID>")]
        public IEnumerable<string> SupportedDiagnostics { get; set; }

        internal bool TryParseDiagnosticSeverity(DiagnosticSeverity defaultValue, out DiagnosticSeverity value)
        {
            return ParseHelpers.TryParseOptionValueAsEnum(SeverityLevel, OptionNames.SeverityLevel, out value, defaultValue);
        }
    }
}
