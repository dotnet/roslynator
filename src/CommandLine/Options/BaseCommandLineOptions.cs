// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    public class BaseCommandLineOptions : AbstractCommandLineOptions
    {
        [Option(
            longName: "file-log",
            HelpText = "Path to a file that should store output.",
            MetaValue = "<FILE_PATH>")]
        public string FileLog { get; set; }

        [Option(
            longName: "file-log-verbosity",
            HelpText = "Verbosity of the file log. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed] and diag[nostic].",
            MetaValue = "<LEVEL>")]
        public string FileLogVerbosity { get; set; }
    }
}
