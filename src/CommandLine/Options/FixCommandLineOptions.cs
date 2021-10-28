// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
    // NoConcurrent, RemoveEmptyFolders, RemoveUnusedSymbols
    [Verb("fix", HelpText = "Fixes diagnostics in the specified project or solution.")]
    public class FixCommandLineOptions : AbstractAnalyzeCommandLineOptions
    {
        [Option(
            longName: "batch-size",
            Default = -1,
            HelpText = "Defines maximum number of diagnostics that can be fixed in one batch.",
            MetaValue = "<BATCH_SIZE>")]
        public int BatchSize { get; set; }

        [AdditionalDescription(" If there are two (or more) fixers for a diagnostic and both provide a fix "
            + "it is necessary to determine which one should be used to fix the diagnostic. "
            + "Set verbosity to 'diagnostic' to see which diagnostics cannot be fixed due to multiple fixers.")]
        [Option(
            longName: "diagnostic-fixer-map",
            HelpText = "Defines mapping between diagnostic and its fixer (CodeFixProvider).",
            MetaValue = "<DIAGNOSTIC_ID=FIXER_FULL_NAME>")]
        public IEnumerable<string> DiagnosticFixerMap { get; set; }

        [Option(
            longName: "diagnostic-fix-map",
            HelpText = "Defines mapping between diagnostic and its fix (CodeAction).",
            MetaValue = "<DIAGNOSTIC_ID=EQUIVALENCE_KEY>")]
        public IEnumerable<string> DiagnosticFixMap { get; set; }

        [Option(
            longName: "diagnostics-fixable-one-by-one",
            HelpText = "Defines diagnostics that can be fixed even if there is no FixAllProvider for them.",
            MetaValue = "<DIAGNOSTIC_ID>")]
        public IEnumerable<string> DiagnosticsFixableOneByOne { get; set; }

        [Option(
            longName: "file-banner",
            HelpText = "Defines text that should be at the of each source file.",
            MetaValue = "<FILE_BANNER>")]
        public string FileBanner { get; set; }

        [Option(
            longName: OptionNames.FixScope,
            HelpText = "Defines fix scope. Allowed values are project (default) or document.",
            MetaValue = "<FIX_SCOPE>")]
        public string FixScope { get; set; }

        [Option(
            longName: "format",
            HelpText = "Indicates whether each document should be formatted.")]
        public bool Format { get; set; }

        [Option(
            longName: "ignore-compiler-errors",
            HelpText = "Indicates whether fixing should continue even if compilation has errors.")]
        public bool IgnoreCompilerErrors { get; set; }

        [Option(
            longName: "ignored-compiler-diagnostics",
            HelpText = "Defines compiler diagnostics that should be ignored even if --ignore-compiler-errors is not set.")]
        public IEnumerable<string> IgnoredCompilerDiagnostics { get; set; }

        [Option(
            longName: "max-iterations",
            Default = -1,
            HelpText = "Defines maximum numbers of fixing iterations.",
            MetaValue = "<MAX_ITERATIONS>")]
        public int MaxIterations { get; set; }
    }
}
