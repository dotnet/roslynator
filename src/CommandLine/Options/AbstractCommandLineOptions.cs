// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    public abstract class AbstractCommandLineOptions
    {
        [Option(
            shortName: OptionShortNames.Help,
            longName: OptionNames.Help,
            HelpText = "Show command line help.")]
        public bool Help { get; set; }

        [Option(
            shortName: OptionShortNames.Verbosity,
            longName: "verbosity",
            HelpText = "Verbosity of the log. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed] and diag[nostic].",
            MetaValue = "<LEVEL>")]
        public string Verbosity { get; set; }
    }
}
