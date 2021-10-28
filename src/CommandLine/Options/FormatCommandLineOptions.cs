// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("format", HelpText = "Formats documents in the specified project or solution.")]
    public class FormatCommandLineOptions : MSBuildCommandLineOptions
    {
        [Value(
            index: 0,
            HelpText = "Path to one or more project/solution files.",
            MetaName = "<PROJECT|SOLUTION>")]
        public IEnumerable<string> Paths { get; set; }

        [Option(
            longName: "culture",
            HelpText = "Defines culture that should be used to display diagnostic message.",
            MetaValue = "<CULTURE_ID>")]
        public string Culture { get; set; }

        [Hidden]
        [Option(
            longName: OptionNames.EndOfLine,
            HelpText = "Defines end of line character(s). Allowed values are lf or crlf.",
            MetaValue = "<END_OF_LINE>")]
        public string EndOfLine { get; set; }

        [Option(
            shortName: OptionShortNames.IncludeGeneratedCode,
            longName: "include-generated-code",
            HelpText = "Indicates whether generated code should be formatted.")]
        public bool IncludeGeneratedCode { get; set; }
    }
}
