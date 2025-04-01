// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine;

[Verb("analyze", HelpText = "Analyzes specified project or solution and reports diagnostics.")]
public class AnalyzeCommandLineOptions : AbstractAnalyzeCommandLineOptions
{
    [Option(
        longName: "execution-time",
        HelpText = "Indicates whether to measure execution time of each analyzer.")]
    public bool ExecutionTime { get; set; }

    [Option(
        longName: "ignore-compiler-diagnostics",
        HelpText = "Indicates whether to display compiler diagnostics.")]
    public bool IgnoreCompilerDiagnostics { get; set; }

    [Option(
        shortName: OptionShortNames.Output,
        longName: "output",
        HelpText = "Defines path to file that will store reported diagnostics. The format of the file is determined by the --output-format option, with the default being xml.",
        MetaValue = "<FILE_PATH>")]
    public string Output { get; set; }

    [Option(
        longName: "output-format",
        HelpText = "Defines the file format of the report written to file. Supported options are: gitlab and xml, with xml the default if no option is provided.")]
    public string OutputFormat { get; set; }

    [Option(
        longName: "report-not-configurable",
        HelpText = "Indicates whether diagnostics with 'NotConfigurable' tag should be reported.")]
    public bool ReportNotConfigurable { get; set; }

    [Option(
        longName: "report-suppressed-diagnostics",
        HelpText = "Indicates whether suppressed diagnostics should be reported.")]
    public bool ReportSuppressedDiagnostics { get; set; }

    internal bool TryParseOutputFormat(string defaultValue)
    {
        return ParseHelpers.TryParseOutputFormat(OutputFormat, defaultValue);
    }
}
